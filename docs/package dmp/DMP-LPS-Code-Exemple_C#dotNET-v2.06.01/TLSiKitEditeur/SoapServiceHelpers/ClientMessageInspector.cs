
using System.IO;
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
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;

namespace TLSiKitEditeur.SoapServiceHelpers
{
    /// <summary>
    /// Représente un objet inspecteur de message pouvant être ajouté à la collection <c> MessageInspectors </ c> pour afficher ou modifier les messages.
    /// </summary>
    public class ClientMessageInspector : IClientMessageInspector , IEndpointBehavior
    {
       
        /// <inheritdoc />
        /// <summary>Initialise une nouvelle instance de la classe &lt;see cref = "ClientMessageInspector" /&gt;.</summary>
        /// <param name="header">en-têtes</param>
        public ClientMessageInspector()
        {
          
        }

        /// <summary>
        /// Permet l'inspection ou la modification d'un message avant l'envoi d'un message de requête à un service.
        /// </summary>
        /// <param name = "request"> Le message à envoyer au service. </param>
        /// <param name = "channel"> Le canal d'objet client WCF. </param>
        /// <returns>
        /// L'objet renvoyé sous forme d'argument <paramref name = "correlationState"/> de
        /// la <see cref = "M: System.ServiceModel.Dispatcher.IClientMessageInspector.AfterReceiveReply (System.ServiceModel.Channels.Message @, System.Object)" /> méthode.
        /// Ceci est null si aucun état de corrélation n'est utilisé. La meilleure pratique consiste à en faire une <voir cref = "T: System.Guid" /> pour vous assurer que
        /// les objets <paramref name = "correlationState" /> sont identiques.
        /// </returns>
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            Logger.Log.Debug("======Sending Request======");
            for (int i = 0; i < request.Headers.Count; i++)
            {
                request.Headers.RemoveAt(i);
            }

            //foreach (SoapUnknownHeader unknownHeader in SoapServiceInitializer.header)
            //{
            //    request.Headers.Add(new SoapSecureMessageHeader(unknownHeader));

            //}
              request.Headers.Add(new SoapSecureMessageHeader(SoapServiceInitializer.header));
         //   XmlWriterSettings settings = new XmlWriterSettings();
         //   StringBuilder builder = new StringBuilder();
         //   builder.Append(SoapServiceInitializer.header.OuterXml);
         //   var xw = new XmlTextWriter(new StringWriter(builder));
            
         ////   xw.WriteRaw(SoapServiceInitializer.header.OuterXml);
         //   request.Headers.WriteHeader(0,xw);
         //   xw.Dispose();
            Logger.Log.Info(request.ToString());
            return request.Headers;
        }

        /// <summary>
        /// Active l'inspection ou la modification d'un message après la réception d'un message de réponse, mais avant de le renvoyer à l'application cliente.
        /// </summary>
        /// <param name="reply">Le message à transformer en types et à restituer à l'application cliente</param>
        /// <param name="correlationState">Données d'état de corrélation</param>
        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            string xmlReceived = reply.ToString();
            Logger.Log.Debug("======Receiving Reply======");
            Logger.Log.Debug(xmlReceived);
            

            if (xmlReceived.Contains("AdhocQueryResponse"))
            {
                MessageBuffer buffer = reply.CreateBufferedCopy(System.Int32.MaxValue);
                reply = buffer.CreateMessage();
                Message replyToRead = buffer.CreateMessage();

                XmlSerializer serializer = new XmlSerializer(typeof(DocumentRegistryService.AdhocQueryResponse), new XmlRootAttribute
                {
                    ElementName = "AdhocQueryResponse",
                    Namespace = "urn:oasis:names:tc:ebxml-regrep:xsd:query:3.0"
                });
                
                using (var reader = replyToRead.GetReaderAtBodyContents())
                {
                    SoapService.DocumentRegistrySoapService.RegistryStoredQueryResponse = (DocumentRegistryService.AdhocQueryResponse)serializer.Deserialize(reader);
                }
            }
        }

        public void Validate(ServiceEndpoint endpoint)
        {
            
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
           
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            
        }
        /// <inheritdoc />
        /// <summary>Attacher l'inspecteur au client</summary>
        /// <param name="endpoint">Représente le noeud final d'un service permettant aux clients du service de rechercher et de communiquer avec le service.</param>
        /// <param name="clientRuntime">Représente le point d'insertion pour les classes qui étendent les fonctionnalités des objets clients Windows Communication Foundation (WCF) pour tous les messages traités par une application cliente.</param>
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Clear();
            clientRuntime.MessageInspectors.Add(this);
        }
    }
}
