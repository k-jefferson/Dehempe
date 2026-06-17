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
using System.IO;
using System.Text;
using System.Xml;
using TLSiKitEditeur.CertificateHelpers;
using TLSiKitEditeur.DocumentRepositoryService;
using TLSiKitEditeur.SoapServiceHelpers;
using TLSiKitEditeur.VIHF;


namespace TLSiKitEditeur.SoapService
{
    public class DocumentRepositoryCVASoapService : IDisposable
    {
        private DocumentRepository_PortTypeClient _service;

        public DocumentRepositoryCVASoapService()
        {
            _service = new DocumentRepository_PortTypeClient();
            _service.Init("repository");
        }

        public void ProvideAndRegisterDocumentSet()
        {
            Logger.Log.Info($"Appel de la fonction : {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            _service.AddHeaders("urn:ihe:iti:xds-b:2007:ProvideAndRegisterDocumentSet-b");
            SubmitObjectsRequest request = XDSb_CVA.createXDSb(Dmp.PatientIns, Dmp.SubmissionSet_OID, Dmp.DocumentCDA_OID, Dmp.Signature_OID, Dmp.AuthorPerson, VihfProvider.Instance.GetVihf().Identifiant_Structure);
            ProvideAndRegisterDocumentSetRequestType document = new ProvideAndRegisterDocumentSetRequestType();
            XmlDocument documentCDA = XDSb_CVA.loadCDADocument();
            document.Document = new ProvideAndRegisterDocumentSetRequestTypeDocument[2];
            document.Document[0] = new ProvideAndRegisterDocumentSetRequestTypeDocument();
            document.Document[0].id = "Signature01";
            document.Document[0].Value = XDSb_CVA.getXadesSignature(documentCDA, Dmp.DocumentCDA_OID, Dmp.SubmissionSet_OID, Dmp.Signature_OID, CertificateHelper.SignatureCertificate);
#if DEBUG
            //dump xades
            FileStream fs = new FileStream("Xades_part.xml", FileMode.Create);
            BinaryWriter w = new BinaryWriter(fs);
            w.Write(document.Document[0].Value);
            fs.Close();
            XmlDocument xmlXades = new XmlDocument();
            xmlXades.Load("Xades_part.xml");
            XDSb_CVA.checkSignature(xmlXades);
#endif
            document.Document[1] = new ProvideAndRegisterDocumentSetRequestTypeDocument();
            document.Document[1].id = "Document01";
            document.Document[1].Value = Encoding.UTF8.GetBytes(documentCDA.OuterXml);
            document.SubmitObjectsRequest = request;

            var response = _service.DocumentRepository_ProvideAndRegisterDocumentSetb(document);
            // si la réponse est OK il conviendrait de stocker le UniqueId dans la cle Dmp.XDS_CVA_UNIQUE_ID pour ensuite pouvoir updater ou dépublier le CVA
            CheckResponseOk(response);
        }

        public void ProvideAndRegisterDocumentSetUpdate()
        {
            Logger.Log.Info($"Appel de la fonction : {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            _service.AddHeaders("urn:ihe:iti:xds-b:2007:ProvideAndRegisterDocumentSet-b");
            SubmitObjectsRequest request = XDSb_CVA.createXDSbUpdate(Dmp.PatientIns, Dmp.SubmissionSet_OID, Dmp.DocumentCDA_OID, Dmp.Signature_OID, Dmp.AuthorPerson, VihfProvider.Instance.GetVihf().Identifiant_Structure);
            ProvideAndRegisterDocumentSetRequestType document = new ProvideAndRegisterDocumentSetRequestType();
            XmlDocument documentCDA = XDSb_CVA.loadCDADocumentUpdate();
            document.Document = new ProvideAndRegisterDocumentSetRequestTypeDocument[2];
            document.Document[0] = new ProvideAndRegisterDocumentSetRequestTypeDocument();
            document.Document[0].id = "Signature01";
            document.Document[0].Value = XDSb_CVA.getXadesSignature(documentCDA, Dmp.DocumentCDA_OID, Dmp.SubmissionSet_OID, Dmp.Signature_OID, CertificateHelper.SignatureCertificate);
#if DEBUG
            //dump xades
            FileStream fs = new FileStream("Xades_part.xml", FileMode.Create);
            BinaryWriter w = new BinaryWriter(fs);
            w.Write(document.Document[0].Value);
            fs.Close();
            XmlDocument xmlXades = new XmlDocument();
            xmlXades.Load("Xades_part.xml");
            XDSb_CVA.checkSignature(xmlXades);
#endif
            document.Document[1] = new ProvideAndRegisterDocumentSetRequestTypeDocument();
            document.Document[1].id = "Document01";
            document.Document[1].Value = Encoding.UTF8.GetBytes(documentCDA.OuterXml);
            document.SubmitObjectsRequest = request;

            var response = _service.DocumentRepository_ProvideAndRegisterDocumentSetb(document);
            CheckResponseOk(response);
        }

        public void RetrieveDocumentSet(DocumentIdentity documentIdentities)
        {
            Logger.Log.Info($"Appel de la fonction : {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            _service.AddHeaders("urn:ihe:iti:2007:RetrieveDocumentSet");
            RetrieveDocumentSetRequestTypeDocumentRequest[] request = new[] { new RetrieveDocumentSetRequestTypeDocumentRequest() };

            if (documentIdentities != null)
            {
                request[0].DocumentUniqueId = documentIdentities.GetDocumentUniqueId();
                request[0].RepositoryUniqueId = documentIdentities.GetRepositoryUniqueId();
            }

            var response = _service.DocumentRepository_RetrieveDocumentSet(request);
            CheckResponseOk(response.RegistryResponse);
        }

        public void RetrieveDocumentSet()
        {
            Logger.Log.Info($"Appel de la fonction : {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            _service.AddHeaders("urn:ihe:iti:2007:RetrieveDocumentSet");
            RetrieveDocumentSetRequestTypeDocumentRequest[] request = new[] { new RetrieveDocumentSetRequestTypeDocumentRequest() };

            request[0].DocumentUniqueId = Dmp.XDS_HISTOVAC_UNIQUE_ID;
            request[0].RepositoryUniqueId = "1.2.250.1.213.4.1.1.1.2";

            var response = _service.DocumentRepository_RetrieveDocumentSet(request);
            CheckResponseOk(response.RegistryResponse);
        }

        private bool CheckResponseOk(RegistryResponseType response)
        {
            Console.WriteLine("Statut de la reponse: " + response.status);

            if (response.status == "urn:oasis:names:tc:ebxml-regrep:ResponseStatusType:Success" && response.RegistryErrorList == null) return true;

            for (int i = 0; i < response.RegistryErrorList.RegistryError.Length; i++)
            {
                Console.WriteLine("Probleme:");
                Console.WriteLine("Value: " + response.RegistryErrorList.RegistryError[i].Value);
                Console.WriteLine("Code: " + response.RegistryErrorList.RegistryError[i].errorCode);
                Console.WriteLine("location: " + response.RegistryErrorList.RegistryError[i].location);
                Console.WriteLine("severity: " + response.RegistryErrorList.RegistryError[i].severity);
                Console.WriteLine("codeContext: " + response.RegistryErrorList.RegistryError[i].codeContext);
                Console.WriteLine();
            }
            return false;
        }

        #region IDisposable Support
        private bool disposedValue = false; // Pour détecter les appels redondants

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ((IDisposable)_service).Dispose();
                }

                // TODO: libérer les ressources non managées (objets non managés) et remplacer un finaliseur ci-dessous.
                // TODO: définir les champs de grande taille avec la valeur Null.

                disposedValue = true;
            }
        }

        // TODO: remplacer un finaliseur seulement si la fonction Dispose(bool disposing) ci-dessus a du code pour libérer les ressources non managées.
        // ~DocumentRepositoryCVASoapService() {
        //   // Ne modifiez pas ce code. Placez le code de nettoyage dans Dispose(bool disposing) ci-dessus.
        //   Dispose(false);
        // }

        // Ce code est ajouté pour implémenter correctement le modèle supprimable.
        public void Dispose()
        {
            // Ne modifiez pas ce code. Placez le code de nettoyage dans Dispose(bool disposing) ci-dessus.
            Dispose(true);
            // TODO: supprimer les marques de commentaire pour la ligne suivante si le finaliseur est remplacé ci-dessus.
            // GC.SuppressFinalize(this);
        }
        #endregion








    }
}