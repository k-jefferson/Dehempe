/********************************************************
 *  CopyRight....: GIE SESAM VITALE - 2016              *
 *  solution.....: Formation-TLSi-2016                  *
 *  projet.......: TLSi_AssertionLibrary                *
 *  Summary......: génération d'assertions PS et Vitale *
 *  Date.........: 04 janvier 2016                      *
 ********************************************************/

namespace TLSi_AssertionLibrary
{
    /// <summary>
    /// Classe de gestion d'un attribut SAML
    /// </summary>
    public class SamlAttribute
    {
#region Properties
        /// <summary>
        /// Le nom de l'attribut
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// La valeur de l'attribut
        /// </summary>
        public string Value { get; set; }
#endregion

#region Constructeurs
        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public SamlAttribute()
        {
            Name = null;
            Value = null;
        }

        /// <summary>
        /// Contructeur à partir des caractéristiques de l'attribut
        /// </summary>
        /// <param name="name">nom de l'attribut</param>
        /// <param name="value">valeur de l'attribut</param>
        public SamlAttribute(string name, string value)
        {
            Name = name;
            Value = value;
        }
#endregion
    }
}
