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
using System.ServiceModel;
using TLSiKitEditeur.CertificateHelpers;
using TLSiKitEditeur.DocumentRegistryService;
using TLSiKitEditeur.Messages;
using TLSiKitEditeur.SoapServiceHelpers;
using TLSiKitEditeur.VIHF;

namespace TLSiKitEditeur.SoapService
{
    public class DocumentRegistryHistoCVASoapService : IDisposable
    {
        public static AdhocQueryResponse RegistryStoredQueryResponse = null;

        private DocumentRegistry_PortTypeClient _service;
        public DocumentRegistryHistoCVASoapService()
        {
            _service = new DocumentRegistry_PortTypeClient();
            _service.Init("registry");
        }

        public RegistryStoredQueryResponse FindDocument()
        {
            return RegistryStoredQuery("Resources\\template_FindDocument.xml");
        }

        public RegistryStoredQueryResponse RegistryStoredQuery(string PathToXML)
        {
            Logger.Log.Info($"Appel de la fonction : {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            _service.AddHeaders("urn:ihe:iti:xds-b:2007:RegistryStoredQuery-b");
            var request = XDSb_CVA.LoadRegistryXdsbAdhocQueryRequest(PathToXML);
            var response = _service.DocumentRegistry_RegistryStoredQuery(request);

            if (CheckResponseOk(RegistryStoredQueryResponse))
            {
                var registryStoredQueryResponse = new RegistryStoredQueryResponse();

                foreach (var obj in RegistryStoredQueryResponse.RegistryObjectList)
                {
                    // Recherche du node correspondant à l'historique de vaccination code 11369-6
                    foreach (ClassificationType ct in obj.Classification)
                    {
                        if ("11369-6".Equals(ct.nodeRepresentation))
                        {
                            DocumentIdentity documentIdentity = new DocumentIdentity();

                            foreach (var slot in obj.Slot)
                            {
                                if ("repositoryUniqueId".Equals(slot.name))
                                {
                                    documentIdentity.SetRepositoryUniqueId(slot.ValueList.Value[0]);
                                }
                            }
                            foreach (var externalIdentifier in obj.ExternalIdentifier)
                            {
                                if ("XDSDocumentEntry.uniqueId".Equals(externalIdentifier.Name.LocalizedString[0].value))
                                {
                                    documentIdentity.SetDocumentUniqueId(externalIdentifier.value);
                                }
                            }

                            if (documentIdentity.GetRepositoryUniqueId() != null && documentIdentity.GetDocumentUniqueId() != null)
                            {
                                registryStoredQueryResponse.documentIdentities = documentIdentity;
                            }
                        }
                    }
                }
                
                return registryStoredQueryResponse;
            }
            return null;
        }

        private bool CheckResponseOk(RegistryResponseType response)
        {
            Console.WriteLine("Statut de la reponse: " + response.status);

            if (response.status == "urn:oasis:names:tc:ebxml-regrep:ResponseStatusType:Success" && response.RegistryErrorList == null)
            {
                Console.WriteLine("Reponse:"+response.ResponseSlotList);
                return true;
            }
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
        // ~DocumentRegistrySoapService() {
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