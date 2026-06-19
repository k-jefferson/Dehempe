using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dehempe.Infrastructure.Dmp.Soap;

/// <summary>
/// Vérifie que le tunnel mTLS local (stunnel + PKCS#11) écoute, et le démarre
/// automatiquement via le script <c>docs/stunnel/start-tunnel.sh</c> sinon.
///
/// Singleton DI : la première requête XDS sortante déclenche le check ; les suivantes
/// court-circuitent dès que le tunnel est marqué actif. Si stunnel meurt en cours de
/// route, le prochain `Connection refused` côté HttpClient ne déclenchera PAS de
/// redémarrage automatique (le manager ne ré-évalue qu'une fois) — c'est volontaire,
/// pour éviter un cycle infini lance/crash si la conf stunnel est cassée. À ce moment-là
/// remettre le port à zéro nécessitera un redémarrage de l'API ou un appel manuel à
/// <see cref="Invalidate"/> via un endpoint admin (non exposé).
/// </summary>
internal sealed class StunnelManager
{
    private readonly Uri _endpoint;
    private readonly string? _startScript;
    private readonly ILogger<StunnelManager> _logger;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private bool _verified;

    public StunnelManager(IOptions<DmpOptions> options, ILogger<StunnelManager> logger)
    {
        var opts = options.Value;
        if (string.IsNullOrWhiteSpace(opts.TunnelEndpoint))
            throw new InvalidOperationException(
                "StunnelManager instancié alors que Dmp:TunnelEndpoint est vide — vérifie le wiring DI.");

        _endpoint    = new Uri(opts.TunnelEndpoint);
        _startScript = opts.StunnelStartScript ?? AutoDetectStartScript();
        _logger      = logger;
    }

    /// <summary>
    /// Garantit que le tunnel écoute sur <c>Dmp:TunnelEndpoint</c>. Bloque la première
    /// fois (check TCP + démarrage si nécessaire), puis devient un no-op. Idempotent
    /// sous concurrence (sérialisé par <see cref="SemaphoreSlim"/>).
    /// </summary>
    public async Task EnsureRunningAsync(CancellationToken ct = default)
    {
        if (_verified) return;

        await _gate.WaitAsync(ct);
        try
        {
            if (_verified) return;

            if (await IsListeningAsync(ct))
            {
                _logger.LogDebug("Tunnel stunnel déjà actif sur {Endpoint}.", _endpoint);
                _verified = true;
                return;
            }

            if (string.IsNullOrEmpty(_startScript) || !File.Exists(_startScript))
                throw new InvalidOperationException(
                    $"Le tunnel stunnel n'écoute pas sur {_endpoint} et aucun script de démarrage n'est " +
                    "disponible (Dmp:StunnelStartScript vide / fichier introuvable). " +
                    "Lance-le manuellement : docs/stunnel/start-tunnel.sh");

            _logger.LogInformation(
                "Tunnel stunnel inactif sur {Endpoint} — démarrage via {Script}.", _endpoint, _startScript);

            await LaunchStartScriptAsync(ct);
            await WaitForListenAsync(TimeSpan.FromSeconds(15), ct);

            _verified = true;
            _logger.LogInformation("Tunnel stunnel actif sur {Endpoint}.", _endpoint);
        }
        finally { _gate.Release(); }
    }

    /// <summary>Force la re-vérification au prochain appel (utile si le tunnel meurt).</summary>
    public void Invalidate() => _verified = false;

    // ── Internals ────────────────────────────────────────────────────────────

    private async Task<bool> IsListeningAsync(CancellationToken ct)
    {
        try
        {
            using var client = new TcpClient();
            using var cts    = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromMilliseconds(500));
            await client.ConnectAsync(_endpoint.Host, _endpoint.Port, cts.Token);
            return client.Connected;
        }
        catch
        {
            return false;
        }
    }

    private async Task WaitForListenAsync(TimeSpan timeout, CancellationToken ct)
    {
        var deadline = DateTime.UtcNow + timeout;
        while (DateTime.UtcNow < deadline)
        {
            if (await IsListeningAsync(ct)) return;
            await Task.Delay(TimeSpan.FromMilliseconds(500), ct);
        }
        throw new TimeoutException(
            $"Le tunnel stunnel n'a pas ouvert le port {_endpoint} dans le délai imparti ({timeout.TotalSeconds:0}s). " +
            "Vérifie /tmp/dehempe-stunnel.log.");
    }

    /// <summary>
    /// Lance <c>bash start-tunnel.sh</c> en sous-process. Le script gère lui-même le
    /// <c>nohup</c> / <c>disown</c> pour que stunnel survive à l'arrêt du process .NET.
    /// On attend la fin du script (≤5s normalement) pour récupérer son code de sortie.
    /// </summary>
    private async Task LaunchStartScriptAsync(CancellationToken ct)
    {
        var psi = new ProcessStartInfo("/bin/bash", $"\"{_startScript}\"")
        {
            RedirectStandardOutput = true,
            RedirectStandardError  = true,
            UseShellExecute        = false,
            CreateNoWindow         = true,
        };

        using var proc = Process.Start(psi)
            ?? throw new InvalidOperationException($"Impossible de lancer {_startScript}.");

        try
        {
            await proc.WaitForExitAsync(ct);
        }
        catch (OperationCanceledException)
        {
            try { proc.Kill(entireProcessTree: true); } catch { /* best-effort */ }
            throw;
        }

        if (proc.ExitCode != 0)
        {
            var stderr = await proc.StandardError.ReadToEndAsync(ct);
            var stdout = await proc.StandardOutput.ReadToEndAsync(ct);
            throw new InvalidOperationException(
                $"start-tunnel.sh a échoué (code {proc.ExitCode}). stdout: {stdout.Trim()} / stderr: {stderr.Trim()}");
        }
    }

    /// <summary>
    /// Cherche <c>docs/stunnel/start-tunnel.sh</c> en remontant depuis le binaire courant.
    /// </summary>
    private static string? AutoDetectStartScript()
    {
        var dir = AppContext.BaseDirectory;
        for (int i = 0; i < 10 && !string.IsNullOrEmpty(dir); i++)
        {
            var candidate = Path.Combine(dir, "docs", "stunnel", "start-tunnel.sh");
            if (File.Exists(candidate)) return candidate;
            dir = Path.GetDirectoryName(dir);
        }
        return null;
    }
}
