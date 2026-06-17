/********************************************************
 *  CopyRight....: GIE SESAM VITALE - 2016              *
 *  solution.....: Formation-TLSi-2016                  *
 *  projet.......: TLSi_AssertionLibrary                *
 *  Summary......: génération d'assertions PS et Vitale *
 *  Date.........: 04 janvier 2016                      *
 ********************************************************/

using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

namespace TLSi_AssertionLibrary
{
    internal class SamlPsAssertion : SamlAssertion
    {
        #region constructeur
        /// <summary>
        /// Constructeur de l'assertion PS
        /// </summary>
        /// <param name="signature">Objet de signature pour l'assertion PS</param>
        internal SamlPsAssertion(PsSignatureProvider signature) : base(signature)
        {
        }
        #endregion

        #region méthodes publiques
        /// <summary>
        /// Construit l'assertion PS
        /// </summary>
        /// <returns>le document XML contenant l'assertion PS</returns>
        internal XDocument Build()
        {
            PsSignatureProvider signProv;
            if (this.SignatureProvider is PsSignatureProvider)
            {
                signProv = (PsSignatureProvider)SignatureProvider;
            }
            else
                throw new AssertionException("L'élément de Signature est incompatible avec la CPS");
            X509Certificate2 certificatCps = signProv.Certificat;
            String cpsType = getCardType(certificatCps);

            return build(true, "urn:oasis:names:tc:SAML:1.1:nameid-format:X509SubjectName",
                         certificatCps.Subject, cpsType,
                         certificatCps.GetNameInfo(X509NameType.SimpleName, false));
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Permet de récupérer le code du type de carte PS dans le certificat de signature
        /// </summary>
        /// <param name="certificate">Certificat de signature de la CPS</param>
        /// <returns>le type de CPS</returns>
        private string getCardType(X509Certificate2 certificate)
        {
            string typeCarte = null;
            try
            {
                X509Extension extension = GetExtensionByOidValue(certificate, "1.2.250.1.71.1.2.2");
                byte[] bytes = extension.RawData;
                if (bytes == null || bytes.Length == 0)
                {
                    throw new Exception("Ext GipCardType non trouvé");
                }
                else if (bytes.Length != 3)
                {
                    throw new Exception("Ext GipCardType invalide");
                }
                else
                {
                    short codeTypeCard = bytes[2];
                    typeCarte = translateCardType(codeTypeCard);
                    if (typeCarte == null)
                        throw new Exception("GipCardType inconnu " + codeTypeCard);
                }
            }
            catch (Exception ex)
            {
                typeCarte = "CPX";
                Debug.WriteLine(ex.Message);
                //Console.ForegroundColor = ConsoleColor.Red;
                //Console.WriteLine(ex.Message);
            }
            return typeCarte;
        }

        /// <summary>
        /// Permet d'obtenir l'extension d'un certificat en fonction de son OID
        /// </summary>
        /// <param name="certificat">Certificat</param>
        /// <param name="oidValue">Valeur de l'OID</param>
        /// <returns>l'extension</returns>
        private X509Extension GetExtensionByOidValue(X509Certificate2 certificat, string oidValue)
        {
            X509Extension extensionFind = null;
            foreach (X509Extension extension in certificat.Extensions)
            {
                if (extension.Oid.Value == oidValue)
                {
                    extensionFind = extension;
                }
            }
            return extensionFind;
        }

        /// <summary>
        /// traduit le code de la carte en type de CPx
        /// </summary>
        /// <param name="codeCard">code de la carte</param>
        /// <returns>Type de carte CPS</returns>
        private string translateCardType(short codeCard)
        {
            string cardType = null;
            switch (codeCard)
            {
                case 0:
                    cardType = "CPS";
                    break;
                case 1:
                    cardType = "CPF";
                    break;
                case 2:
                    cardType = "CDE-CPE";
                    break;
                case 3:
                    cardType = "CPA";
                    break;
            }

            return cardType;
        }
        #endregion
    }
}
