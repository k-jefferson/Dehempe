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
using TLSiKitEditeur.VIHF;

namespace TLSiKitEditeur
{
    /// <summary>class statique pour les variables partageés utiliser dans les exemples</summary>
    public static class Dmp
    {
        public static bool UseCPS = false;
        public static bool UseAIR = true;
        //=============Proxy
        public static bool UseProxy = true;
        public static string ProxyAddress = 
        public static string ProxyUsername =
        public static string ProxyPassword = 
        //=============
        public static string PatientIns = "207058575627097";
        public static string PatientInsc = "0740724093302701701806";
        public static string ID_STRUCTURE_CPS = "10B0167011";
        public static string XDS_DOCUMENT_UNIQUE_ID = "1.2.250.1.999.1.1.7898.1.995657539";
        public static string XDS_HISTOVAC_UNIQUE_ID = "1.2.250.1.213.4.1.1.1.4.1.1014850115.20220207132401.5";
        public static string XDS_CVA_UNIQUE_ID = "1.2.250.1.999.1.1.7898.4.1649424171399";
        public static string XDS_CVA_ENTRY_UUID = "urn:uuid:1eed738d-b71f-11ec-80da-0050569e422e";
        public static string XDS_SUBMISSIONSET_UNIQUE_ID = "1.2.250.1.999.1.1.7898.1.1103987654324";
        public static string XDS_SIGNATURE_UNIQUE_ID = "1.2.250.1.999.1.1.7898.3.1103993";
        public static string PSIDNAT_AUTH_INDIRECT = "899900077985";
        public static string SI_DMP_DOMAIN = "https://dev6.lps2.dmp.gouv.fr/si-dmp-server/v2/services/";
        //================== Certificate
        public static string P12_AUTHENTIFICATION_CERTIFICATE_PATH = "S:\\TLSI\\DMP\\KitEditeur_Certificats\\HOPITAL_DES_3-DAMES\\asip-p12-EL-TEST-ORG-AUTH_CLI-20220322-153515.p12";
        public static string P12_AUTHENFTIFICATION_PASSWORD = 
        public static string P12_SIGNATURE_CERTIFICATE_PATH = "S:\\TLSI\\DMP\\KitEditeur_Certificats\\HOPITAL_DES_3-DAMES\\asip-p12-EL-TEST-ORG-SIGN-20220215-151658.p12";
        public static string P12_SIGNATURE_PASSWORD =
        //==================
        public static string ResourceNirOD = "207058575627097^^^&1.2.250.1.213.1.4.10&ISO^NH";
        public static string patientNirOD = "207058575627097^^^&1.2.250.1.213.1.4.10&ISO^NH";
        public static string dateNaiss = "1953-07-24";
        public static string CDA_TEMPLATE = "..\\..\\Resources\\template_CDA.xml";
        public static string CDA_TEMPLATE_CVA = "..\\..\\Resources\\template_CVA_CDA.xml";
        public static string CDA_TEMPLATE_CVA_update = "..\\..\\Resources\\template_CVA_CDA_Update.xml";
        public static string DocumentCDA_OID = "1.2.250.1.999.1.1.7898.1." + (995656565 + (DateTime.Now.Hour * 60) + (DateTime.Now.Minute)).ToString();
        public static string SubmissionSet_OID = "1.2.250.1.999.1.1.7898.3." + (88465413 + (DateTime.Now.Hour * 60) + (DateTime.Now.Minute)).ToString();
        public static string Signature_OID = "1.2.250.1.999.1.1.7898.4." + (1103987654324 + (DateTime.Now.Hour * 60) + (DateTime.Now.Minute)).ToString();
        public static string AuthorPerson = PSIDNAT_AUTH_INDIRECT + "^Kit^Editeur^^^^^^&1.2.250.1.71.4.2.1&ISO^D^^^IDNPS";
        //================== LPS
        public static string LPS_Version = "1.1";
        public static string LPS_ID = "1.2.250.1.999.888.2";
        public static string LPS_ID_HOMOLOGATION_DMP = "KIR-gpt-875-55-hue-dia-R2";
        public static string LPS_Nom = "KIR Royal R2";
        //==================
        
        //================== Profession
        public static string ProfessionCode = "40";
        public static string ProfessionDisplayName = "Chirurgien-Dentiste";
        public static string ProfessionSystemCode = "1.2.250.1.71.1.2.7";
        public static string ProfessionSystemName = "professions";
        //==================
        //================== Spec
        public static string SpecCode = "SM07";
        public static string SpecDisplayName = "Médecin - Chirurgie maxillo-faciale et stomatologie (SM)";
        public static string SpecSystemCode = "1.2.250.1.71.4.2.5";
        public static string SpecSystemName = "specialites RPPS";


    }
}