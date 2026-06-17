/********************************************************
 *  CopyRight....: GIE SESAM VITALE - 2016              *
 *  solution.....: Formation-TLSi-2016                  *
 *  projet.......: TLSi_AssertionLibrary                *
 *  Summary......: génération d'assertions PS et Vitale *
 *  Date.........: 04 janvier 2016                      *
 ********************************************************/

using System;
using System.Xml.Linq;

namespace TLSi_AssertionLibrary
{
    internal class SamlVitaleAssertion : SamlAssertion
    {
        #region constructeur
        /// <summary>
        /// Constructeur de l'assertion PS
        /// </summary>
        /// <param name="signature">Objet de signature pour l'assertion PS</param>
        internal SamlVitaleAssertion(VitaleSignatureProvider signature)
            : base(signature)
        {
        }
        #endregion

        #region propriétés
        /// <summary>
        /// L'Issuer de l'assertion Vitale
        /// </summary>
        public String Issuer { get; set; }
        #endregion

        #region méthodes publiques
        /// <summary>
        /// Construit l'assertion Vitale
        /// </summary>
        /// <param name="avecSignature">indique si l'assertion doit être signée ou non</param>
        /// <returns>le document XML contenant l'assertion Vitale</returns>
        internal XDocument Build(Boolean avecSignature)
        {
            VitaleSignatureProvider signProv;
            if (this.SignatureProvider is VitaleSignatureProvider)
            {
                signProv = (VitaleSignatureProvider)SignatureProvider;
            }
            else
                throw new AssertionException("L'élément de Signature est incompatible avec la carte Vitale");

            return build(avecSignature, null, Issuer, "VITALE", signProv.Vitale.Serial);
        }
        #endregion
    }
}
