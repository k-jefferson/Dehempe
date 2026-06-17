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
    /// Objet Role present dans le jeton VIHF
    /// </summary>
   public class Role
    {
        string code;
        string codeSystem;
        string displayName;
        string codeSystemName;

        public Role(string code, string codeSystem, string displayName,string codeSystemName)
        {
            this.code = code;
            this.codeSystem = codeSystem;
            this.displayName = displayName;
            this.codeSystemName = codeSystemName;
        }

        public Role(string code, string codeSystem, string displayName)
        {
            this.code = code;
            this.codeSystem = codeSystem;
            this.displayName = displayName;
        }

        /// <summary>
        /// fonction construisant un noeud Xml Role a partir des informations
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public XmlNode getXmlElement(XmlDocument document)
        {
            // construction du noeud
            XmlNode node = document.CreateElement("Role", "urn:hl7-org:v3");

            // creation des attributs
            XmlAttribute code = document.CreateAttribute("code");
            code.Value = this.code;
            XmlAttribute codeSystem = document.CreateAttribute("codeSystem");
            codeSystem.Value = this.codeSystem;
            XmlAttribute displayName = document.CreateAttribute("displayName");
            displayName.Value = this.displayName;
            XmlAttribute codeSystemName = document.CreateAttribute("codeSystemName");
            codeSystemName.Value = this.codeSystemName;

            XmlAttribute type = document.CreateAttribute("xsi", "type", "http://www.w3.org/2001/XMLSchema-instance");
            type.Value = "CE";

            // on associe les attributs au noeud
            node.Attributes.Append(code);
            node.Attributes.Append(codeSystem);
            if(!string.IsNullOrEmpty(this.codeSystemName))
            node.Attributes.Append(codeSystemName);
            node.Attributes.Append(displayName);
            node.Attributes.Append(type);

            // on retourne le noeud
            return node;
        }

        public override string ToString()
        {
            var str = "";
            if (string.IsNullOrEmpty(this.codeSystemName))
                str= "<Role xmlns=\"urn:hl7-org:v3\" code=\"" + code + "\" codeSystem=\"" + codeSystem + "\" displayName=\"" + displayName + "\" xsi:type=\"CE\" />";
            else
                str = "<Role xmlns=\"urn:hl7-org:v3\" code=\"" + code + "\" codeSystem=\"" + codeSystem  +"\" codeSystemName=\"" + codeSystemName + "\" displayName=\"" + displayName + "\" xsi:type=\"CE\" />";
            return str;
        }
    }
}
