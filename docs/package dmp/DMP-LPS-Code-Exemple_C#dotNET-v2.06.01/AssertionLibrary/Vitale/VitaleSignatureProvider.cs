/********************************************************
 *  CopyRight....: GIE SESAM VITALE - 2016              *
 *  solution.....: Formation-TLSi-2016                  *
 *  projet.......: TLSi_AssertionLibrary                *
 *  Summary......: génération d'assertions PS et Vitale *
 *  Date.........: 04 janvier 2016                      *
 ********************************************************/

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using cryptoki;

namespace TLSi_AssertionLibrary
{
    /// <summary>
    /// Fournisseur de signature Vitale
    /// </summary>
    internal class VitaleSignatureProvider : SignatureProvider
    {
        #region membres privés
        string lecteur;
        CarteVitale carteVitale;
        #endregion

        #region constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="lecteur">le nom du lecteur contenant la carte Vitale</param>
        public VitaleSignatureProvider(string lecteur)
        {
            this.lecteur = lecteur;
            this.carteVitale = Mica.Instance.FindByName(lecteur);
        }
        #endregion

        #region propriétés
        /// <summary>
        /// La liste des Ressources Galss possibles pour la carte Vitale
        /// </summary>
        public static string[] LecteursGalss { get; set; }
        /// <summary>
        /// la liste des lecteurs Vitale (même ceux ayant éventuellement une CPS)
        /// </summary>
        internal static List<string> ListeLecteursVitale
        {
            get
            {
                List<Slot> liste = Cryptoki.Instance.GetSlotList(false);
                List<string> vitList = new List<string>();
                for (int i = 0; i < liste.Count; i++) // on recherche les cartes Vitale dans les lecteurs PC/SC
                {
                    if (!liste[i].Info.Description.Contains("PSS Reader")) // la fente CPS du lecteur bifente n'est jamais dispo pour une Vitale
                    {
                        vitList.Add(liste[i].Info.Description);
                    }
                }
                if (LecteursGalss != null)
                {
                    for (int i = 0; i < LecteursGalss.Length; i++) // on recherche les cartes Vitale dans les ressources GALSS
                    {
                        vitList.Add(LecteursGalss[i]);
                    }
                }
                return vitList;
            }
        }
        /// <summary>
        /// le lecteur utilisé pour lire la carte Vitale
        /// </summary>
        public string Lecteur
        {
            get
            {
                return Vitale.Lecteur;
            }
            set
            {
                carteVitale = Mica.Instance.FindByName(value);
                Vitale = carteVitale;
            }
        }
        /// <summary>
        /// réactualisation de la Carte Vitale dans le lecteur
        /// </summary>
        public CarteVitale Vitale
        {
            get
            {
                return carteVitale;
            }

            internal set { }
        }
        #endregion

        /// <summary>
        /// Signature de la carte Vitale
        /// </summary>
        /// <param name="dataToSign">donnée à signer</param>
        /// <returns>la donnée signée</returns>
        protected override string sign(string dataToSign)
        {
            // calcul du SHA256 de la donnée à signer
            SHA256 sha256 = SHA256.Create();
            byte[] data = UTF8Encoding.UTF8.GetBytes(dataToSign);
            byte[] digest = sha256.ComputeHash(data);

            // signature du SHA256
            byte[] signature = Mica.Instance.SignatureVitale(Vitale.Lecteur, digest);
            String signatureB64 = Convert.ToBase64String(signature);
            return signatureB64;
        }

        /// <summary>
        /// Le contenu de l'élément KeyInfo pour la carte Vitale
        /// </summary>
        protected override string KeyInfoContent
        {
            get
            {
                if (Vitale.Info.Type == CarteVitaleType.Unknown)
                {
                    return null;
                }
                StringBuilder keyInfo = new StringBuilder("<KeyInfo xmlns=\"http://www.sesam-vitale.fr/xmldsig/2011/12/vitale\"><Type>");
                if (Vitale.Info.Type == CarteVitaleType.Vitale1_IGEA)
                {
                    keyInfo.Append("V1IGEA");
                }
                if (Vitale.Info.Type == CarteVitaleType.Vitale1_SCOT)
                {
                    keyInfo.Append("V1SCOT");
                }
                if (Vitale.Info.Type == CarteVitaleType.Vitale2)
                {
                    keyInfo.Append("V2");
                }
                keyInfo.Append("</Type><Nature>");
                keyInfo.Append(Vitale.Info.Nature);
                keyInfo.Append("</Nature><NumSerieLogique>");
                keyInfo.Append(Vitale.Serial);
                keyInfo.Append("</NumSerieLogique>");
                if (Vitale.Info.Type == CarteVitaleType.Vitale1_SCOT)
                {
                    keyInfo.Append("<NumFabricant>");
                    keyInfo.Append(Vitale.Info.ManufacturerId);
                    keyInfo.Append("</NumFabricant>");
                }
                if (Vitale.Info.Type == CarteVitaleType.Vitale2)
                {
                    keyInfo.Append("<CleMere>");
                    keyInfo.Append(Vitale.Info.Key);
                    keyInfo.Append("</CleMere>");
                }
                keyInfo.Append("</KeyInfo>");
                return keyInfo.ToString();
            }
        }

        /// <summary>
        /// Restitue l'algorithme de signature de la carte Vitale
        /// </summary>
        internal override string SignatureAlgorithm
        {
            get
            {
                if (Vitale.Info.Type == CarteVitaleType.Vitale1_IGEA)
                    return CarteVitaleSignatureAlgorithms.V1_IGEA_Sha256Signature;
                if (Vitale.Info.Type == CarteVitaleType.Vitale1_SCOT)
                    return CarteVitaleSignatureAlgorithms.V1_SCOT_Sha256Signature;
                if (Vitale.Info.Type == CarteVitaleType.Vitale2)
                    return CarteVitaleSignatureAlgorithms.V2_Sha256Signature;
                return null;
            }
        }
    }
}
