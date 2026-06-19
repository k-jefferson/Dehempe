using Microsoft.Extensions.Logging;

namespace Dehempe.Infrastructure.Dmp.Soap;

/// <summary>
/// <see cref="DelegatingHandler"/> qui réécrit l'URI sortante vers un proxy mTLS local
/// (stunnel + PKCS#11) en préservant l'authorité d'origine dans le header <c>Host</c>.
///
/// Utilisé sur macOS dev où l'on ne peut pas attacher un cert client PKCS#11 au handshake
/// TLS directement (limitation <c>AppleCertificatePal.CopyWithPrivateKey</c>). stunnel
/// termine le mTLS côté carte via <c>engine_pkcs11</c> ; côté loopback, .NET parle en HTTP
/// simple. Le serveur DMP voit le bon <c>Host</c> et reçoit le cert client de la carte.
/// </summary>
internal sealed class DmpTunnelHandler : DelegatingHandler
{
    private readonly Uri _tunnel;
    private readonly StunnelManager _stunnel;
    private readonly ILogger<DmpTunnelHandler> _logger;

    public DmpTunnelHandler(string tunnelEndpoint, StunnelManager stunnel, ILogger<DmpTunnelHandler> logger)
    {
        _tunnel  = new Uri(tunnelEndpoint);
        _stunnel = stunnel;
        _logger  = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        // S'assure que stunnel écoute avant d'envoyer la requête — démarre le tunnel
        // automatiquement à la première utilisation si nécessaire (singleton, idempotent).
        await _stunnel.EnsureRunningAsync(ct);

        if (request.RequestUri is { } original && original.Host != _tunnel.Host)
        {
            var rewritten = new UriBuilder(original)
            {
                Scheme = _tunnel.Scheme,
                Host   = _tunnel.Host,
                Port   = _tunnel.Port,
            }.Uri;

            // Préserve l'autorité d'origine dans le Host header :
            // .NET aurait sinon mis "127.0.0.1:5443", ce que le DMP rejetterait.
            request.Headers.Host = original.Authority;
            request.RequestUri   = rewritten;

            _logger.LogDebug("Tunnel mTLS : {Original} → {Rewritten} (Host={Host})",
                original, rewritten, original.Authority);
        }

        return await base.SendAsync(request, ct);
    }
}
