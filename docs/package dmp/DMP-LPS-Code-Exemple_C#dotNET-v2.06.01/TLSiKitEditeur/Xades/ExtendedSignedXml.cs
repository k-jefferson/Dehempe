using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TLSiKitEditeur.Xades
{
    /// <summary>
    /// Implementation qui rajoute le support pour la signature de reference se trouvant dans les noeuds enfants.
    /// </summary>
    /// <remarks>
    /// L'implementation de System.Security.Cryptography.Xml.SignedXml ne permet que de signer des noeuds "object".
    /// Les noeux Manifest devant etre signer, cette classe surcharge les fonctions de recherche d'element en prenant en compte tous les noeuds meme enfant.
    /// Dans le contexte de la signature Xades, la class de base est celle de la librairie de Microsoft gerant les proprietes specifiques Xades.
    /// </remarks>
    class ExtendedSignedXml : Microsoft.Xades.XadesSignedXml
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        public ExtendedSignedXml() : base()
        {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="signatureElement"></param>
        public ExtendedSignedXml(XmlElement signatureElement) : base(signatureElement)
        {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="signatureDocument"></param>
        public ExtendedSignedXml(System.Xml.XmlDocument signatureDocument) : base(signatureDocument)
        {
        }

        /// <summary>
        /// Methode permettant la recherche d'un element a tout niveau de l'arborescence
        /// </summary>
        /// <param name="document">Document</param>
        /// <param name="idValue">Identifiant a chercher</param>
        /// <returns>Element trouver ou null</returns>
        public override XmlElement GetIdElement(XmlDocument document, string idValue)
        {
            XmlElement retVal = base.GetIdElement(document, idValue);
            if (retVal != null)
            {
                return retVal;
            }

            foreach (DataObject containedDocument in m_signature.ObjectList)
            {
                retVal = GetIdElement(containedDocument.Data, idValue);
                if (retVal != null)
                {
                    return retVal;
                }
            }

            return null;
        }

        /// <summary>
        /// Methode permettant la recherche d'un element a tout niveau de l'arborescence
        /// </summary>
        /// <param name="data">Liste de noeud</param>
        /// <param name="idValue">Identifiant a chercher</param>
        /// <returns>Element trouver ou null</returns>
        private XmlElement GetIdElement(XmlNodeList data, string idValue)
        {
            if (data == null)
            {
                return null;
            }

            foreach (XmlNode node in data)
            {
                XmlElement element = GetIdElement(node, idValue);
                if (element != null)
                    return element;
            }

            return null;
        }

        /// <summary>
        /// Methode permettant la recherche d'un element a tout niveau de l'arborescence
        /// </summary>
        /// <param name="node">Noeud</param>
        /// <param name="idValue">Identifiant a chercher</param>
        /// <returns>Element trouver ou null</returns>
        private XmlElement GetIdElement(XmlNode node, string idValue)
        {
            if (node == null)
            {
                return null;
            }

            XmlElement element = node.SelectSingleNode("//*[@Id=\"" + idValue + "\"]") as XmlElement;
            if (element != null)
            {
                return element;
            }

            element = node.SelectSingleNode("//*[@id=\"" + idValue + "\"]") as XmlElement;
            if (element != null)
            {
                return element;
            }

            return (node.SelectSingleNode("//*[@ID=\"" + idValue + "\"]") as XmlElement);
        }
    }
}
