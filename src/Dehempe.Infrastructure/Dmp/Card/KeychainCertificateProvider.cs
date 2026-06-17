using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;

namespace Dehempe.Infrastructure.Dmp.Card;

/// <summary>
/// Reads CPS personal certificates from the standard OS keychain
/// (CurrentUser\My on Windows). Works when the CPS middleware (eGate CSP/CNG
/// or PKCS#11 importer) publishes the smart card certificate into the standard
/// store — typical Windows behaviour.
/// </summary>
internal sealed class KeychainCertificateProvider : ICpsCertificateProvider
{
    private static readonly string[] CpsIssuerPatterns =
    {
        "ASIPSANTE", "ASIP-SANTE", "ASIP SANTE",
        "IGC-SANTE", "IGC SANTE",
        "AC RACINE CPS", "CPS SOUS-AUTORITE",
        "ANS-"
    };

    private readonly ILogger _logger;

    public KeychainCertificateProvider(ILogger logger) => _logger = logger;

    public IReadOnlyList<(X509Certificate2 Cert, string CardId)> FindCpsCertificates()
    {
        using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
        store.Open(OpenFlags.ReadOnly);

        var candidates = store.Certificates
            .OfType<X509Certificate2>()
            .Where(IsCpsCertificate)
            .Where(c => !IsCaCertificate(c))
            .ToList();

        _logger.LogInformation("{Count} certificat(s) CPS dans CurrentUser\\My.", candidates.Count);

        // On Windows we don't get a hardware card serial through this channel.
        // The X.509 serial number is unique per card-issued cert and is good enough.
        return candidates
            .Select(c => (c, FormatSerialAsDecimal(c.SerialNumber)))
            .ToList();
    }

    private static bool IsCaCertificate(X509Certificate2 cert)
    {
        if (cert.Extensions["2.5.29.19"] is X509BasicConstraintsExtension bc && bc.CertificateAuthority)
            return true;
        return cert.Subject.Equals(cert.Issuer, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsCpsCertificate(X509Certificate2 cert)
    {
        var issuer = cert.Issuer.ToUpperInvariant();
        return CpsIssuerPatterns.Any(p => issuer.Contains(p, StringComparison.OrdinalIgnoreCase));
    }

    private static string FormatSerialAsDecimal(string hexSerial)
    {
        if (string.IsNullOrEmpty(hexSerial)) return "";
        try
        {
            var dec = System.Numerics.BigInteger.Parse("0" + hexSerial,
                System.Globalization.NumberStyles.HexNumber);
            return dec.ToString();
        }
        catch { return hexSerial; }
    }
}
