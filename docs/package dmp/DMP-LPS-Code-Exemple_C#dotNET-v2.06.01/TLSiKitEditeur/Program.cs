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
using TLSiKitEditeur.CertificateHelpers;
using TLSiKitEditeur.Helpers;
using TLSiKitEditeur.PatientSpecificService;
using TLSiKitEditeur.SoapService;

namespace TLSiKitEditeur
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Log.Info("Initialization des certificats");
            CertificateHelper.InitCertificate();

            // Verification de la presence des certificats necessaires
            Logger.Log.Info("Vérification des certificats");
            CertificateHelper.CheckCertificates();

            Logger.Log.Info("Initialization de Proxy");
            WebProxyHelper.ConfigureProxy();

            HabilitationSoapService habilitationSoapService = new HabilitationSoapService();
            PatientSoapService patientSoapService = new PatientSoapService();
            DocumentRepositorySoapService documentRepositorySoapService = new DocumentRepositorySoapService();
            DocumentRepositoryCVASoapService documentRepositoryCVASoapService = new DocumentRepositoryCVASoapService();
            DocumentRegistrySoapService documentRegistrySoapService = new DocumentRegistrySoapService();
            DocumentRegistryCVASoapService documentRegistryCVASoapService = new DocumentRegistryCVASoapService();
            DocumentRegistryHistoCVASoapService documentRegistryHistoCVASoapService = new DocumentRegistryHistoCVASoapService();

            // TLSi INS : acquérir les données concernant le patient
            INS.PatientINS.findINS();

            // WS_DMP_TD0.0 : acquérir les données concernant le patient
            //patientSoapService.FindNIR(Dmp.patientNirOD, Dmp.dateNaiss, 1);

            // WS_DMP_TD1.1 : Création DMP
            //patientSoapService.CreateDMP();
            
            // WS_DMP_TD0.5 : Recherche d'un patient sans INS
            patientSoapService.FindCandidatesQueryPatient();
            
            // WS_DMP_TD1.6 : Liste des PS autorisés
            habilitationSoapService.ListAuthorizationByPatient();
            
            // WS_DMP_TD0.3 : Mise à jour de l'autorisation
            habilitationSoapService.RevokeAuthorization();
            habilitationSoapService.GrantAuthorization();
            
            // WS_DMP_TD0.4 : Liste des DMP autorisés
            patientSoapService.PatientList("01012018");
            
            // WS_DMP_TD1.5a : Création du compte internet patient
            patientSoapService.CreatePatientAccess(otpChannelTypeEnum.EMAIL, "alain@gmail.com");
            
            // WS_DMP_TD1.5d : Maj des informations du compte internet
            patientSoapService.UpdatePatientAccess();
            
            // WS_DMP_TD1.3a : Consultation des données administratives
            patientSoapService.GetDataDMP();

            //------------------------------------- CVA --------------------
            //  Alimentation Note de Vaccination
            documentRepositoryCVASoapService.ProvideAndRegisterDocumentSet();

            //  Recherche de document Historique de vaccination
            //documentRepositoryCVASoapService.RetrieveDocumentSet();
            RegistryStoredQueryResponse registryStoredCVAQueryResponse = documentRegistryHistoCVASoapService.FindDocument();

            //  Consultation d'un document historique de vaccination
            documentRepositoryCVASoapService.RetrieveDocumentSet(registryStoredCVAQueryResponse.documentIdentities);

            //  Mettre à jour un document
            documentRepositoryCVASoapService.ProvideAndRegisterDocumentSetUpdate();

            // WS_DMP_TD3.3c : Supprimer un document
            documentRegistryCVASoapService.Delete();


            //-------------------------------------/ CVA --------------------

            // WS_DMP_TD2.1 : Alimentation
             documentRepositorySoapService.ProvideAndRegisterDocumentSet();

            // WS_DMP_TD3.1 : Recherche de document
            RegistryStoredQueryResponse registryStoredQueryResponse = documentRegistrySoapService.FindDocument();

            // WS_DMP_TD3.2 : Consultation d'un document
             documentRepositorySoapService.RetrieveDocumentSet(registryStoredQueryResponse.documentIdentities);

            // WS_DMP_TD3.3a : Masquer un document au PS
              documentRegistrySoapService.Mask();

              // WS_DMP_TD3.3c : Supprimer un document
              documentRegistrySoapService.Delete();

              // WS_DMP_TD3.3d : Archiver un document
              documentRegistrySoapService.Archive();

              // WS_DMP_TD1.5b : Ajout d'un canal OTP
              patientSoapService.PatientOtpUpdate(otpChannelTypeEnum.EMAIL, actionTypeEnum.AJOUT, "alain@gmail.com");

             // WS_DMP_TD0.2 : Test d'existence
             patientSoapService.IsDMPExist();

             // WS_DMP_TD1.3b : Maj des données administratives
              patientSoapService.ModifyDataDMP();

             // WS_DMP_TD1.4 : Fermeture
             //patientSoapService.CloseDMP();

             // WS_DMP_TD1.2 : Réactivation
             patientSoapService.ReactivateDMP();
             
            Console.ReadKey();
        }
    }
}
