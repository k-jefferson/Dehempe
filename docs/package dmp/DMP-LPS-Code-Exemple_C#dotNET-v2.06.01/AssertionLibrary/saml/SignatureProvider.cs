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
using System.Security.Cryptography;

namespace TLSi_AssertionLibrary
{
    /// <summary>
    /// Classe de base gérant l'élément XMLSignature
    /// </summary>
    public abstract class SignatureProvider
    {
        #region variables statiques privées
        private static String SIGNATURE_BASE = "<Signature xmlns=\"http://www.w3.org/2000/09/xmldsig#\">"
                                             + "$SignedInfo$"
                                             + "<SignatureValue>$SignatureValue$</SignatureValue>"
                                             + "<KeyInfo>$KeyInfo$</KeyInfo>"
                                             + "</Signature>";

        private static String SIGNEDINFO_BASE = "<SignedInfo>"
                                             + "<CanonicalizationMethod Algorithm=\"http://www.w3.org/2001/10/xml-exc-c14n#\"/>"
                                             + "<SignatureMethod Algorithm=\"$SignatureAlgo$\"/>"
                                             + "<Reference URI=\"$URI$\">"
                                             + "<Transforms>"
                                             + "<Transform Algorithm=\"http://www.w3.org/2000/09/xmldsig#enveloped-signature\"/>"
                                             + "<Transform Algorithm=\"http://www.w3.org/2001/10/xml-exc-c14n#\"/>"
                                             + "</Transforms>"
                                             + "<DigestMethod Algorithm=\"http://www.w3.org/2001/04/xmlenc#sha256\"/>"
                                             + "<DigestValue>$DigestValue$</DigestValue>"
                                             + "</Reference></SignedInfo>";

        private static String C14NSIGNEDINFO_BASE = "<SignedInfo xmlns=\"http://www.w3.org/2000/09/xmldsig#\">"
                                             + "<CanonicalizationMethod Algorithm=\"http://www.w3.org/2001/10/xml-exc-c14n#\"></CanonicalizationMethod>"
                                             + "<SignatureMethod Algorithm=\"$SignatureAlgo$\"></SignatureMethod>"
                                             + "<Reference URI=\"$URI$\">"
                                             + "<Transforms>"
                                             + "<Transform Algorithm=\"http://www.w3.org/2000/09/xmldsig#enveloped-signature\"></Transform>"
                                             + "<Transform Algorithm=\"http://www.w3.org/2001/10/xml-exc-c14n#\"></Transform>"
                                             + "</Transforms>"
                                             + "<DigestMethod Algorithm=\"http://www.w3.org/2001/04/xmlenc#sha256\"></DigestMethod>"
                                             + "<DigestValue>$DigestValue$</DigestValue>"
                                             + "</Reference></SignedInfo>";
        #endregion

        #region méthode protégée
        /// <summary>
        /// instancie une XMLSignature pour une assertion
        /// </summary>
        /// <param name="assertion">l'assertion non signée</param>
        /// <returns>l'élément XMLSignature</returns>
        internal String Signature(String assertion)
        {
            HashAlgorithm sha2 = new SHA256CryptoServiceProvider();
            byte[] hash = sha2.ComputeHash(System.Text.Encoding.UTF8.GetBytes(assertion));
            String digestB64 = Convert.ToBase64String(hash);
            Debug.WriteLine("Digest SHA256 = " + digestB64);

            int posStartId = assertion.IndexOf("ID=\"") + 4;
            String referenceUri = assertion.Substring(posStartId, assertion.IndexOf("\" IssueInstant")-posStartId);
            Debug.WriteLine("ID = " + referenceUri);

            // fabrication de l'élément signedInfo à insérer dans l'élément XML Signature
            StringBuilder signedInfo = new StringBuilder(SIGNEDINFO_BASE);
            signedInfo.Replace("$SignatureAlgo$", SignatureAlgorithm);
            signedInfo.Replace("$URI$", "#" + referenceUri);
            signedInfo.Replace("$DigestValue$", digestB64);
            // fabrication de l'élément SignedInfo canonisé C14N à signer par la carte
            StringBuilder c14nSignedInfo = new StringBuilder(C14NSIGNEDINFO_BASE);
            c14nSignedInfo.Replace("$SignatureAlgo$", SignatureAlgorithm);
            c14nSignedInfo.Replace("$URI$", "#" + referenceUri);
            c14nSignedInfo.Replace("$DigestValue$", digestB64);

            // fabrication de l'objet XMLSignature complet
            StringBuilder xmlSignature = new StringBuilder(SIGNATURE_BASE);
            xmlSignature.Replace("$SignedInfo$", signedInfo.ToString());
            xmlSignature.Replace("$SignatureValue$", sign(c14nSignedInfo.ToString()));
            xmlSignature.Replace("$KeyInfo$", KeyInfoContent);
            return xmlSignature.ToString();
        }
        #endregion

        #region méthode vituelle
        /// <summary>
        /// signature d'une donnée
        /// </summary>
        /// <param name="dataToSign">données à signer</param>
        /// <returns></returns>
        abstract protected String sign(String dataToSign); // restitue la signature en Base64
        #endregion

        #region propriétés virtuelles
        /// <summary>
        /// Elément XML des données de clé de la carte
        /// </summary>
        virtual protected String KeyInfoContent { get; set; }
        /// <summary>
        /// la valeur de l'attribut Algorithm de l'élément SignatureMethod
        /// </summary>
        virtual internal String SignatureAlgorithm { get; set; }
        #endregion

    }
}
