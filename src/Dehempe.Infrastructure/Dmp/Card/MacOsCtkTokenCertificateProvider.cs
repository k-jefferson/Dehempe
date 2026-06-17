using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;

namespace Dehempe.Infrastructure.Dmp.Card;

/// <summary>
/// macOS-specific certificate provider. The CPS smart card on macOS is owned
/// by the CryptoTokenKit framework's <c>fr.asip.esante.CPSToken</c> extension,
/// which keeps its certificates inside a virtual keychain invisible to
/// <see cref="X509Store"/>. To reach them we shell out to the Swift runtime
/// (always present on macOS dev machines), let it call <c>SecItemCopyMatching</c>
/// with the right <c>kSecAttrTokenID</c>, and read back DER bytes via stdout.
/// </summary>
internal sealed class MacOsCtkTokenCertificateProvider : ICpsCertificateProvider
{
    private const string CpsTokenPrefix = "fr.asip.esante.CPSToken:";

    private readonly ILogger _logger;

    public MacOsCtkTokenCertificateProvider(ILogger logger) => _logger = logger;

    public IReadOnlyList<(X509Certificate2 Cert, string CardId)> FindCpsCertificates()
    {
        var tokenIds = ListCpsTokenIds();
        if (tokenIds.Count == 0)
        {
            _logger.LogWarning("Aucun token CryptoTokenKit CPS détecté (préfixe « {Prefix} »).", CpsTokenPrefix);
            return Array.Empty<(X509Certificate2, string)>();
        }

        var result = new List<(X509Certificate2, string)>();
        foreach (var tokenId in tokenIds)
        {
            var cardId = tokenId[CpsTokenPrefix.Length..];
            foreach (var cert in QueryCertsByTokenId(tokenId))
                result.Add((cert, cardId));
        }
        return result;
    }

    // ── Step 1: list CTK token IDs ────────────────────────────────────────

    private List<string> ListCpsTokenIds()
    {
        var psi = new ProcessStartInfo
        {
            FileName               = "security",
            Arguments              = "list-smartcards",
            RedirectStandardOutput = true,
            RedirectStandardError  = true,
            UseShellExecute        = false,
        };

        using var p = Process.Start(psi)
            ?? throw new InvalidOperationException("Failed to start `security list-smartcards`.");

        var stdout = p.StandardOutput.ReadToEnd();
        p.WaitForExit(5_000);

        return stdout
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.Trim())
            .Where(l => l.StartsWith(CpsTokenPrefix, StringComparison.Ordinal))
            .ToList();
    }

    // ── Step 2: query certs for a given token via Swift one-liner ─────────

    private IEnumerable<X509Certificate2> QueryCertsByTokenId(string tokenId)
    {
        var script = $$"""
            import Foundation
            import Security
            let q: [CFString: Any] = [
                kSecClass: kSecClassCertificate,
                kSecMatchLimit: kSecMatchLimitAll,
                kSecReturnRef: true,
                kSecAttrTokenID: "{{tokenId}}"
            ]
            var r: AnyObject?
            guard SecItemCopyMatching(q as CFDictionary, &r) == errSecSuccess,
                  let arr = r as? [SecCertificate] else { exit(0) }
            for c in arr {
                if let d = SecCertificateCopyData(c) as Data? {
                    print(d.base64EncodedString())
                }
            }
            """;

        var psi = new ProcessStartInfo
        {
            FileName               = "swift",
            Arguments              = "-",
            RedirectStandardInput  = true,
            RedirectStandardOutput = true,
            RedirectStandardError  = true,
            UseShellExecute        = false,
        };

        using var p = Process.Start(psi)
            ?? throw new InvalidOperationException("Failed to start `swift`.");

        p.StandardInput.Write(script);
        p.StandardInput.Close();

        var stdout = p.StandardOutput.ReadToEnd();
        var stderr = p.StandardError.ReadToEnd();
        p.WaitForExit(15_000);

        if (p.ExitCode != 0)
        {
            _logger.LogWarning("swift query failed for {Token}: {Err}", tokenId, stderr);
            yield break;
        }

        foreach (var line in stdout.Split('\n', StringSplitOptions.RemoveEmptyEntries))
        {
            X509Certificate2? cert = null;
            try { cert = new X509Certificate2(Convert.FromBase64String(line.Trim())); }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Skipping invalid base64 cert line.");
            }
            if (cert is not null) yield return cert;
        }
    }
}
