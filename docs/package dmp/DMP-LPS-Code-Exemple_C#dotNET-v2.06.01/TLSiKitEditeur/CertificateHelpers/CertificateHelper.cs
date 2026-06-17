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
using System.Data;
using System.Security.Cryptography.X509Certificates;

namespace TLSiKitEditeur.CertificateHelpers
{
    public static class CertificateHelper
    {
        public static X509Certificate2 AuthCertificate { get; private set; }
        public static X509Certificate2 SignatureCertificate { get; private set; }


        /// <summary>Initialise les certificats</summary>
        /// <exception cref="NoNullAllowedException"></exception>
        public static void InitCertificate()
        {
            if (Dmp.UseCPS)
            {
                Logger.Log.Info("Selection des certificats Pkcs11.");

                // recuperation du certificat depuis la carte CPS
                Pkcs11.Pkcs11 pkcs11 = new Pkcs11.Pkcs11();
                AuthCertificate = pkcs11.authentificationCertificate;
                SignatureCertificate = pkcs11.signatureCertificate;
            }
            else
            {
                Logger.Log.Info("Selection des certificats Pkcs12.");
                // recuperation du certificat P12 depuis le fichier
                Pkcs12.Pkcs12 pkcs12Auth = new Pkcs12.Pkcs12(Dmp.P12_AUTHENTIFICATION_CERTIFICATE_PATH,
                    Dmp.P12_AUTHENFTIFICATION_PASSWORD);
                Pkcs12.Pkcs12 pkcs12Sign =
                    new Pkcs12.Pkcs12(Dmp.P12_SIGNATURE_CERTIFICATE_PATH, Dmp.P12_SIGNATURE_PASSWORD);
                AuthCertificate = pkcs12Auth.certificate;
                SignatureCertificate = pkcs12Sign.certificate;
            }

            if (CertificateHelper.AuthCertificate == null || CertificateHelper.SignatureCertificate == null)
            {
                throw new NoNullAllowedException();
            }
        }


        /// <summary>
        /// Verification de la presence des certificats des autorites de certification dans les stores de Microsoft
        /// </summary>
        /// <returns>true si tous present</returns>
        public static bool CheckCertificates()
        {
            bool rc = true;

            X509Certificate2Collection CertCol;
            X509Certificate2Collection CertCol2;
            X509Certificate2Collection c;
            X509Certificate2Collection c2;
            X509Store rootStore = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            X509Store rootStore2 = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
            X509Store intermediateStore = new X509Store(StoreName.CertificateAuthority, StoreLocation.LocalMachine);
            X509Store intermediateStore2 = new X509Store(StoreName.CertificateAuthority, StoreLocation.CurrentUser);
            rootStore.Open(OpenFlags.ReadOnly);
            rootStore2.Open(OpenFlags.ReadOnly);
            intermediateStore.Open(OpenFlags.ReadOnly);
            intermediateStore2.Open(OpenFlags.ReadOnly);

            // Vérification de la présence des Autorités principales de confiance

            // Filtre sur les certificats contenant "GIP-CPS"
            CertCol = rootStore.Certificates.Find(X509FindType.FindByIssuerName, "GIP-CPS", true);
            c = CertCol.Find(X509FindType.FindBySerialNumber, "30 30 30 31 31 30 36 35 36 39 34 38 32 30 30 30 00",
                true);
            CertCol2 = rootStore2.Certificates.Find(X509FindType.FindByIssuerName, "GIP-CPS", true);
            c2 = CertCol2.Find(X509FindType.FindBySerialNumber, "30 30 30 31 31 30 36 35 36 39 34 38 32 30 30 30 00",
                true);
            if ((c.Count == 0) && (c2.Count == 0))
            {
                Logger.Log.Info(
                    "Le certificat GIP-CPS ne se trouve pas dans le Store Microsoft \"Autorités principales de confiance\"! Veuillez l'installer ...");
               
                rc = false;
            }

            // Filtre sur les certificats contenant "TEST PROFESSIONNEL"
            // MAJ 14/09/2012 : mise à jour des numéros de série (utilisation de l'IGC CPS2ter prorogée "2020")
            CertCol = rootStore.Certificates.Find(X509FindType.FindByIssuerName, "TEST PROFESSIONNEL", true);
            c = CertCol.Find(X509FindType.FindBySerialNumber, "11 01", true); //num. série en decimale = 4353
            CertCol2 = rootStore2.Certificates.Find(X509FindType.FindByIssuerName, "TEST PROFESSIONNEL", true);
            c2 = CertCol2.Find(X509FindType.FindBySerialNumber, "11 01", true); //num. série en decimale = 4353
            if ((c.Count == 0) && (c2.Count == 0))
            {
                Logger.Log.Info(
                    "Le certificat TEST PROFESSIONNEL ne se trouve pas dans le Store Microsoft \"Autorités principales de confiance\"! Veuillez l'installer ...");
              
                rc = false;
            }

            // Filtre sur les certificats contenant "GIP-CPS-TEST"
            CertCol = rootStore.Certificates.Find(X509FindType.FindByIssuerName, "GIP-CPS-TEST", true);
            c = CertCol.Find(X509FindType.FindBySerialNumber, "30 30 30 31 31 30 36 32 33 32 36 37 34 30 30 30 00",
                true);
            CertCol2 = rootStore2.Certificates.Find(X509FindType.FindByIssuerName, "GIP-CPS-TEST", true);
            c2 = CertCol2.Find(X509FindType.FindBySerialNumber, "30 30 30 31 31 30 36 32 33 32 36 37 34 30 30 30 00",
                true);
            if ((c.Count == 0) && (c2.Count == 0))
            {
                Logger.Log.Info(
                    "Le certificat GIP-CPS-TEST ne se trouve pas dans le Store Microsoft \"Autorités principales de confiance\"! Veuillez l'installer ...");
              
                rc = false;
            }

            // Vérification de la présence des Autorités intermédiaires

            // Filtre sur les certificats contenant "AC-CLASSE-4"
            CertCol = intermediateStore.Certificates.Find(X509FindType.FindByIssuerName, "GIP-CPS", true);
            c = CertCol.Find(X509FindType.FindBySerialNumber, "30 30 30 31 31 30 34 32 34 33 30 39 30 30 30 30 00",
                true);
            CertCol2 = intermediateStore2.Certificates.Find(X509FindType.FindByIssuerName, "GIP-CPS", true);
            c2 = CertCol2.Find(X509FindType.FindBySerialNumber, "30 30 30 31 31 30 34 32 34 33 30 39 30 30 30 30 00",
                true);
            if ((c.Count == 0) && (c2.Count == 0))
            {
                Logger.Log.Info(
                    "Le certificat AC-CLASSE-4 ne se trouve pas dans le Store Microsoft \"Autorités intermédiaires\"! Veuillez l'installer ...");
               
                rc = false;
            }

            // Filtre sur les certificats contenant "TEST CLASSE-1"
            // MAJ 14/09/2012 : mise à jour des numéros de série (utilisation de l'IGC CPS2ter prorogée "2020")
            CertCol = intermediateStore.Certificates.Find(X509FindType.FindByIssuerName, "TEST PROFESSIONNEL", true);
            c = CertCol.Find(X509FindType.FindBySerialNumber, "11 12", true); //num. série en decimale = 4370
            CertCol2 = intermediateStore2.Certificates.Find(X509FindType.FindByIssuerName, "TEST PROFESSIONNEL", true);
            c2 = CertCol2.Find(X509FindType.FindBySerialNumber, "11 12", true); //num. série en decimale = 4370
            if ((c.Count == 0) && (c2.Count == 0))
            {
                Logger.Log.Info(
                    "Le certificat TEST CLASSE-1 ne se trouve pas dans le Store Microsoft \"Autorités intermédiaires\"! Veuillez l'installer ...");
                
                rc = false;
            }

            // Filtre sur les certificats contenant "AC-CLASSE-4-TEST"
            CertCol = intermediateStore.Certificates.Find(X509FindType.FindByIssuerName, "GIP-CPS-TEST", true);
            c = CertCol.Find(X509FindType.FindBySerialNumber, "30 30 30 31 31 30 34 32 33 30 31 30 39 30 30 30 00",
                true);
            CertCol2 = intermediateStore2.Certificates.Find(X509FindType.FindByIssuerName, "GIP-CPS-TEST", true);
            c2 = CertCol2.Find(X509FindType.FindBySerialNumber, "30 30 30 31 31 30 34 32 33 30 31 30 39 30 30 30 00",
                true);
            if ((c.Count == 0) && (c2.Count == 0))
            {
                Logger.Log.Info(
                    "Le certificat AC-CLASSE-4-TEST ne se trouve pas dans le Store Microsoft \"Autorités intermédiaires\"! Veuillez l'installer ...");
               
                rc = false;
            }

            if (rc == false)
            {
                Logger.Log.Info(
                    "NB: L'installation de ces certificats peut se faire via l'installation de la CryptoLib");
               
            }

            return rc;
        }
    }
}