using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Dehempe.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Net.Pkcs11Interop.Common;
using Net.Pkcs11Interop.HighLevelAPI;

namespace Dehempe.Infrastructure.Dmp.Auth.Pkcs11;

/// <summary>
/// Situation d'exercice du praticien lue depuis un objet <c>CPS_ACTIVITY_xx_PS</c> de la carte.
/// </summary>
/// <param name="Index">Numéro d'exercice (1..15).</param>
/// <param name="StructureId">Identifiant national de la structure (<c>Struct_IdNat</c>) — valeur du <c>Identifiant_Structure</c> VIHF.</param>
/// <param name="StructureName">Raison sociale de la structure.</param>
/// <param name="SectorCode">Code secteur d'activité (ex: <c>SA07</c> = libéral, <c>SA01</c> = salarié hospitalier).</param>
public sealed record CpsActivity(int Index, string StructureId, string StructureName, string SectorCode);

/// <summary>
/// Charge la librairie PKCS#11 du middleware CPS, ouvre une session sur le slot du token
/// et expose :
///
/// <list type="bullet">
/// <item>les objets PUBLICS de la carte (certificats, <c>CPS_INFO_PS</c>) — sans PIN.</item>
/// <item>la signature avec la clé privée d'authentification — login déclenché à la demande
///       avec un PIN fourni par l'appelant (UI ou tunnel mTLS), JAMAIS depuis la config.</item>
/// </list>
///
/// Singleton dans le DI : la session reste ouverte pour toute la vie du process.
/// La clé privée ne quitte jamais le token.
/// </summary>
internal sealed class Pkcs11CpsKeyStore : IDisposable
{
    /// <summary>
    /// Suffixes de <c>CKA_ID</c> identifiant les paires (cert, clé) sur une carte CPS3.
    /// Convention IAS-ECC : dernier octet = <c>0x20</c> pour la paire d'AUTHENTIFICATION
    /// (mTLS, ClientCertVerify), <c>0x10</c> pour la paire de SIGNATURE
    /// (assertions VIHF, documents). Les deux paires sont obligatoirement présentes sur
    /// une CPS3 personnelle ; ne jamais signer le VIHF avec la clé d'auth — le DMP rejette
    /// avec « Le certificat ayant signé le VIHF est invalide : Not a valid signature certificate ».
    /// </summary>
    private const byte AuthCkaIdLastByte      = 0x20;
    private const byte SignatureCkaIdLastByte = 0x10;

    /// <summary>
    /// Emplacements connus de la librairie PKCS#11 du middleware CPS de l'ANS,
    /// par ordre de préférence. Le premier fichier existant gagne.
    /// </summary>
    private static readonly string[] MacOsLibraryPaths =
    {
        "/usr/local/lib/libcps3_pkcs11_osx.dylib",
        "/Library/Frameworks/cps3.framework/cps3_pkcs11",
    };

    private static readonly string[] WindowsLibraryPaths =
    {
        @"C:\Windows\System32\cps3_pkcs11_w64.dll",
        @"C:\Program Files\santeestoolbox\cps3_pkcs11_w64.dll",
        @"C:\Windows\System32\cps3_pkcs11.dll",
    };

    private static readonly string[] LinuxLibraryPaths =
    {
        "/usr/lib/libcps3_pkcs11.so",
        "/usr/local/lib/libcps3_pkcs11.so",
    };

    /// <summary>Header HTTP utilisé pour transmettre le PIN de la carte CPS depuis le frontend.</summary>
    public const string PinHeaderName = "X-Cps-Pin";

    private readonly Pkcs11InteropFactories _factories = new();
    private readonly CpsOptions             _options;
    private readonly IHttpContextAccessor   _http;
    private readonly ILogger<Pkcs11CpsKeyStore> _logger;

    private IPkcs11Library?  _library;
    private ISlot?           _slot;
    private Net.Pkcs11Interop.HighLevelAPI.ISession? _session;
    private IObjectHandle?   _authPrivateKey;
    private IObjectHandle?   _signaturePrivateKey;
    private X509Certificate2? _authCertificate;
    private X509Certificate2? _signatureCertificate;
    private byte[]?           _authCkaId;       // CKA_ID du cert d'auth, utilisé pour apparier la clé privée après login
    private byte[]?           _signatureCkaId;  // idem pour le cert de signature
    private IReadOnlyList<CpsActivity>? _activities; // situations d'exercice lues sur la carte (cache)
    private readonly object   _initLock  = new();
    private readonly object   _loginLock = new();
    private bool              _libraryLoaded;
    private bool              _loggedIn;

    private readonly string? _resolvedLibraryPath;

    public Pkcs11CpsKeyStore(
        IOptions<CpsOptions> options,
        IHttpContextAccessor http,
        ILogger<Pkcs11CpsKeyStore> logger)
    {
        _options = options.Value;
        _http    = http;
        _logger  = logger;
        _resolvedLibraryPath = ResolveLibraryPath(_options.Pkcs11LibraryPath);
    }

    /// <summary>
    /// Indique si une bibliothèque PKCS#11 du middleware CPS est disponible sur la machine
    /// (override de config ou chemin auto-détecté par plateforme).
    /// </summary>
    public bool IsEnabled => _resolvedLibraryPath is not null;

    /// <summary>Certificat d'AUTHENTIFICATION (mTLS / ClientCertVerify). À NE PAS utiliser pour signer le VIHF.</summary>
    public X509Certificate2 GetAuthCertificate()
    {
        EnsureLibraryLoaded();
        return _authCertificate!;
    }

    /// <summary>Certificat de SIGNATURE (assertions VIHF, documents). À utiliser pour toute signature applicative.</summary>
    public X509Certificate2 GetSignatureCertificate()
    {
        EnsureLibraryLoaded();
        return _signatureCertificate!;
    }

    /// <summary>
    /// Signe un DigestInfo SHA-256 avec la clé privée d'AUTHENTIFICATION du token.
    /// Réservé au handshake mTLS (ClientCertVerify). N'utilise PAS cette méthode pour
    /// la signature applicative — le DMP rejette le VIHF signé avec la clé d'auth.
    /// </summary>
    public byte[] SignWithAuthKey(byte[] digestInfo)
    {
        EnsureLibraryLoaded();
        EnsureLoggedIn();
        using var mech = _factories.MechanismFactory.Create(CKM.CKM_RSA_PKCS);
        return _session!.Sign(mech, _authPrivateKey!, digestInfo);
    }

    /// <summary>
    /// Signe un DigestInfo SHA-256 avec la clé privée de SIGNATURE du token.
    /// À utiliser pour signer le VIHF et les documents — c'est ce que le DMP attend.
    /// </summary>
    public byte[] SignWithSignatureKey(byte[] digestInfo)
    {
        EnsureLibraryLoaded();
        EnsureLoggedIn();
        using var mech = _factories.MechanismFactory.Create(CKM.CKM_RSA_PKCS);
        return _session!.Sign(mech, _signaturePrivateKey!, digestInfo);
    }

    /// <summary>
    /// Lit l'objet de données PKCS#11 <c>CPS_INFO_PS</c> (public, sans PIN) et retourne
    /// le code spécialité ANS — convention CPS3 : 4 derniers caractères de la valeur UTF-8.
    /// Retourne <see cref="string.Empty"/> si PKCS#11 n'est pas disponible ou si l'objet est absent.
    /// </summary>
    public string ReadSpecialityCode()
    {
        if (!IsEnabled) return string.Empty;
        try
        {
            EnsureLibraryLoaded();
            var attrs = new List<IObjectAttribute>
            {
                _factories.ObjectAttributeFactory.Create(CKA.CKA_CLASS,   CKO.CKO_DATA),
                _factories.ObjectAttributeFactory.Create(CKA.CKA_TOKEN,   true),
                _factories.ObjectAttributeFactory.Create(CKA.CKA_PRIVATE, false),
                _factories.ObjectAttributeFactory.Create(CKA.CKA_LABEL,   "CPS_INFO_PS"),
            };
            var objects = _session!.FindAllObjects(attrs);
            if (objects.Count == 0)
            {
                _logger.LogDebug("Objet PKCS#11 CPS_INFO_PS absent du token.");
                return string.Empty;
            }

            var valueAttr = _session.GetAttributeValue(objects[0], new List<CKA> { CKA.CKA_VALUE });
            var raw = Encoding.UTF8.GetString(valueAttr[0].GetValueAsByteArray());
            _logger.LogDebug("CPS_INFO_PS brut : {Raw}", raw);

            // Convention CPS3 : le code spécialité occupe les 4 derniers caractères.
            if (raw.Length < 4) return string.Empty;
            var code = raw[^4..];
            _logger.LogInformation("Code spécialité CPS lu depuis PKCS#11 : {Code}", code);
            return code;
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Lecture de CPS_INFO_PS échouée : {Msg}", ex.Message);
            return string.Empty;
        }
    }

    /// <summary>
    /// Lit les situations d'exercice du praticien depuis les objets <c>CPS_ACTIVITY_01_PS</c>
    /// à <c>CPS_ACTIVITY_15_PS</c> (objets <b>privés</b> — login requis). Chaque activité porte
    /// la structure d'exercice (raison sociale + <c>Struct_IdNat</c>) et le secteur d'activité.
    ///
    /// En authentification directe, le DMP impose que <c>Identifiant_Structure</c> du VIHF soit
    /// le <c>Struct_IdNat</c> lu sur la carte (SEL-MP-037 §VIHF, p.156) ; le mettre en config
    /// fait remonter « Structure introuvable ou Inactive ».
    ///
    /// Résultat mis en cache pour la durée de vie du process (la carte ne change pas à chaud).
    /// </summary>
    public IReadOnlyList<CpsActivity> ReadActivities()
    {
        if (!IsEnabled) return Array.Empty<CpsActivity>();
        if (_activities is not null) return _activities;

        EnsureLibraryLoaded();
        EnsureLoggedIn(); // CPS_ACTIVITY_xx_PS sont CKA_PRIVATE=true

        var list = new List<CpsActivity>();
        for (int i = 1; i <= 15; i++)
        {
            var label = $"CPS_ACTIVITY_{i:00}_PS";
            var objects = _session!.FindAllObjects(new List<IObjectAttribute>
            {
                _factories.ObjectAttributeFactory.Create(CKA.CKA_CLASS, CKO.CKO_DATA),
                _factories.ObjectAttributeFactory.Create(CKA.CKA_LABEL, label),
            });
            if (objects.Count == 0) continue;

            var raw = _session.GetAttributeValue(objects[0], new List<CKA> { CKA.CKA_VALUE })[0].GetValueAsByteArray();
            var parsed = ParseActivity(i, raw);
            if (parsed is not null)
            {
                list.Add(parsed);
                _logger.LogInformation(
                    "Exercice CPS {Index} : structure '{Name}' (Struct_IdNat={Id}), secteur {Sector}",
                    parsed.Index, parsed.StructureName, parsed.StructureId, parsed.SectorCode);
            }
        }

        _activities = list;
        return _activities;
    }

    /// <summary>
    /// Décode un objet <c>CPS_ACTIVITY_xx_PS</c> (BER-TLV). Conteneur <c>0xEE</c> englobant des
    /// TLV simples : <c>0x84</c> = raison sociale, <c>0x85</c> = Struct_IdNat, <c>0x86</c> = secteur.
    /// </summary>
    private CpsActivity? ParseActivity(int index, byte[] raw)
    {
        try
        {
            if (raw.Length < 2 || raw[0] != 0xEE) return null;

            // Saute le tag conteneur 0xEE + son octet de longueur, parcourt les TLV internes.
            int pos = 2;
            string name = string.Empty, structId = string.Empty, sector = string.Empty;
            while (pos + 2 <= raw.Length)
            {
                byte tag = raw[pos++];
                int len  = raw[pos++];
                if (pos + len > raw.Length) break;
                var value = Encoding.UTF8.GetString(raw, pos, len).Trim();
                pos += len;

                switch (tag)
                {
                    case 0x84: name     = value; break;
                    case 0x85: structId = value; break;
                    case 0x86: sector   = value; break;
                }
            }

            if (string.IsNullOrEmpty(structId)) return null;
            return new CpsActivity(index, structId, name, sector);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Décodage de CPS_ACTIVITY_{Index:00}_PS échoué : {Msg}", index, ex.Message);
            return null;
        }
    }

    // ── Init — phase 1 : librairie + session + identité publique (sans PIN) ──

    private void EnsureLibraryLoaded()
    {
        if (_libraryLoaded) return;
        lock (_initLock)
        {
            if (_libraryLoaded) return;
            LoadLibrary();
            _libraryLoaded = true;
        }
    }

    private void LoadLibrary()
    {
        if (_resolvedLibraryPath is null)
            throw new DmpAuthException(
                "Aucune librairie PKCS#11 CPS détectée. " +
                "Installe le middleware CPS de l'ANS, ou renseigne Cps:Pkcs11LibraryPath pour forcer un chemin.");

        _logger.LogInformation("Chargement de la librairie PKCS#11 : {Path}", _resolvedLibraryPath);
        _library = _factories.Pkcs11LibraryFactory.LoadPkcs11Library(
            _factories, _resolvedLibraryPath, AppType.MultiThreaded);

        var slots = _library.GetSlotList(SlotsType.WithTokenPresent);
        if (slots.Count == 0)
            throw new DmpAuthException("Aucun token CPS détecté via PKCS#11 (carte non insérée ?).");

        _slot = slots[0];
        var token = _slot.GetTokenInfo();
        _logger.LogInformation("Token CPS détecté : {Label} (serial {Serial})", token.Label, token.SerialNumber);

        _session = _slot.OpenSession(SessionType.ReadOnly);

        var (authCert, authId) = FindCertificateByCkaIdSuffix(AuthCkaIdLastByte, "authentification");
        _authCertificate       = authCert;
        _authCkaId             = authId;

        var (signCert, signId) = FindCertificateByCkaIdSuffix(SignatureCkaIdLastByte, "signature");
        _signatureCertificate  = signCert;
        _signatureCkaId        = signId;

        _logger.LogInformation(
            "PKCS#11 prêt (session publique) — cert d'auth : {Auth}, cert de signature : {Sign}",
            authCert.Subject, signCert.Subject);
    }

    // ── Init — phase 2 : login (PIN requis, déclenché à la 1re signature) ───

    private void EnsureLoggedIn()
    {
        if (_loggedIn) return;
        lock (_loginLock)
        {
            if (_loggedIn) return;
            Login();
            _loggedIn = true;
        }
    }

    private void Login()
    {
        var tokenInfo        = _slot!.GetTokenInfo();
        var protectedAuthPath = tokenInfo.TokenFlags.ProtectedAuthenticationPath;

        try
        {
            if (protectedAuthPath)
            {
                // CKF_PROTECTED_AUTHENTICATION_PATH : le middleware CPS gère lui-même la saisie
                // du PIN (NSAlert macOS, dialog Windows...). On appelle C_Login(USER, NULL, 0).
                _logger.LogInformation(
                    "Login PKCS#11 via Protected Authentication Path — le middleware CPS va demander le PIN à l'utilisateur.");
                _session!.Login(CKU.CKU_USER, (byte[]?)null);
            }
            else
            {
                // Le middleware CPS ne sait pas afficher de dialog — le PIN doit être fourni
                // par le frontend via le header X-Cps-Pin. Fallback dev sur Cps:Pkcs11Pin.
                var pin = ResolvePinFromRequest() ?? _options.Pkcs11Pin;

                if (string.IsNullOrWhiteSpace(pin))
                {
                    _logger.LogInformation(
                        "Aucun PIN disponible pour la carte CPS (header {Header} absent) — réponse 401 attendue par le frontend.",
                        PinHeaderName);
                    throw new DmpPinRequiredException();
                }

                _session!.Login(CKU.CKU_USER, pin);
            }
        }
        catch (DmpException) { throw; }
        catch (Pkcs11Exception ex)
        {
            throw new DmpAuthException(
                $"Échec du login PKCS#11 sur la carte CPS ({ex.RV}). Code PIN incorrect ou saisie annulée ?", ex);
        }

        // Les clés privées sont des objets CKA_PRIVATE=true, donc invisibles avant le login.
        _authPrivateKey      = FindPrivateKeyByCkaId(_authCkaId!,      "authentification");
        _signaturePrivateKey = FindPrivateKeyByCkaId(_signatureCkaId!, "signature");
        _logger.LogInformation("Login PKCS#11 effectué — clés d'authentification et de signature disponibles.");
    }

    /// <summary>
    /// Lit le PIN dans le header HTTP <c>X-Cps-Pin</c> de la requête en cours, le cas échéant.
    /// Retourne <c>null</c> hors contexte HTTP ou quand le header est absent / vide.
    /// </summary>
    private string? ResolvePinFromRequest()
    {
        var headers = _http.HttpContext?.Request.Headers;
        if (headers is null) return null;
        var value = headers[PinHeaderName].ToString();
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    /// <summary>
    /// Phase publique : retrouve un certificat sur la CPS3 par le dernier octet de son <c>CKA_ID</c>
    /// (<c>0x20</c> pour la paire d'auth, <c>0x10</c> pour la paire de signature). Renvoie le cert
    /// ET son <c>CKA_ID</c> complet — nécessaire pour apparier la clé privée correspondante après login.
    /// </summary>
    private (X509Certificate2 cert, byte[] ckaId) FindCertificateByCkaIdSuffix(byte suffix, string usageLabel)
    {
        var certs = _session!.FindAllObjects(new List<IObjectAttribute>
        {
            _factories.ObjectAttributeFactory.Create(CKA.CKA_CLASS, CKO.CKO_CERTIFICATE)
        });

        if (certs.Count == 0)
            throw new DmpAuthException("Aucun certificat trouvé sur la carte via PKCS#11.");

        foreach (var c in certs)
        {
            var attrs = _session.GetAttributeValue(c, new List<CKA> { CKA.CKA_ID, CKA.CKA_LABEL });
            var id    = attrs[0].GetValueAsByteArray();
            var label = Encoding.UTF8.GetString(attrs[1].GetValueAsByteArray());
            if (id.Length > 0 && id[^1] == suffix)
            {
                _logger.LogDebug("Cert de {Usage} retenu : label='{Label}' CKA_ID={Hex}",
                    usageLabel, label, Convert.ToHexString(id));
                var certAttrs = _session.GetAttributeValue(c, new List<CKA> { CKA.CKA_VALUE });
                var cert      = new X509Certificate2(certAttrs[0].GetValueAsByteArray());
                return (cert, id);
            }
        }

        throw new DmpAuthException(
            $"Aucun certificat de {usageLabel} trouvé sur la carte CPS (CKA_ID terminant par 0x{suffix:X2}).");
    }

    /// <summary>
    /// Phase post-login : retrouve une clé privée en appariant les <c>CKA_ID</c>.
    /// Doit être appelée APRÈS <c>C_Login</c> car les objets <c>CKO_PRIVATE_KEY</c> portent
    /// <c>CKA_PRIVATE=true</c> et restent invisibles tant que le PIN n'a pas été présenté.
    /// </summary>
    private IObjectHandle FindPrivateKeyByCkaId(byte[] ckaId, string usageLabel)
    {
        var privKeys = _session!.FindAllObjects(new List<IObjectAttribute>
        {
            _factories.ObjectAttributeFactory.Create(CKA.CKA_CLASS, CKO.CKO_PRIVATE_KEY)
        });

        if (privKeys.Count == 0)
            throw new DmpAuthException(
                "Aucune clé privée visible sur la carte CPS via PKCS#11 (login effectué mais session sans accès).");

        foreach (var k in privKeys)
        {
            var attrs = _session.GetAttributeValue(k, new List<CKA> { CKA.CKA_ID });
            if (attrs[0].GetValueAsByteArray().AsSpan().SequenceEqual(ckaId))
                return k;
        }

        throw new DmpAuthException(
            $"Clé privée de {usageLabel} introuvable (CKA_ID={Convert.ToHexString(ckaId)}).");
    }

    // ── Auto-détection du chemin de la librairie ─────────────────────────────

    private string? ResolveLibraryPath(string? configuredPath)
    {
        if (!string.IsNullOrWhiteSpace(configuredPath))
        {
            if (File.Exists(configuredPath))
            {
                _logger.LogDebug("Librairie PKCS#11 forcée par config : {Path}", configuredPath);
                return configuredPath;
            }
            _logger.LogWarning(
                "Cps:Pkcs11LibraryPath = « {Path} » introuvable — fallback sur l'auto-détection.", configuredPath);
        }

        var candidates =
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX)     ? MacOsLibraryPaths   :
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? WindowsLibraryPaths :
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux)   ? LinuxLibraryPaths   :
            Array.Empty<string>();

        foreach (var path in candidates)
        {
            if (File.Exists(path))
            {
                _logger.LogInformation("Librairie PKCS#11 CPS auto-détectée : {Path}", path);
                return path;
            }
        }

        _logger.LogInformation("Aucune librairie PKCS#11 CPS détectée sur cette machine.");
        return null;
    }

    public void Dispose()
    {
        try { if (_loggedIn) _session?.Logout(); } catch { }
        _session?.Dispose();
        _library?.Dispose();
        _authCertificate?.Dispose();
        _signatureCertificate?.Dispose();
        _logger.LogDebug("Session PKCS#11 fermée.");
    }
}
