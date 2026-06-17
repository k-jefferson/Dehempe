using System.Security.Cryptography.X509Certificates;

namespace Dehempe.Infrastructure.Dmp.Card;

/// <summary>
/// Returns CPS personal certificates available on this machine.
/// Implementations are platform-specific (Windows/macOS/Linux) because the way
/// smart cards expose their certificates to the OS differs significantly.
/// </summary>
internal interface ICpsCertificateProvider
{
    /// <param name="CardId">A stable, card-specific identifier (e.g. CTK token serial on macOS, X.509 serial on Windows).</param>
    IReadOnlyList<(X509Certificate2 Cert, string CardId)> FindCpsCertificates();
}
