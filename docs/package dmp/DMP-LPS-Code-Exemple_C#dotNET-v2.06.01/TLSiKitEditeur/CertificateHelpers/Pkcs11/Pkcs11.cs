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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

namespace TLSiKitEditeur.CertificateHelpers.Pkcs11
{
    /**
    * Classe offrant les fonctionnalites PKCS11
    * */
    class Pkcs11
    {
        /// <summary>
        /// Le magasin de certificat utilise
        /// </summary>
        public X509Store x509Store { get; set; }

        /// <summary>
        /// Le certificat de signature
        /// </summary>
        public X509Certificate2 signatureCertificate { get; set; }

        /// <summary>
        /// Le certificat d'authentification
        /// </summary>
        public X509Certificate2 authentificationCertificate { get; set; }

        /// <summary>
        /// Constructeur
        /// </summary>
        public Pkcs11()
        {
            // chargement du store X509 Windows (les certificats de la carte CPS se trouve dedans
            // car la dll CSP de la cryptolib est inscrite comme composant cryptographique dans la base de registre)            
            x509Store = new X509Store(StoreName.My, StoreLocation.CurrentUser);

            // Ouverture du magasin de certificats en mode Lecture/Ecriture
            x509Store.Open(OpenFlags.ReadWrite);

            bool hasClientAuthentication = false;
            bool hasSmartCardLogon = false;
            String issuer = null;

            // On itere sur la liste des certificats présents dans le magasin
            foreach (X509Certificate2 certificate in x509Store.Certificates)
            {
                //on évalue le type de certificat
                typeCertificat(certificate, ref hasClientAuthentication, ref hasSmartCardLogon);
                if (hasSmartCardLogon && hasClientAuthentication)
                { 
                    authentificationCertificate = certificate;
                    //on stocke la valeur de l'issuer afin d'aller chercher la signature correspondante
                    issuer = certificate.Issuer;
                }
            }

            foreach (X509Certificate2 certificate in x509Store.Certificates)
            {
                if(certificate.Issuer.Equals(issuer))
                {
                    //recherche des extensions pour déterminer le type de certitifcat (signature / authentification)
                    foreach (X509Extension ext in certificate.Extensions)
                    {
                        if (ext is X509KeyUsageExtension)
                        {
                            X509KeyUsageFlags flags = ((X509KeyUsageExtension)ext).KeyUsages;
                            //certificat de signature :
                            if ((flags & X509KeyUsageFlags.NonRepudiation) == X509KeyUsageFlags.NonRepudiation)
                                signatureCertificate = certificate;
                        }
                    }
                }
            }
            
            if (authentificationCertificate == null || signatureCertificate == null) throw new Exception("Impossible de charger les certificats de la carte CPS");
        }

        /// <summary>
        /// Détermine le type du certificat
        /// </summary>
        /// <param name="certif">le certificat à tester</param>
        /// <param name="hasClientAuthentication">true si c'est un certificat d'authentification</param>
        /// <param name="hasSmartCardLogon">true si c'est une carte à puce</param>
        private void typeCertificat(X509Certificate2 certif,
                                    ref bool hasClientAuthentication,
                                    ref bool hasSmartCardLogon)
        {
            var oidClientAuthentication = "1.3.6.1.5.5.7.3.2"; //Client Authentication
            var oidSmartCardLogon = "1.3.6.1.4.1.311.20.2.2"; //Smart Card Logon
            hasClientAuthentication = false;
            hasSmartCardLogon = false;
            
            foreach (var ext in certif.Extensions)
            {
                var eku = ext as X509EnhancedKeyUsageExtension;
                if (eku != null)
                {
                    foreach (var oid in eku.EnhancedKeyUsages)
                    {
                        if (oid.Value == oidClientAuthentication)
                        {
                            hasClientAuthentication = true;
                        }

                        if (oid.Value == oidSmartCardLogon)
                        {
                            hasSmartCardLogon = true;
                        }
                    }
                }
            }
        }





        /// <summary>
        /// Fonction signant les donnees en entree avec le certificat de signature de la carte CPS
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        //public byte[] sign(byte[] data)
        //{
        //    // Creation d'un Provider RSA depuis la cle privee
        //    RSACryptoServiceProvider rsaProvider = (RSACryptoServiceProvider)signatureCertificate.PrivateKey;

        //    // Signature en utilisant la cle privee
        //    return rsaProvider.SignData(data, "SHA1");
        //}
    }
}
