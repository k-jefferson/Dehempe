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
using TLSiKitEditeur.HabilitationService;
using TLSiKitEditeur.SoapServiceHelpers;
using TLSiKitEditeur.VIHF;

namespace TLSiKitEditeur.SoapService
{
    public class HabilitationSoapService : IDisposable
    {
        /// <summary>
        /// webservice Habilitation
        /// </summary>
        HabilitationsEndPointClient _service;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="clientCertificate">Certificat d'authentification</param>
        public HabilitationSoapService()
        {
            _service = new HabilitationsEndPointClient();
            _service.Init("habilitations");
        }

        public WebServiceResponse GrantAuthorization()
        {
            Logger.Log.Info($"Appel de la fonction : {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            _service.AddHeaders("urn:si-dmp-habilitations:HabilitationsEndPoint:setAuthorization");
            WebServiceResponse resp = new WebServiceResponse();
            try
            {
                // appel de la fonction setAuthorization
                resp = _service.setAuthorization(actionType.AJOUT, roleType.MEDECIN_TRAITANT);
                Logger.Log.Info("retour : status=" + resp.status);
                if (string.IsNullOrEmpty(resp.context))
                {
                    Logger.Log.Info("retour : context=" + resp.context);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e.Message);
            }
     
            return resp;
        }

        public WebServiceResponse RevokeAuthorization()
        {
            Logger.Log.Info($"Appel de la fonction : {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            _service.AddHeaders("urn:si-dmp-habilitations:HabilitationsEndPoint:setAuthorization");
            // appel de la fonction setAuthorization
            WebServiceResponse resp = new WebServiceResponse();
            try
            {
                resp = _service.setAuthorization(actionType.SUPPRESSION, roleType.MEDECIN_TRAITANT);
                Logger.Log.Info("retour : status=" + resp.status);
                if (string.IsNullOrEmpty(resp.context))
                {
                    Logger.Log.Info("retour : context=" + resp.context);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e.Message);
            }
            return resp;
        }

        public void ListAuthorizationByPatient()
        {
            Logger.Log.Info($"Appel de la fonction : {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            _service.AddHeaders("urn:si-dmp-habilitations:HabilitationsEndPoint:listAuthorizationByPatient");
            try
            {
                var authList = _service.listAuthorizationByPatient(modeType.TOUTE);
                Logger.Log.Info("retour : status=" + authList.status);
                if (string.IsNullOrEmpty(authList.context))
                {
                    Logger.Log.Info("retour : context=" + authList.context);
                }
                if (authList.listOfAuthorizationByPatient == null) return;
                Logger.Log.Info("listAutorisationByPatient réussie : nombre de PS/ES=" +
                                authList.listOfAuthorizationByPatient.Length);
                foreach (var auth in authList.listOfAuthorizationByPatient)
                {
                    Logger.Log.Info($"ID={auth.nationalId} nom={auth.firstName} {auth.lastName}");
                }
            }
            catch (Exception exp)
            {
                Logger.Log.Error(exp.Message);
            }
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
        // ~HabilitationSoapService() {
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