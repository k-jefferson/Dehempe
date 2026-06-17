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
using System.Xml;

namespace TLSiKitEditeur.VIHF
{
    /// <summary>
    /// Objet PurposeOfUse present dans le jeton VIHF
    /// </summary>
  public  class PurposeOfUse
    {
        string code;
        string codeSystem;
        string codeSystemName;
        string displayName;

        public PurposeOfUse(string code, string codeSystem, string codeSystemName, string displayName)
        {
            this.code = code;
            this.codeSystem = codeSystem;
            this.codeSystemName = codeSystemName;
            this.displayName = displayName;
        }

        /// <summary>
        /// fonction construisant un noeud Xml PurposeOfUse a partir des informations
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public XmlNode getXmlElement(XmlDocument document)
        {
            // construction du noeud
            XmlNode node = document.CreateElement("PurposeOfUse", "urn:hl7-org:v3");

            // creation des attributs
            XmlAttribute code = document.CreateAttribute("code");
            code.Value = this.code;
            XmlAttribute codeSystem = document.CreateAttribute("codeSystem");
            codeSystem.Value = this.codeSystem;
            XmlAttribute codeSystemName = document.CreateAttribute("codeSystemName");
            codeSystemName.Value = this.codeSystemName;
            XmlAttribute displayName = document.CreateAttribute("displayName");
            displayName.Value = this.displayName;
            XmlAttribute type = document.CreateAttribute("xsi", "type", "http://www.w3.org/2001/XMLSchema-instance");
            type.Value= "CE";

            // on associe les attributs au noeud
            node.Attributes.Append(code);
            node.Attributes.Append(codeSystem);
            node.Attributes.Append(codeSystemName);
            node.Attributes.Append(displayName);
            node.Attributes.Append(type);

            // on retourne le noeud
            return node;
        }

        public override string ToString()
        {
            return "<PurposeOfUse xmlns=\"urn:hl7-org:v3\" code=\""+code + "\" codeSystem=\""+codeSystem+"\" codeSystemName=\""+codeSystemName+"\" VIHF 1.0\" displayName=\""+displayName +"\" xsi:type=\"CE\" />";
        }
    }
}
