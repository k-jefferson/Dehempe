/********************************************************
 *  CopyRight....: GIE SESAM VITALE - 2016              *
 *  solution.....: Formation-TLSi-2016                  *
 *  projet.......: Client_AATi                          *
 *  Summary......: Accès au TLSi Avis Arret Travail     *
 *  Date.........: 04 janvier 2016                      *
 ********************************************************/

using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Description;
using System.Xml.Linq;
using System.IO;
using System.Xml;
using System.ServiceModel.Channels;

namespace TLSi_AssertionLibrary
{
    /// <summary>
    /// Inpecteur de message devant ajouter les assertions PS et Vitale dans le header Security du message requête du TLSi
    /// </summary>
    public class AssertionMessageInspector : IEndpointBehavior, IClientMessageInspector
    {[Import]
        private AssertionFactory factory;

        #region Constructeur
        /// <summary>
        /// Contructuer de l'inspecteur de message pour les assertions PS et Vitale
        /// </summary>
        /// <param name="f">la fabrique d'assertions</param>
        public AssertionMessageInspector(AssertionFactory f)
        {
            this.factory = f;
            this.AvecVitale = true;
        }

        /// <summary>
        /// Constructeur de l'inspecteur de message pour les assertions
        /// </summary>
        /// <param name="f">la fabrique d'assertions</param>
        /// <param name="vit">l'indicateur de génération d'assertion Vitale</param>
        public AssertionMessageInspector(AssertionFactory f, Boolean vit)
        {
            this.factory = f;
            this.AvecVitale = vit;
        }
        #endregion

        #region Properties
        /// <summary>
        /// indique si une assertion Vitale doit être générée
        /// </summary>
        public Boolean AvecVitale { get; set; }

        /// <summary>
        /// Message requête
        /// </summary>
        public string RequestMessage { get; set; }
        /// <summary>
        /// Message réponse
        /// </summary>
        public string ResponseMessage { get; set; }
        #endregion

        #region IEndpointBehavior Members
        void IEndpointBehavior.AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        void IEndpointBehavior.ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            // adds our inspector to the runtime
            clientRuntime.MessageInspectors.Add(this);
        }

        void IEndpointBehavior.ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        void IEndpointBehavior.Validate(ServiceEndpoint endpoint)
        {
        }
        #endregion

        #region IClientMessageInspector Members
        void IClientMessageInspector.AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            this.ResponseMessage = reply.ToString();
            StreamWriter outfile = new StreamWriter("MessageSOAPRecu.xml");
            outfile.Write(reply.ToString());
            outfile.Flush();
            outfile.Close();
        }

        object IClientMessageInspector.BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
        {
            string msgRequest = request.ToString();

            #region Ajout des assertions PS et Vitale dans l'élément Security
            // Recherche du noeud Security
            XDocument doc = XDocument.Parse(msgRequest);
            XElement root = doc.Root;
            XNamespace ns = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";
            XElement xElementSecurity = doc.Descendants(ns + "Security").First();

            // Création de l'assertion PS signée
            XElement assertionPS = factory.AssertionPS.Root;

            Console.WriteLine("assertion PS OK");
            xElementSecurity.Add(assertionPS);

            // création de l'assertion Vitale
            if (AvecVitale)
            {
                XElement assertionVitale = factory.AssertionVitale.Root;
                Console.WriteLine("assertion Vitale OK");
                xElementSecurity.Add(assertionVitale);
            }
            #endregion

            //Permet de recréér un nouveau message SOAP à partir du message d'origine + 
            //les assertions dans le Header
            MemoryStream ms = new MemoryStream();

            // Attention très important :
            // Il faut Sauvegarde le XML dans un flux avec l'option DisableFormatting
            // pour que le flux ne soit pas modifié (ajout ou suppression d'espace)
            doc.Save(ms, SaveOptions.DisableFormatting);

            // Sauvegarde du document SOAP en entrée
            StreamWriter outfile = new StreamWriter("MessageSOAPEnvoye.xml");
            outfile.Write(doc.ToString());
            outfile.Flush();
            outfile.Close();

            ms.Position = 0;    // Se positionner au début du flux : important
            XmlReader reader = XmlReader.Create(ms);

            request = Message.CreateMessage(reader, int.MaxValue, request.Version);

            this.RequestMessage = request.ToString();

            return null;
        }
        #endregion
    }
}
