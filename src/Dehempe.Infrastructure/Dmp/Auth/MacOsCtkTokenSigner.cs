using System.Diagnostics;
using Dehempe.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Dehempe.Infrastructure.Dmp.Auth;

/// <summary>
/// Signe des hashes SHA-256 en déléguant à la clé privée verrouillée dans le
/// token CryptoTokenKit (CTK) de la carte CPS sur macOS.
///
/// Le binding se fait via un sous-processus <c>swift</c> qui appelle
/// <c>SecKeyCreateSignature</c> avec l'algorithme
/// <c>kSecKeyAlgorithmRSASignatureDigestPKCS1v15SHA256</c>. La clé privée
/// ne quitte jamais le token — seul le résultat de la signature transite.
/// </summary>
internal sealed class MacOsCtkTokenSigner
{
    private const string SwiftScript = """
        import Foundation
        import Security
        import CryptoKit

        let args = CommandLine.arguments
        guard args.count == 4 else { exit(1) }
        let tokenId    = args[1]
        let targetThumb = args[2].lowercased()
        let hashB64    = args[3]
        guard let hash = Data(base64Encoded: hashB64) else { exit(2) }

        let q: [CFString: Any] = [
            kSecClass: kSecClassIdentity,
            kSecMatchLimit: kSecMatchLimitAll,
            kSecReturnRef: true,
            kSecAttrTokenID: tokenId
        ]
        var r: AnyObject?
        let st = SecItemCopyMatching(q as CFDictionary, &r)
        guard st == errSecSuccess, let ids = r as? [SecIdentity] else {
            FileHandle.standardError.write("identity-lookup-failed:\(st)\n".data(using:.utf8)!)
            exit(3)
        }

        func sha1Hex(_ c: SecCertificate) -> String {
            let d = SecCertificateCopyData(c) as Data
            return Insecure.SHA1.hash(data: d).map { String(format: "%02x", $0) }.joined()
        }

        var signingKey: SecKey?
        for id in ids {
            var c: SecCertificate?
            SecIdentityCopyCertificate(id, &c)
            if let cert = c, sha1Hex(cert) == targetThumb {
                var k: SecKey?
                SecIdentityCopyPrivateKey(id, &k)
                signingKey = k
                break
            }
        }
        guard let key = signingKey else {
            FileHandle.standardError.write("cert-not-found-in-token\n".data(using:.utf8)!)
            exit(4)
        }

        var err: Unmanaged<CFError>?
        guard let sig = SecKeyCreateSignature(key, .rsaSignatureDigestPKCS1v15SHA256, hash as CFData, &err) else {
            let e = (err?.takeRetainedValue() as Error?)?.localizedDescription ?? "unknown"
            FileHandle.standardError.write("sign-failed:\(e)\n".data(using:.utf8)!)
            exit(5)
        }
        print((sig as Data).base64EncodedString())
        """;

    private static readonly string ScriptPath = WriteScriptOnce();

    private readonly ILogger _logger;

    public MacOsCtkTokenSigner(ILogger logger) => _logger = logger;

    public byte[] SignSha256Pkcs1(string tokenId, string certThumbprintHex, byte[] sha256Hash)
    {
        if (sha256Hash.Length != 32)
            throw new ArgumentException("SHA-256 hash must be 32 bytes.", nameof(sha256Hash));

        var psi = new ProcessStartInfo
        {
            FileName               = "swift",
            RedirectStandardOutput = true,
            RedirectStandardError  = true,
            UseShellExecute        = false,
        };
        psi.ArgumentList.Add(ScriptPath);
        psi.ArgumentList.Add(tokenId);
        psi.ArgumentList.Add(certThumbprintHex.ToLowerInvariant());
        psi.ArgumentList.Add(Convert.ToBase64String(sha256Hash));

        using var p = Process.Start(psi)
            ?? throw new DmpAuthException("Impossible de lancer le sous-processus `swift`.");

        var stdout = p.StandardOutput.ReadToEnd().Trim();
        var stderr = p.StandardError.ReadToEnd();
        p.WaitForExit(15_000);

        if (p.ExitCode != 0)
            throw new DmpAuthException(
                $"Signature CTK échouée (exit {p.ExitCode}). stderr={stderr.Trim()}");

        try
        {
            var bytes = Convert.FromBase64String(stdout);
            _logger.LogDebug("Signature CTK générée ({Len} octets) pour le cert {Thumb}",
                bytes.Length, certThumbprintHex);
            return bytes;
        }
        catch (FormatException ex)
        {
            throw new DmpAuthException(
                $"Signature CTK invalide (base64 illisible) : {stdout}", ex);
        }
    }

    private static string WriteScriptOnce()
    {
        var path = Path.Combine(Path.GetTempPath(), "dehempe_ctk_signer.swift");
        File.WriteAllText(path, SwiftScript);
        return path;
    }
}
