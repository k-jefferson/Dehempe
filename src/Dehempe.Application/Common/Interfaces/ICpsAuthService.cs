using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Dehempe.Application.Common.Interfaces;

/// <summary>
/// Fournit l'accès aux DEUX paires (cert, clé) d'une carte CPS3 :
/// la paire d'<b>authentification</b> (mTLS / ClientCertVerify) et la paire de
/// <b>signature</b> (assertions VIHF, documents). Ces deux usages sont distincts —
/// les utiliser à mauvais escient fait rejeter la requête par le DMP. Voir
/// <c>CLAUDE.md</c> § « Paires de clés CPS3 ».
/// </summary>
public interface ICpsAuthService
{
    /// <summary>
    /// Certificat d'<b>authentification</b> du praticien (CPS3 : <c>CKA_ID</c> terminant par <c>0x20</c>).
    /// Destiné au handshake mTLS uniquement. À NE PAS utiliser pour signer le VIHF.
    /// </summary>
    Task<X509Certificate2> GetAuthenticationCertificateAsync(CancellationToken ct = default);

    /// <summary>
    /// Clé privée d'<b>authentification</b> du praticien, sous la forme d'une RSA déléguée
    /// (la clé reste verrouillée dans le token PKCS#11 / CTK).
    /// Destinée au handshake mTLS uniquement.
    /// </summary>
    Task<RSA> GetAuthenticationKeyAsync(CancellationToken ct = default);

    /// <summary>
    /// Certificat de <b>signature</b> du praticien (CPS3 : <c>CKA_ID</c> terminant par <c>0x10</c>).
    /// À utiliser pour signer le VIHF et les documents — c'est ce que le DMP exige.
    /// Le DN porte le même praticien que le cert d'auth mais les usages X.509 diffèrent
    /// (<c>digitalSignature, nonRepudiation</c> vs <c>digitalSignature, keyEncipherment</c>).
    /// </summary>
    Task<X509Certificate2> GetSignatureCertificateAsync(CancellationToken ct = default);

    /// <summary>
    /// Clé privée de <b>signature</b>, sous la forme d'une RSA déléguée. À utiliser pour
    /// <c>SignedXml.SigningKey</c> dans la construction du VIHF.
    /// </summary>
    Task<RSA> GetSignatureKeyAsync(CancellationToken ct = default);
}
