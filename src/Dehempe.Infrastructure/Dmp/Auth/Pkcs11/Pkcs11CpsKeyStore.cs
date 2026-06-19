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
    /// Suffixe de <c>CKA_ID</c> identifiant la clé d'AUTHENTIFICATION sur une carte CPS3
    /// (convention IAS-ECC : <c>…1020</c> = AUT, <c>…1010</c> = SIG).
    /// </summary>
    private const byte AuthCkaIdLastByte = 0x20;

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
    private X509Certificate2? _authCertificate;
    private byte[]?           _authCkaId;       // CKA_ID du cert d'auth, utilisé pour apparier la clé privée après login
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

    public X509Certificate2 GetAuthCertificate()
    {
        EnsureLibraryLoaded();
        return _authCertificate!;
    }

    /// <summary>
    /// Signe un DigestInfo SHA-256 (préfixe ASN.1 + 32 octets de hash) avec la clé privée
    /// d'authentification du token. Déclenche le login PKCS#11 à la première signature ;
    /// le PIN est fourni par <see cref="ICpsPinProvider"/> (UI / tunnel mTLS), pas par la config.
    /// </summary>
    public byte[] SignWithAuthKey(byte[] digestInfo)
    {
        EnsureLibraryLoaded();
        EnsureLoggedIn();
        using var mech = _factories.MechanismFactory.Create(CKM.CKM_RSA_PKCS);
        return _session!.Sign(mech, _authPrivateKey!, digestInfo);
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

        var (cert, ckaId) = FindAuthCertificate();
        _authCertificate  = cert;
        _authCkaId        = ckaId;

        _logger.LogInformation("PKCS#11 prêt (session publique) — cert d'authentification : {Subject}", cert.Subject);
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
        _authPrivateKey = FindAuthPrivateKey(_authCkaId!);
        _logger.LogInformation("Login PKCS#11 effectué — signature avec la clé d'authentification disponible.");
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
    /// Phase publique : retrouve le certificat d'authentification de la CPS3.
    /// Deux paires (cert, clé) coexistent sur la carte ; la paire d'auth est repérée
    /// par un <c>CKA_ID</c> dont le dernier octet vaut <c>0x20</c> (la signature porte <c>0x10</c>).
    /// On retourne aussi le <c>CKA_ID</c> pour pouvoir apparier la clé privée après login.
    /// </summary>
    private (X509Certificate2 cert, byte[] ckaId) FindAuthCertificate()
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
            if (id.Length > 0 && id[^1] == AuthCkaIdLastByte)
            {
                _logger.LogDebug("Cert d'auth retenu : label='{Label}' CKA_ID={Hex}",
                    label, Convert.ToHexString(id));
                var certAttrs = _session.GetAttributeValue(c, new List<CKA> { CKA.CKA_VALUE });
                var cert      = new X509Certificate2(certAttrs[0].GetValueAsByteArray());
                return (cert, id);
            }
        }

        throw new DmpAuthException(
            "Aucun certificat d'authentification trouvé sur la carte CPS (CKA_ID terminant par 0x20).");
    }

    /// <summary>
    /// Phase post-login : retrouve la clé privée d'auth en appariant les <c>CKA_ID</c>.
    /// Doit être appelée APRÈS <c>C_Login</c> car les objets <c>CKO_PRIVATE_KEY</c> portent
    /// <c>CKA_PRIVATE=true</c> et restent invisibles tant que le PIN n'a pas été présenté.
    /// </summary>
    private IObjectHandle FindAuthPrivateKey(byte[] authCkaId)
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
            if (attrs[0].GetValueAsByteArray().AsSpan().SequenceEqual(authCkaId))
                return k;
        }

        throw new DmpAuthException(
            $"Clé privée d'authentification introuvable (CKA_ID={Convert.ToHexString(authCkaId)}).");
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
        _logger.LogDebug("Session PKCS#11 fermée.");
    }
}
