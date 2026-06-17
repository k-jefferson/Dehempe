using System.Security.Cryptography.X509Certificates;
using System.Text;
using Dehempe.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Net.Pkcs11Interop.Common;
using Net.Pkcs11Interop.HighLevelAPI;

namespace Dehempe.Infrastructure.Dmp.Auth.Pkcs11;

/// <summary>
/// Charge la librairie PKCS#11 du middleware CPS, ouvre une session, se logue avec le PIN
/// et expose le couple (certificat d'authentification, clé privée associée) prêt à signer.
///
/// Singleton dans le DI : la session reste ouverte pour toute la vie du process.
/// La clé privée ne quitte jamais le token ; les opérations de signature sont déléguées
/// au token via <c>C_Sign</c> (mécanisme <c>CKM_RSA_PKCS</c> avec <c>DigestInfo</c> SHA-256
/// en entrée — c'est exactement ce que <see cref="System.Security.Cryptography.RSA.SignHash"/>
/// est censé produire en sortie).
/// </summary>
internal sealed class Pkcs11CpsKeyStore : IDisposable
{
    /// <summary>
    /// Suffixe de <c>CKA_ID</c> identifiant la clé d'AUTHENTIFICATION sur une carte CPS3
    /// (convention IAS-ECC : <c>…1020</c> = AUT, <c>…1010</c> = SIG).
    /// </summary>
    private const byte AuthCkaIdLastByte    = 0x20;

    private readonly Pkcs11InteropFactories _factories = new();
    private readonly CpsOptions             _options;
    private readonly ILogger<Pkcs11CpsKeyStore> _logger;

    private IPkcs11Library?  _library;
    private ISlot?           _slot;
    private ISession?        _session;
    private IObjectHandle?   _authPrivateKey;
    private X509Certificate2? _authCertificate;
    private readonly object   _initLock = new();
    private bool              _initialized;

    public Pkcs11CpsKeyStore(IOptions<CpsOptions> options, ILogger<Pkcs11CpsKeyStore> logger)
    {
        _options = options.Value;
        _logger  = logger;
    }

    /// <summary>Indique si une bibliothèque PKCS#11 est configurée.</summary>
    public bool IsEnabled => !string.IsNullOrWhiteSpace(_options.Pkcs11LibraryPath);

    public X509Certificate2 GetAuthCertificate()
    {
        EnsureInitialized();
        return _authCertificate!;
    }

    /// <summary>
    /// Signe un DigestInfo SHA-256 (préfixe ASN.1 + 32 octets de hash) avec la clé privée
    /// d'authentification du token. <c>data</c> doit déjà contenir le DigestInfo complet.
    /// </summary>
    public byte[] SignWithAuthKey(byte[] digestInfo)
    {
        EnsureInitialized();
        using var mech = _factories.MechanismFactory.Create(CKM.CKM_RSA_PKCS);
        return _session!.Sign(mech, _authPrivateKey!, digestInfo);
    }

    // ── Init ─────────────────────────────────────────────────────────────────

    private void EnsureInitialized()
    {
        if (_initialized) return;
        lock (_initLock)
        {
            if (_initialized) return;
            Initialize();
            _initialized = true;
        }
    }

    private void Initialize()
    {
        if (!IsEnabled)
            throw new DmpAuthException("Cps:Pkcs11LibraryPath n'est pas configuré — PKCS#11 désactivé.");

        if (string.IsNullOrWhiteSpace(_options.Pkcs11Pin))
            throw new DmpAuthException("Cps:Pkcs11Pin doit être renseigné pour déverrouiller la carte CPS.");

        _logger.LogInformation("Chargement de la librairie PKCS#11 : {Path}", _options.Pkcs11LibraryPath);
        _library = _factories.Pkcs11LibraryFactory.LoadPkcs11Library(
            _factories, _options.Pkcs11LibraryPath!, AppType.MultiThreaded);

        var slots = _library.GetSlotList(SlotsType.WithTokenPresent);
        if (slots.Count == 0)
            throw new DmpAuthException("Aucun token CPS détecté via PKCS#11 (carte non insérée ?).");

        _slot = slots[0];
        var token = _slot.GetTokenInfo();
        _logger.LogInformation("Token CPS détecté : {Label} (serial {Serial})", token.Label, token.SerialNumber);

        _session = _slot.OpenSession(SessionType.ReadOnly);
        try
        {
            _session.Login(CKU.CKU_USER, _options.Pkcs11Pin!);
        }
        catch (Pkcs11Exception ex)
        {
            throw new DmpAuthException(
                $"Échec du login PKCS#11 sur la carte CPS ({ex.RV}). Code PIN incorrect ?", ex);
        }

        var (cert, privKey) = FindAuthIdentity();
        _authCertificate = cert;
        _authPrivateKey  = privKey;

        _logger.LogInformation("PKCS#11 prêt — cert d'authentification : {Subject}", cert.Subject);
    }

    /// <summary>
    /// Sur la carte CPS3, deux paires (cert, clé) coexistent. La paire d'authentification
    /// est repérée par un <c>CKA_ID</c> dont le dernier octet vaut <c>0x20</c> (la paire de
    /// signature porte <c>0x10</c>). On apparie cert et clé par <c>CKA_ID</c> identique.
    /// </summary>
    private (X509Certificate2 cert, IObjectHandle privKey) FindAuthIdentity()
    {
        var certs = _session!.FindAllObjects(new List<IObjectAttribute>
        {
            _factories.ObjectAttributeFactory.Create(CKA.CKA_CLASS, CKO.CKO_CERTIFICATE)
        });

        var privKeys = _session.FindAllObjects(new List<IObjectAttribute>
        {
            _factories.ObjectAttributeFactory.Create(CKA.CKA_CLASS, CKO.CKO_PRIVATE_KEY)
        });

        if (certs.Count == 0)
            throw new DmpAuthException("Aucun certificat trouvé sur la carte via PKCS#11.");
        if (privKeys.Count == 0)
            throw new DmpAuthException("Aucune clé privée trouvée sur la carte via PKCS#11.");

        IObjectHandle? authCertHandle = null;
        byte[]?        authCkaId      = null;

        foreach (var c in certs)
        {
            var attrs = _session.GetAttributeValue(c, new List<CKA> { CKA.CKA_ID, CKA.CKA_LABEL });
            var id    = attrs[0].GetValueAsByteArray();
            var label = Encoding.UTF8.GetString(attrs[1].GetValueAsByteArray());
            if (id.Length > 0 && id[^1] == AuthCkaIdLastByte)
            {
                authCertHandle = c;
                authCkaId      = id;
                _logger.LogDebug("Cert d'auth retenu : label='{Label}' CKA_ID={Hex}",
                    label, Convert.ToHexString(id));
                break;
            }
        }

        if (authCertHandle is null || authCkaId is null)
            throw new DmpAuthException(
                "Aucun certificat d'authentification trouvé sur la carte CPS (CKA_ID terminant par 0x20).");

        var certAttrs = _session.GetAttributeValue(authCertHandle, new List<CKA> { CKA.CKA_VALUE });
        var cert      = new X509Certificate2(certAttrs[0].GetValueAsByteArray());

        IObjectHandle? privKeyHandle = null;
        foreach (var k in privKeys)
        {
            var attrs = _session.GetAttributeValue(k, new List<CKA> { CKA.CKA_ID });
            if (attrs[0].GetValueAsByteArray().AsSpan().SequenceEqual(authCkaId))
            {
                privKeyHandle = k;
                break;
            }
        }

        if (privKeyHandle is null)
            throw new DmpAuthException(
                $"Clé privée d'authentification introuvable (CKA_ID={Convert.ToHexString(authCkaId)}).");

        return (cert, privKeyHandle);
    }

    public void Dispose()
    {
        try { _session?.Logout(); } catch { }
        _session?.Dispose();
        _library?.Dispose();
        _authCertificate?.Dispose();
        _logger.LogDebug("Session PKCS#11 fermée.");
    }
}
