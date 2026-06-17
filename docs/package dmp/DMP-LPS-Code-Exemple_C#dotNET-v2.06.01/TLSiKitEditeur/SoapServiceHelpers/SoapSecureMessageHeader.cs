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
using System.ServiceModel.Channels;
using System.Web.Services.Protocols;
using System.Xml;

namespace TLSiKitEditeur.SoapServiceHelpers
{
    /// <summary>
    /// Represents a custom message header.
    /// </summary>
    public class SoapSecureMessageHeader : MessageHeader
    {
        public SoapSecureMessageHeader(SoapUnknownHeader header)
        {
            this.Header = header;
        }

        private SoapUnknownHeader Header { get; set; }

        public override string Name => Header.Element.LocalName;

        public override string Namespace => Header.Element.NamespaceURI;

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            writer.WriteRaw(Header.Element.InnerXml);   
        }

    }
}
