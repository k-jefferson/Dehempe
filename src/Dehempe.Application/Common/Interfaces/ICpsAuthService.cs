using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Dehempe.Application.Common.Interfaces;

/// <summary>
/// Fournit l'accès au certificat CPS et à sa clé privée (réelle ou déléguée via token).
/// </summary>
public interface ICpsAuthService
{
    /// <summary>Retourne le certificat X.509 du praticien.</summary>
    Task<X509Certificate2> GetCertificateAsync(CancellationToken ct = default);

    /// <summary>
    /// Retourne une instance <see cref="RSA"/> utilisable pour signer.
    /// Sur Windows / .p12 : la clé privée embarquée dans le cert.
    /// Sur macOS avec CTK : un adaptateur qui délègue à <c>SecKeyCreateSignature</c>
    /// sans exposer la clé privée (qui reste verrouillée dans le token).
    /// </summary>
    Task<RSA> GetSigningKeyAsync(CancellationToken ct = default);

    /// <summary>Signe des données arbitraires avec la clé privée de la CPS (SHA-256 / PKCS#1).</summary>
    Task<byte[]> SignAsync(byte[] data, CancellationToken ct = default);
}
