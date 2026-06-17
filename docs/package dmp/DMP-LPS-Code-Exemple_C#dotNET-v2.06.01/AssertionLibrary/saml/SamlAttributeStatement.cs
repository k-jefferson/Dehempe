/********************************************************
 *  CopyRight....: GIE SESAM VITALE - 2016              *
 *  solution.....: Formation-TLSi-2016                  *
 *  projet.......: TLSi_AssertionLibrary                *
 *  Summary......: génération d'assertions PS et Vitale *
 *  Date.........: 04 janvier 2016                      *
 ********************************************************/

using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace TLSi_AssertionLibrary
{
    /// <summary>
    /// Classe de gestion d'un AttributeStatement conforme à la norme SAML
    /// </summary>
    public class SamlAttributeStatement
    {
        // Les fragments de l'attribute Statement
        private static string AS_BEGIN = "<AttributeStatement>";
        private static string AS_END = "</AttributeStatement>";
        // les fragments de l'attribut
        private static string A_BEGIN = "<Attribute Name=\"";
        private static string A_FOLLOWING = "\"><AttributeValue>";
        private static string A_END = "</AttributeValue></Attribute>";

#region Properties
        /// <summary>
        /// AttributeStatement sérialisé
        /// </summary>
        public string SerializedAttributeStatement { get; set; }
#endregion

#region Constructeurs
        /// <summary>
        /// Constructeur de base
        /// </summary>
        public SamlAttributeStatement()
        {
            SerializedAttributeStatement = AS_BEGIN + AS_END;
        }

        /// <summary>
        /// Constructeur intégrant un attribut
        /// </summary>
        /// <param name="attribut">l'attribut à ajouter</param>
        public SamlAttributeStatement(SamlAttribute attribut)
        {
            SerializedAttributeStatement = AS_BEGIN + AS_END;
            addAttribute(attribut);
        }

        /// <summary>
        /// Constructeur intégrant un attribut
        /// </summary>
        /// <param name="attributs">liste d'attributs à ajouter</param>
        public SamlAttributeStatement(List<SamlAttribute> attributs)
        {
            SerializedAttributeStatement = AS_BEGIN + AS_END;
            foreach (SamlAttribute a in attributs)
                addAttribute(a);
        }
#endregion

#region methods
        /// <summary>
        /// Ajoute un attribut à l'AttributeStatement
        /// </summary>
        /// <param name="attribut">attribut à ajouter</param>
        public void addAttribute(SamlAttribute attribut)
        {
            Debug.WriteLine("Ajout attribut {0}, valeur {1}", attribut.Name, attribut.Value);
            StringBuilder asString = new StringBuilder(SerializedAttributeStatement);
            Debug.WriteLine("LastIndexOf(AS_END) = {0}", SerializedAttributeStatement.LastIndexOf(AS_END));
            asString.Insert(SerializedAttributeStatement.LastIndexOf(AS_END),
                                      A_BEGIN + attribut.Name + A_FOLLOWING + attribut.Value + A_END);
            SerializedAttributeStatement = asString.ToString();
            Debug.WriteLine("AttributeStatement ===> {0}", SerializedAttributeStatement);
        }
#endregion
    }
}
