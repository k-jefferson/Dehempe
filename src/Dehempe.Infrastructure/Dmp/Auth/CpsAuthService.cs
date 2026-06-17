using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Dehempe.Application.Common.Interfaces;
using Dehempe.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dehempe.Infrastructure.Dmp.Auth;

internal sealed class CpsAuthService : ICpsAuthService
{
    private readonly CpsOptions _options;
    private readonly ILogger<CpsAuthService> _logger;
    private X509Certificate2? _cached;

    public CpsAuthService(IOptions<CpsOptions> options, ILogger<CpsAuthService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task<X509Certificate2> GetCertificateAsync(CancellationToken ct = default)
    {
        if (_cached is not null)
            return Task.FromResult(_cached);

        _cached = LoadCertificate();
        return Task.FromResult(_cached);
    }

    public async Task<byte[]> SignAsync(byte[] data, CancellationToken ct = default)
    {
        var cert = await GetCertificateAsync(ct);
        using var rsa = cert.GetRSAPrivateKey()
            ?? throw new DmpAuthException("Le certificat CPS ne contient pas de clé privée RSA.");

        return rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }

    private X509Certificate2 LoadCertificate()
    {
        if (!string.IsNullOrWhiteSpace(_options.CertificatePath))
        {
            _logger.LogInformation("Chargement du certificat CPS depuis fichier : {Path}", _options.CertificatePath);
            return new X509Certificate2(
                _options.CertificatePath,
                _options.CertificatePassword,
                X509KeyStorageFlags.Exportable);
        }

        if (!string.IsNullOrWhiteSpace(_options.CertificateThumbprint))
        {
            var location = Enum.Parse<StoreLocation>(_options.StoreLocation, ignoreCase: true);
            using var store = new X509Store(StoreName.My, location);
            store.Open(OpenFlags.ReadOnly);

            var certs = store.Certificates.Find(
                X509FindType.FindByThumbprint,
                _options.CertificateThumbprint,
                validOnly: false);

            if (certs.Count == 0)
                throw new DmpAuthException(
                    $"Certificat CPS introuvable dans le magasin {location} avec l'empreinte : {_options.CertificateThumbprint}");

            _logger.LogInformation("Certificat CPS chargé depuis le magasin système.");
            return certs[0];
        }

        throw new DmpAuthException("Aucune source de certificat CPS configurée (CertificatePath ou CertificateThumbprint).");
    }
}
