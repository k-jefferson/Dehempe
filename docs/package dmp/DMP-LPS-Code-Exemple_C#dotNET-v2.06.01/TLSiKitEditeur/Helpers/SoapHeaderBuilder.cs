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
using System.Web.Services.Protocols;
using System.Xml;
using TLSiKitEditeur.VIHF;

namespace TLSiKitEditeur.Helpers
{
    public static class SoapHeaderBuilder
    {
        /// <summary>
        /// Fonction construisant un Header complet depuis un VIHF
        /// </summary>
        ///         /// <param name="SignVIHF"></param>
        /// <returns></returns>
        public static SoapUnknownHeader GetHeader( bool SignVIHF)
        {
            // le XMLDocument servant de base pour le header
            XmlDocument soapHeader = new XmlDocument();
            string h = $"<Security xmlns=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\">{VihfProvider.Instance.GetVihf().GetVihfXmlElement(SignVIHF).OuterXml}</Security>";
            soapHeader.LoadXml(h);
           SoapUnknownHeader header = new SoapUnknownHeader {Element = (XmlElement) soapHeader.FirstChild };
            return header;
        }
    }
}
