/**
* La présente mise à disposition du code exemple ne saurait être interprétée comme un quelconque transfert des droits de propriété sur celui-ci.
* L’utilisateur désigne ci-après l’entité destinataire du code exemple.
* L'attention de l’utilisateur est appelée sur les modalités d'utilisation du code exemple. 
* Ce dernier est fourni à titre d'information  permettant à l’utilisateur de réaliser librement l'adaptation personnalisée nécessaire à la création de l'interfaçage de son logiciel.  
* Le code exemple est transmis en son état de développement sans garantie, il n'a notamment pas fait l'objet de qualification sécuritaire.
* Le code exemple ne fait l'objet d'aucune maintenance.
* L’utilisateur est seul responsable des conditions de l’utilisation du code exemple et est libre de s'inspirer des éléments fournis et de les adapter par ses propres moyens à la situation particulière de la solution logicielle qu'il développe.
* Ainsi notamment, il est déconseillé de procéder par voie de copier-coller du code à partir des exemples fournis.
 */


using System.Security.Cryptography.X509Certificates;

namespace TLSiKitEditeur.CertificateHelpers.Pkcs12
{
    /// <summary>
    /// 
    /// </summary>
    class Pkcs12
    {
        /// <summary>
        /// le certificat X509 obtenu depuis le p12 passe en parametre
        /// </summary>
        public X509Certificate2 certificate { get; set; }
                
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="password"></param>
        public Pkcs12(string fileName, string password)
        {
            certificate = new X509Certificate2(fileName, password);
        }
        
        /// <summary>
        /// signe les donnees avec le certificat
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        //public byte[] sign(byte[] data)
        //{
        //    RSACryptoServiceProvider rsaProvider = (RSACryptoServiceProvider)certificate.PrivateKey;

        //    return rsaProvider.SignData(data, "SHA1");
        //}

        /// <summary>
        /// verifie que la signature passee en parametre est valide
        /// (qu'elle a bien ete generee a l'aide du certificat et a partir des donnees passees en parametre)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="signature"></param>
        /// <returns></returns>
        //public bool verify(byte[] data, byte[] signature)
        //{
        //    RSACryptoServiceProvider rsaProvider = (RSACryptoServiceProvider)certificate.PrivateKey;
        //    return rsaProvider.VerifyData(data, "SHA1", signature);
        //}
    }
}
