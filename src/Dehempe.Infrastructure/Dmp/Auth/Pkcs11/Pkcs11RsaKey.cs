using System.Security.Cryptography;
using Dehempe.Domain.Exceptions;

namespace Dehempe.Infrastructure.Dmp.Auth.Pkcs11;

/// <summary>
/// <see cref="RSA"/> qui délègue toute opération de signature à la clé privée d'un token
/// PKCS#11 (via <see cref="Pkcs11CpsKeyStore"/>) — sans extraire la clé.
///
/// L'instance est destinée à être passée à <c>X509Certificate2.CopyWithPrivateKey(rsa)</c>,
/// produisant un cert avec <c>HasPrivateKey = true</c> que le pipeline TLS de .NET attache
/// au handshake mTLS. Pendant le handshake, <c>SignHash</c> est invoquée par la pile TLS,
/// on relaye au token via PKCS#11.
/// </summary>
internal sealed class Pkcs11RsaKey : RSA
{
    /// <summary>Préfixe ASN.1 DER d'un <c>DigestInfo</c> SHA-256 ; à concaténer avant les 32 octets de hash.</summary>
    private static readonly byte[] Sha256DigestInfoPrefix =
    {
        0x30, 0x31, 0x30, 0x0d, 0x06, 0x09, 0x60, 0x86, 0x48, 0x01,
        0x65, 0x03, 0x04, 0x02, 0x01, 0x05, 0x00, 0x04, 0x20
    };

    private readonly RSA _publicView;
    private readonly Pkcs11CpsKeyStore _store;

    public Pkcs11RsaKey(RSA publicView, Pkcs11CpsKeyStore store)
    {
        _publicView  = publicView;
        _store       = store;
        KeySizeValue = publicView.KeySize;
    }

    public override KeySizes[] LegalKeySizes => _publicView.LegalKeySizes;

    public override byte[] SignHash(byte[] hash, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
    {
        if (hashAlgorithm != HashAlgorithmName.SHA256)
            throw new DmpAuthException(
                $"PKCS#11 CPS ne supporte ici que SHA-256 ; reçu {hashAlgorithm.Name}.");
        if (padding != RSASignaturePadding.Pkcs1)
            throw new DmpAuthException("PKCS#11 CPS ne supporte que le padding PKCS#1 v1.5.");
        if (hash.Length != 32)
            throw new ArgumentException("Hash SHA-256 attendu (32 octets).", nameof(hash));

        // CKM_RSA_PKCS attend le DigestInfo complet, pas le hash brut.
        var digestInfo = new byte[Sha256DigestInfoPrefix.Length + hash.Length];
        Buffer.BlockCopy(Sha256DigestInfoPrefix, 0, digestInfo, 0, Sha256DigestInfoPrefix.Length);
        Buffer.BlockCopy(hash, 0, digestInfo, Sha256DigestInfoPrefix.Length, hash.Length);

        return _store.SignWithAuthKey(digestInfo);
    }

    public override bool VerifyHash(byte[] hash, byte[] signature, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
        => _publicView.VerifyHash(hash, signature, hashAlgorithm, padding);

    public override RSAParameters ExportParameters(bool includePrivateParameters)
    {
        if (includePrivateParameters)
            throw new CryptographicException("La clé privée CPS reste verrouillée dans le token PKCS#11.");
        return _publicView.ExportParameters(includePrivateParameters: false);
    }

    public override void ImportParameters(RSAParameters parameters)
        => throw new NotSupportedException("Pkcs11RsaKey ne supporte pas l'import de clé.");

    protected override void Dispose(bool disposing)
    {
        if (disposing) _publicView.Dispose();
        base.Dispose(disposing);
    }
}
