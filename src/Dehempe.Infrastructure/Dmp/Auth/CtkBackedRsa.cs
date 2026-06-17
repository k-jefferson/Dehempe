using System.Security.Cryptography;
using Dehempe.Domain.Exceptions;

namespace Dehempe.Infrastructure.Dmp.Auth;

/// <summary>
/// <see cref="RSA"/> qui délègue la signature à un token CryptoTokenKit (CPS) sur macOS.
///
/// Le but est de pouvoir passer cette instance en <c>SignedXml.SigningKey</c> sans modifier
/// le code de signature SAML : <c>ComputeSignature()</c> finit par appeler
/// <see cref="SignHash"/>, qu'on intercepte ici pour le déléguer au <see cref="MacOsCtkTokenSigner"/>.
///
/// La partie publique du RSA est exportée normalement (depuis le cert). La partie privée
/// est inaccessible : <see cref="ExportParameters"/> avec <c>includePrivateParameters=true</c> jette.
/// </summary>
internal sealed class CtkBackedRsa : RSA
{
    private readonly RSA _public;
    private readonly MacOsCtkTokenSigner _signer;
    private readonly string _tokenId;
    private readonly string _certThumbprint;

    public CtkBackedRsa(RSA publicKey, MacOsCtkTokenSigner signer, string tokenId, string certThumbprint)
    {
        _public         = publicKey;
        _signer         = signer;
        _tokenId        = tokenId;
        _certThumbprint = certThumbprint;
        KeySizeValue    = publicKey.KeySize;
    }

    public override KeySizes[] LegalKeySizes => _public.LegalKeySizes;

    public override byte[] SignHash(byte[] hash, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
    {
        if (hashAlgorithm != HashAlgorithmName.SHA256)
            throw new DmpAuthException(
                $"Le signeur CTK ne supporte que SHA-256 ; reçu {hashAlgorithm.Name}.");

        if (padding != RSASignaturePadding.Pkcs1)
            throw new DmpAuthException(
                "Le signeur CTK ne supporte que le padding PKCS#1 v1.5.");

        return _signer.SignSha256Pkcs1(_tokenId, _certThumbprint, hash);
    }

    public override bool VerifyHash(byte[] hash, byte[] signature, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
        => _public.VerifyHash(hash, signature, hashAlgorithm, padding);

    public override RSAParameters ExportParameters(bool includePrivateParameters)
    {
        if (includePrivateParameters)
            throw new CryptographicException(
                "La clé privée CPS est verrouillée dans le token CryptoTokenKit ; impossible à exporter.");
        return _public.ExportParameters(includePrivateParameters: false);
    }

    public override void ImportParameters(RSAParameters parameters)
        => throw new NotSupportedException("CtkBackedRsa ne supporte pas l'import de clé.");

    protected override void Dispose(bool disposing)
    {
        if (disposing) _public.Dispose();
        base.Dispose(disposing);
    }
}
