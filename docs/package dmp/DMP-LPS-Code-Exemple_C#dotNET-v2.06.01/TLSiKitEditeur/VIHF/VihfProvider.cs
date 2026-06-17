using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using TLSiKitEditeur.CertificateHelpers;

namespace TLSiKitEditeur.VIHF
{
  ///  VIHF.ExampleVIHF.getExampleVIHF(CertificateHelper.SignatureCertificate, Dmp.PatientIns)
   public class VihfProvider
    {
        private static readonly Lazy<VihfProvider> lazy =
            new Lazy<VihfProvider>(() => new VihfProvider());
        public static VihfProvider Instance { get { return lazy.Value; } }
        private VihfProvider()
        {
        }
        public VIHF GetVihf()
        {
            var result = TestVihf.getExampleVIHF();
            return result;
        }
    }

    public static class TestVihf
    {
        public static VIHF getExampleVIHF()
        {
            //  Program.ts.TraceEvent(TraceEventType.Information, 0, "Création du VIHF Example");
            VIHF vihf = new VIHF();

            // extraction des donnees du certificat
            string[] subject = CertificateHelper.SignatureCertificate.Subject.Split(new char[] { ',', '+' });

            // On affiche le Subject dans le log
            //  Program.ts.TraceEvent(TraceEventType.Verbose, 0, "Certificat client utilisé : "+ certificate.ToString());

            string CN = "";
            string SN = "";
            string G = "";
            string OU = "";

            foreach (string part in subject)
            {
                if (part.Trim().StartsWith("CN=")) CN = part.Trim().Split(new char[] { '=' })[1];
                if (part.Trim().StartsWith("SN=")) SN = part.Trim().Split(new char[] { '=' })[1];
                if (part.Trim().StartsWith("G=")) G = part.Trim().Split(new char[] { '=' })[1];
                if (part.Trim().StartsWith("OU=")) OU = part.Trim().Split(new char[] { '=' })[1];
            }

            // Placement des informations dans le VIHF en fonction du type d'authentification
            if (Dmp.UseCPS)
            {
                vihf.Identite_Utilisateur = G + " " + SN;
                vihf.Identifiant_Structure = Dmp.ID_STRUCTURE_CPS;
                vihf.NameId = CN;
                vihf.Secteur_Activite = "SA07^1.2.250.1.71.4.2.4";
                Dmp.AuthorPerson = CN + "^" + SN + "^" + G + "^^^^^^&1.2.250.1.71.4.2.1&ISO^D^^^IDNPS";
            }
            else
            {
                // ATTENTION : Valeur codée en dur
                vihf.Identite_Utilisateur = "Dr CHU-PUBLIC Jean";
                vihf.Identifiant_Structure = OU;
                Dmp.ID_STRUCTURE_CPS = OU;
                vihf.NameId = Dmp.PSIDNAT_AUTH_INDIRECT;
                vihf.Secteur_Activite = "SA05^1.2.250.1.71.4.2.4";
            }

            // Issuer : le GivenName retourné via la classe X509Certificate2 est identifié par 'G=' alors que le 
            // Cadre d'Interopérabilité utilise 'GN=', on fait donc un Replace pour cette valeur
            vihf.Issuer = CertificateHelper.SignatureCertificate.Subject.Replace("G=", "GN=");

            //28/09/2016 mise à jour pour l'IGC Santé : les subject des certificats IGC Santé contiennent un champ "ST" mais l'API .Net le ressort avec un "S"
            //La RFC 2253 indique "ST" (format RFC 2253 requis dans le Cadre d'Interopérabilité des SIS, cf Volet transport synchrone § 4.3.1.5.1.1 /Assertion/Issuer) :
            //on remplace donc "S=" par "ST=" pour être conforme
            vihf.Issuer = vihf.Issuer.Replace("S=", "ST=");

            //   Program.ts.TraceEvent(TraceEventType.Verbose, 0, "Issuer du VIHF fourni au SI DMP : " + exampleVIHF.Issuer);

            //Identification du LPS utilisé

            vihf.LPS_Version = Dmp.LPS_Version;
            vihf.LPS_ID = Dmp.LPS_ID;
            vihf.LPS_ID_HOMOLOGATION_DMP = Dmp.LPS_ID_HOMOLOGATION_DMP;
            vihf.LPS_Nom = Dmp.LPS_Nom;
            vihf.purposeOfuse = new PurposeOfUse("normal", "1.2.250.1.213.1.1.4.248", "mode acces VIHF 1.0", "Accès normal");
            vihf.Ressource_Id = Dmp.ResourceNirOD; //INS-NIR
            vihf.Ressource_URN = "urn:dmp";
            vihf.addRole(new Role(Dmp.ProfessionCode, Dmp.ProfessionSystemCode,Dmp.ProfessionDisplayName,Dmp.ProfessionSystemName));
            if (Dmp.ProfessionCode=="10" || Dmp.ProfessionCode == "20")
            {
                vihf.addRole(new Role(Dmp.SpecCode, Dmp.SpecSystemCode, Dmp.SpecDisplayName, Dmp.SpecSystemName));
            }
            return vihf;
        }
    }

}
