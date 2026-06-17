/********************************************************
 *  CopyRight....: GIE SESAM VITALE - 2016              *
 *  solution.....: Formation-TLSi-2016                  *
 *  projet.......: TLSi_AssertionLibrary                *
 *  Summary......: génération d'assertions PS et Vitale *
 *  Date.........: 04 janvier 2016                      *
 ********************************************************/

using System;
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;

namespace TLSi_AssertionLibrary
{
    internal abstract class SamlAssertion
    {
        // squelette d'une assertion non signée
        private static String ASSERTION_BASE =
              "<Assertion xmlns=\"urn:oasis:names:tc:SAML:2.0:assertion\" ID=\"$ID$\" IssueInstant=\"$IssueInstant$\" Version=\"2.0\">"
            + "<Issuer>$Issuer$</Issuer>"
            + "<Subject><NameID>$NameID$</NameID></Subject></Assertion>";

#region propriété
        /// <summary>
        /// l'élément de signature instancié pour l'assertion
        /// </summary>
        internal SignatureProvider SignatureProvider {get; set; }
#endregion

#region Constructeurs
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="signProv">objet de signature utilisé pour cette assertion</param>
        protected SamlAssertion(SignatureProvider signProv)
        {
            if (signProv == null)
                throw new AssertionException("Elément Signature nul");
            this.SignatureProvider = signProv;
            this.AttributeStatement = null;
        }
#endregion

#region Properties
        /// <summary>
        /// l'AttributeStatement à ajouter aux prochaines générations de l'assertion PS
        /// </summary>
        public SamlAttributeStatement AttributeStatement
        {
            get;
            set;
        }
#endregion

#region public methods
        /// <summary>
        /// Permet de créer une assertion
        /// </summary>
        /// <param name="avecSignature">indique si l'assertion doit être signée ou non</param>
        /// <param name="issuerFormat">Attribut issuerFormat de l'élément Issuer (pas d'attribut si null)</param>
        /// <param name="issuer">élément Issuer</param>
        /// <param name="nameQualifier">Attribut nameQualifier de l'élément Subject (pas d'attribut si null)</param>
        /// <param name="nameID">élément NameID</param>
        /// <returns>l'élément du XDocument créé</returns>
        protected XDocument build(Boolean avecSignature, String issuerFormat, String issuer, String nameQualifier, String nameID)
        {
            try
            {
                StringBuilder assert = new StringBuilder(ASSERTION_BASE);
                // valorisation de l'attribut ID
                assert.Replace("$ID$", "_" + Guid.NewGuid().ToString());
                // Valorisation de l'attribut IssueInstant
                assert.Replace("$IssueInstant$", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
                // IssuerFormat si présent
                if (issuerFormat != null)
                {
                    assert.Replace("<issuer>", "<Issuer IssuerFormat=\"" + issuerFormat + "\">");
                }
                // Valorisation de l'Issuer dans l'assertion
                assert.Replace("$Issuer$", issuer);
                // Valorisation attribut NameQualifier si présent
                if (nameQualifier != null)
                {
                    assert.Replace("<NameID>", "<NameID NameQualifier=\"" + nameQualifier + "\">");
                }
                // ajout NameID avec le n° du PS
                assert.Replace("$NameID$", nameID);
				// ajout des attributs saml le cas échéant
                if (AttributeStatement != null)
                    assert.Replace("</Assertion>", AttributeStatement.SerializedAttributeStatement + "</Assertion>");

                // insertion de l'objet XMLSignature dans l'assertion
                if (avecSignature)
                {
                    assert.Replace("<Subject>", SignatureProvider.Signature(assert.ToString()) + "<Subject>");
                }

                Debug.WriteLine("Assertion signee = " + assert.ToString());
                // sauvegarde de l'assertion dans assertion.xml
                //File.WriteAllBytes("assertion.xml", Encoding.UTF8.GetBytes(assertionSignee));
                return XDocument.Parse(assert.ToString());
            }
            catch (Exception e)
            {
                throw new AssertionException(e.Message);
            }
        }
#endregion
    }
}
