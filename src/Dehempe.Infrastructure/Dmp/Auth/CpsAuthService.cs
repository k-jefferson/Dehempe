using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Dehempe.Application.Common.Interfaces;
using Dehempe.Domain.Exceptions;
using Dehempe.Infrastructure.Dmp.Auth.Pkcs11;
using Dehempe.Infrastructure.Dmp.Card;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dehempe.Infrastructure.Dmp.Auth;

internal sealed class CpsAuthService : ICpsAuthService
{
    private const string CtkTokenIdPrefix = "fr.asip.esante.CPSToken:";

    private readonly CpsOptions _options;
    private readonly Pkcs11CpsKeyStore _pkcs11;
    private readonly ILogger<CpsAuthService> _logger;

    private X509Certificate2? _cachedCert;
    private string?           _ctkTokenId;       // non-null si cert chargé depuis un token CTK macOS
    private Pkcs11RsaKey?     _pkcs11SigningKey; // signature détachée du cert sur macOS (ExportRSAPrivateKey impossible)

    public CpsAuthService(
        IOptions<CpsOptions> options,
        Pkcs11CpsKeyStore pkcs11,
        ILogger<CpsAuthService> logger)
    {
        _options = options.Value;
        _pkcs11  = pkcs11;
        _logger  = logger;
    }

    public Task<X509Certificate2> GetCertificateAsync(CancellationToken ct = default)
    {
        if (_cachedCert is not null) return Task.FromResult(_cachedCert);
        _cachedCert = LoadCertificate();
        return Task.FromResult(_cachedCert);
    }

    public async Task<RSA> GetSigningKeyAsync(CancellationToken ct = default)
    {
        var cert = await GetCertificateAsync(ct);

        // 1. macOS PKCS#11 : la clé est détachée du cert (Apple refuse CopyWithPrivateKey sans export)
        if (_pkcs11SigningKey is not null) return _pkcs11SigningKey;

        // 2. Cert lié à la clé via CopyWithPrivateKey (Windows/Linux PKCS#11, ou .p12)
        var native = cert.GetRSAPrivateKey();
        if (native is not null) return native;

        // 3. Fallback macOS CTK historique (Swift / SecKeyCreateSignature)
        if (_ctkTokenId is not null)
        {
            var pub = cert.GetRSAPublicKey()
                ?? throw new DmpAuthException("Le certificat CPS n'expose pas de clé publique RSA.");
            return new CtkBackedRsa(pub, new MacOsCtkTokenSigner(_logger),
                tokenId: _ctkTokenId,
                certThumbprint: cert.Thumbprint);
        }

        throw new DmpAuthException(
            "Aucune clé privée disponible pour signer avec ce certificat CPS. " +
            "Configure PKCS#11 (Cps:Pkcs11LibraryPath + Cps:Pkcs11Pin), un .p12 (Cps:CertificatePath) " +
            "ou utilise un certificat depuis une carte CPS branchée.");
    }

    public async Task<byte[]> SignAsync(byte[] data, CancellationToken ct = default)
    {
        using var rsa = await GetSigningKeyAsync(ct);
        return rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }

    // ─── Cert loading ─────────────────────────────────────────────────────────

    private X509Certificate2 LoadCertificate()
    {
        // 0. PKCS#11 prioritaire (binding propre carte → mTLS sans extraction de clé)
        if (_pkcs11.IsEnabled)
        {
            try
            {
                var rawCert = _pkcs11.GetAuthCertificate();
                var pub     = rawCert.GetRSAPublicKey()
                    ?? throw new DmpAuthException("Le cert d'auth PKCS#11 n'expose pas de clé publique RSA.");
                var pkcs11Rsa = new Pkcs11RsaKey(pub, _pkcs11);

                // macOS : AppleCertificatePal.CopyWithPrivateKey appelle ExportRSAPrivateKey,
                // ce qui est incompatible avec une clé non-exportable. On garde la clé séparée
                // pour la signature SAML ; le mTLS macOS reste à régler via stunnel/proxy.
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    _pkcs11SigningKey = pkcs11Rsa;
                    _logger.LogInformation(
                        "PKCS#11 sur macOS : signature VIHF disponible, mTLS NON attaché au handshake " +
                        "(limitation AppleCertificatePal — déployer sur Windows/Linux ou utiliser un proxy mTLS).");
                    return rawCert;
                }

                // Windows / Linux : CopyWithPrivateKey conserve une référence à pkcs11Rsa sans export.
                var bound = rawCert.CopyWithPrivateKey(pkcs11Rsa);
                _logger.LogInformation("Cert d'auth PKCS#11 lié à Pkcs11RsaKey (HasPrivateKey={H}).",
                    bound.HasPrivateKey);
                return bound;
            }
            catch (DmpAuthException) { throw; }
            catch (Exception ex)
            {
                throw new DmpAuthException(
                    $"Échec de l'initialisation PKCS#11 : {ex.Message}", ex);
            }
        }

        // 1. Fichier .p12 explicite
        if (!string.IsNullOrWhiteSpace(_options.CertificatePath))
        {
            if (!File.Exists(_options.CertificatePath))
                throw new DmpAuthException(
                    $"Le fichier de certificat CPS configuré n'existe pas : {_options.CertificatePath}. " +
                    "Renseigne un chemin valide dans Cps:CertificatePath, ou laisse vide pour utiliser la carte CPS branchée.");

            try
            {
                _logger.LogInformation("Chargement du certificat CPS depuis fichier : {Path}", _options.CertificatePath);
                return new X509Certificate2(
                    _options.CertificatePath,
                    _options.CertificatePassword,
                    X509KeyStorageFlags.Exportable);
            }
            catch (Exception ex)
            {
                throw new DmpAuthException(
                    $"Impossible de charger le certificat CPS depuis « {_options.CertificatePath} » : {ex.Message}", ex);
            }
        }

        // 2. Empreinte explicite dans le magasin
        if (!string.IsNullOrWhiteSpace(_options.CertificateThumbprint))
        {
            var location = Enum.Parse<StoreLocation>(_options.StoreLocation, ignoreCase: true);
            using var store = new X509Store(StoreName.My, location);
            store.Open(OpenFlags.ReadOnly);
            var certs = store.Certificates.Find(
                X509FindType.FindByThumbprint, _options.CertificateThumbprint, validOnly: false);

            if (certs.Count == 0)
                throw new DmpAuthException(
                    $"Certificat CPS introuvable dans le magasin {location} avec l'empreinte : {_options.CertificateThumbprint}");

            _logger.LogInformation("Certificat CPS chargé depuis le magasin système.");
            return certs[0];
        }

        // 3. Auto-détection : carte CPS branchée
        _logger.LogInformation("Aucun certificat configuré — auto-détection sur la carte CPS branchée.");
        return LoadFromCard();
    }

    private X509Certificate2 LoadFromCard()
    {
        ICpsCertificateProvider provider = RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
            ? new MacOsCtkTokenCertificateProvider(_logger)
            : new KeychainCertificateProvider(_logger);

        var candidates = provider.FindCpsCertificates();
        if (candidates.Count == 0)
            throw new DmpAuthException(
                "Aucun certificat CPS trouvé sur la machine (ni .p12, ni magasin, ni carte). " +
                "Vérifie que le middleware CPS est lancé et que la carte est insérée.");

        // CPS3 personnel = présence de SN (2.5.4.4) ET GN (2.5.4.42) dans le Subject
        var chosen = candidates
            .Where(x => HasOid(x.Cert, "2.5.4.4") && HasOid(x.Cert, "2.5.4.42"))
            .OrderByDescending(x => x.Cert.NotAfter)
            .FirstOrDefault();

        if (chosen.Cert is null) chosen = candidates[0];

        // Mémorise le tokenId pour la signature CTK ultérieure
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && chosen.CardId is { Length: > 0 })
            _ctkTokenId = CtkTokenIdPrefix + chosen.CardId;

        _logger.LogInformation("Certificat CPS auto-détecté : {Subject} (token={Token})",
            chosen.Cert.Subject, _ctkTokenId ?? "(local)");

        return chosen.Cert;
    }

    private static bool HasOid(X509Certificate2 cert, string oid)
    {
        try
        {
            var reader = new System.Formats.Asn1.AsnReader(
                cert.SubjectName.RawData, System.Formats.Asn1.AsnEncodingRules.DER);
            var seq = reader.ReadSequence();
            while (seq.HasData)
            {
                var set = seq.ReadSetOf();
                while (set.HasData)
                {
                    var ava = set.ReadSequence();
                    if (ava.ReadObjectIdentifier() == oid) return true;
                    ava.ReadEncodedValue();
                }
            }
            return false;
        }
        catch { return false; }
    }
}
