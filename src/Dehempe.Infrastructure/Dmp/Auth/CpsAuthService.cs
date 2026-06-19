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

    private X509Certificate2? _authCert;
    private X509Certificate2? _signatureCert;
    private RSA? _authKey;
    private RSA? _signatureKey;
    private string? _ctkTokenId; // non-null si cert chargé depuis un token CTK macOS

    public CpsAuthService(
        IOptions<CpsOptions> options,
        Pkcs11CpsKeyStore pkcs11,
        ILogger<CpsAuthService> logger)
    {
        _options = options.Value;
        _pkcs11  = pkcs11;
        _logger  = logger;
    }

    public Task<X509Certificate2> GetAuthenticationCertificateAsync(CancellationToken ct = default)
    {
        EnsureLoaded();
        return Task.FromResult(_authCert!);
    }

    public Task<X509Certificate2> GetSignatureCertificateAsync(CancellationToken ct = default)
    {
        EnsureLoaded();
        return Task.FromResult(_signatureCert!);
    }

    public Task<RSA> GetAuthenticationKeyAsync(CancellationToken ct = default)
    {
        EnsureLoaded();
        return Task.FromResult(_authKey!);
    }

    public Task<RSA> GetSignatureKeyAsync(CancellationToken ct = default)
    {
        EnsureLoaded();
        return Task.FromResult(_signatureKey!);
    }

    // ─── Chargement (les deux paires en une fois) ─────────────────────────────

    private void EnsureLoaded()
    {
        if (_authCert is not null && _signatureCert is not null) return;

        // 0. PKCS#11 prioritaire — deux paires distinctes sur la carte (CKA_ID 0x20 / 0x10)
        if (_pkcs11.IsEnabled)
        {
            try
            {
                var (authCert, authKey)       = WrapPkcs11Pair(_pkcs11.GetAuthCertificate(),       _pkcs11.SignWithAuthKey,       "auth");
                var (signCert, signKey)       = WrapPkcs11Pair(_pkcs11.GetSignatureCertificate(),  _pkcs11.SignWithSignatureKey,  "signature");
                _authCert      = authCert;
                _authKey       = authKey;
                _signatureCert = signCert;
                _signatureKey  = signKey;
                _logger.LogInformation(
                    "Paires CPS chargées via PKCS#11 — auth : {Auth}, signature : {Sign}",
                    authCert.Subject, signCert.Subject);
                return;
            }
            catch (DmpAuthException) { throw; }
            catch (Exception ex)
            {
                throw new DmpAuthException($"Échec de l'initialisation PKCS#11 : {ex.Message}", ex);
            }
        }

        // 1. Fichier .p12 / 2. Thumbprint / 3. Carte (CTK macOS legacy)
        // Ces chemins ne portent qu'UNE paire — on l'utilise pour auth ET signature,
        // ce qui marche pour les certs combinés (.p12 dev) ou les fallbacks, mais le DMP
        // rejettera en prod si le cert n'a pas le bon KeyUsage. Pour CPS3 réelle, PKCS#11
        // est obligatoire pour avoir les deux paires distinctes.
        var fallbackCert = LoadFallbackCertificate();
        var fallbackKey  = ResolveKeyForFallback(fallbackCert);

        _authCert      = fallbackCert;
        _signatureCert = fallbackCert;
        _authKey       = fallbackKey;
        _signatureKey  = fallbackKey;
        _logger.LogWarning(
            "Fallback non-PKCS#11 : la même paire est utilisée pour mTLS et signature VIHF. " +
            "Sur une carte CPS3 réelle, configure le middleware PKCS#11 pour utiliser les deux paires distinctes.");
    }

    private static (X509Certificate2 cert, RSA key) WrapPkcs11Pair(
        X509Certificate2 rawCert, Func<byte[], byte[]> sign, string label)
    {
        var pub = rawCert.GetRSAPublicKey()
            ?? throw new DmpAuthException($"Le cert PKCS#11 ({label}) n'expose pas de clé publique RSA.");
        var rsa = new Pkcs11RsaKey(pub, sign, label);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // macOS : AppleCertificatePal.CopyWithPrivateKey appelle ExportRSAPrivateKey,
            // incompatible avec une clé token non-exportable. On garde clé et cert séparés —
            // SignedXml accepte une RSA explicite via SigningKey, et le mTLS macOS passe par stunnel.
            return (rawCert, rsa);
        }

        // Windows / Linux : CopyWithPrivateKey conserve une référence à pkcs11Rsa sans extraction.
        var bound = rawCert.CopyWithPrivateKey(rsa);
        return (bound, rsa);
    }

    private RSA ResolveKeyForFallback(X509Certificate2 cert)
    {
        // .p12 / store : la clé privée est portée par le cert lui-même.
        var native = cert.GetRSAPrivateKey();
        if (native is not null) return native;

        // CTK macOS legacy : signature déléguée à SecKeyCreateSignature via le tokenId.
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
            "Configure PKCS#11 (Cps:Pkcs11LibraryPath), un .p12 (Cps:CertificatePath) " +
            "ou utilise un certificat depuis une carte CPS branchée.");
    }

    // ─── Fallbacks non-PKCS#11 ────────────────────────────────────────────────

    private X509Certificate2 LoadFallbackCertificate()
    {
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

        var chosen = candidates
            .Where(x => HasOid(x.Cert, "2.5.4.4") && HasOid(x.Cert, "2.5.4.42"))
            .OrderByDescending(x => x.Cert.NotAfter)
            .FirstOrDefault();

        if (chosen.Cert is null) chosen = candidates[0];

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
