using System.Security.Cryptography.X509Certificates;

namespace Dehempe.Application.Common.Interfaces;

/// <summary>
/// Fournit l'accès au certificat CPS pour signer les requêtes SOAP.
/// </summary>
public interface ICpsAuthService
{
    /// <summary>
    /// Retourne le certificat X.509 de la CPS chargé depuis le magasin configuré.
    /// </summary>
    Task<X509Certificate2> GetCertificateAsync(CancellationToken ct = default);

    /// <summary>
    /// Signe des données avec la clé privée de la CPS.
    /// </summary>
    Task<byte[]> SignAsync(byte[] data, CancellationToken ct = default);
}
