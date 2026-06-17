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
using System.Net;
using System.ServiceModel;
using System.Web.Services.Protocols;
using System.Xml;
using TLSiKitEditeur.CertificateHelpers;
using TLSiKitEditeur.Helpers;
using TLSiKitEditeur.VIHF;

namespace TLSiKitEditeur.SoapServiceHelpers
{
    public static class SoapServiceInitializer
    {
        public static SoapUnknownHeader header;
        /// <summary>Initialise le service soap et le protocole sécurité! c'est une extension générique qui peut étre utilisée par tous les classes qui héritent de ClientBase</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client">Client service</param>
        /// <param name="endpointPath">Addresse du endpoint</param>
        public static void Init<T>(this ClientBase<T> client, string endpointPath) where T : class
        {
            EndpointAddressBuilder ab = new EndpointAddressBuilder();
            ab.Uri = new Uri(Dmp.SI_DMP_DOMAIN + endpointPath);
            ab.Identity = client.Endpoint.Address.Identity;

            client.Endpoint.Address = ab.ToEndpointAddress();
            // si un certificat est present et que l'adresse est https
            if (CertificateHelper.AuthCertificate != null &&
                client.Endpoint.Address.Uri.AbsoluteUri.StartsWith("https"))
            {
                // protocole TLS
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                            // on l'ajoute pour etablir la connexion TLS
                if (client.ClientCredentials != null)
                    client.ClientCredentials.ClientCertificate.Certificate = CertificateHelper.AuthCertificate;
               

            }
        }

        /// <summary>Ajout des headers, ! c'est une extension générique qui peut étre utilisée par tous les classes qui héritent de ClientBase</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client">Client service</param>
        /// <param name="action">action soap</param>
        /// <param name="vihf">vihf</param>
        public static void AddHeaders<T>(this ClientBase<T> client, string action)
            where T : class
        {

            header = SoapHeaderBuilder.GetHeader(!Dmp.UseCPS);
            client.Endpoint.Behaviors.RemoveAll<ClientMessageInspector>();
            client.Endpoint.Behaviors.Add(new ClientMessageInspector());
           
        }
    }
}