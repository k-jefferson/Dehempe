using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using System.IdentityModel.Tokens;

namespace TLSi_AssertionLibrary.structure
{
    class StructureAssertion 
    {
        private string cheminConf = "";
        public StructureAssertion(string cheminConf)
        {
            this.cheminConf = cheminConf;
        }
        
        /// <summary>
        /// cherche les informations de la convention
        /// </summary>
        /// <returns>un dictionnaire contenant les infos </returns>
        public Dictionary<string, string> structVal(XmlDocument xmlConv)
        {
            var MyIni = new INIFile(cheminConf);

            Dictionary<string, string> retourIEnum = new Dictionary<string, string>();
            string Issuer1, Issuer2;

            // Trouver entityID de IDPSSODescriptor
            XmlNode NodeIDPSSODescriptor = xmlConv.SelectSingleNode("//*[local-name()='IDPSSODescriptor']");
            if (NodeIDPSSODescriptor.Attributes["entityID"] == null) throw new Exception("L'attribut entityID de IDPSSODescriptor n'existe pas ou mal formaté");
            else
            {
                Issuer1 = NodeIDPSSODescriptor.Attributes["entityID"].Value.ToString();
            }

            // Trouver  version de Convention
            XmlNode NodeConvention = xmlConv.SelectSingleNode("//*[local-name()='Convention']");
            if (NodeConvention.Attributes["version"] == null) throw new Exception("L'attribut version de Convention n'existe pas ou mal formaté");
            else
            {
                Issuer2 = NodeConvention.Attributes["version"].Value.ToString();
            }
            retourIEnum.Add("Issuer", Issuer1 + ":" + Issuer2);

            // trouver SubjectFormat2 qui est NameIDFormat
            XmlNode NodeNameIDFormat = xmlConv.SelectSingleNode("//*[local-name()='NameIDFormat']");
            if (NodeNameIDFormat == null) throw new Exception("Balise NameIDFormat n'existe pas ou mal formaté");  // si elle existe si non exception
            else
            {
                retourIEnum.Add("SubjectFormat2", NodeNameIDFormat.InnerText.ToString());
            }


            // Trouver Recipient qui est entityID de SPSSODescriptor
            XmlNode NodeSPSSODescriptor = xmlConv.SelectSingleNode("//*[local-name()='SPSSODescriptor']");
            if (NodeSPSSODescriptor.Attributes["entityID"] == null) throw new Exception("L'attribut entityID de SPSSODescriptor n'existe pas ou mal formaté");
            else
            {
                retourIEnum.Add("recipient", NodeSPSSODescriptor.Attributes["entityID"].Value.ToString());
            }


            // Trouver Audience qui est serviceURI de ServiceBinding
            XmlNode NodeServiceBinding = xmlConv.SelectSingleNode("//*[local-name()='ServiceBinding']");
            if (NodeServiceBinding.Attributes["serviceURI"] == null) throw new Exception("L'attribut serviceURI de ServiceBinding n'existe pas ou mal formaté");
            else
            {
                retourIEnum.Add("Audience", NodeServiceBinding.Attributes["serviceURI"].Value.ToString());
            }


            ////<AuthnContextClassRef> contient la méthode d’authentification de l’agent  liste d’éléments XML< AccessAuthentication > de la convention
            XmlNodeList NodeAccessAuthentication = xmlConv.SelectNodes("//*[local-name()='AccessAuthentication']");
            if (NodeAccessAuthentication[0] == null) throw new Exception("AccessAuthentication n'existe pas ou mal formaté");
            else
            {
                retourIEnum.Add("AccessAuthentication", NodeAccessAuthentication[0].InnerText);
            }

            // Trouver PAGM (premier élément) 
            XmlNodeList NodePAGM = xmlConv.SelectNodes("//*[local-name()='RequestedAttribute' and @name='PAGM']");
            if (NodePAGM[0] == null) throw new Exception("NodePAGM n'existe pas ou mal formaté");
            else
            {
                retourIEnum.Add("NodePAGM", NodePAGM[0].FirstChild.InnerText);
            }
            //Definir l'agent qui a envoyé l'assertion
            retourIEnum.Add("agentTlsi", MyIni.Read("sec", "nameId"));

            //identifiantFacturation (facultatif)
            retourIEnum.Add("identifiantFacturation", MyIni.Read("sec", "idFacturation"));

            //identifiant du PS délégant.(facultatif)
            retourIEnum.Add("IdPsDelegue", MyIni.Read("sec", "idDeleguant"));

            return retourIEnum;
        }
        /// <summary>
        /// genère l'assertion et la signe en utilisant la fonction SignXmlDoc()
        /// </summary>
        /// <param name="voir">dictionnaire contenant les infos necessaires</param>
        public XmlDocument strucBuilder(Dictionary<string, string> voir)
        {
            var MyIni = new INIFile(cheminConf);
            // Chargement du certificat
            if (string.IsNullOrEmpty(MyIni.Read("Certificat", "Certificat")))
                throw new Exception("Chemin de certificat invalide;");
            X509Certificate2 cert = new X509Certificate2(MyIni.Read("Certificat", "Certificat"), "", X509KeyStorageFlags.Exportable);

            //Création de Issuer
            Saml2NameIdentifier saml2NameIdentifier = new Saml2NameIdentifier(voir["SubjectFormat2"]);

            //Instance Assertion
            Saml2Assertion saml2Assertion2 = new Saml2Assertion(saml2NameIdentifier);
            //saml2Assertion2.IssueInstant = DateTime.Now.AddHours(1);

            //Création de  "Subject"
            Saml2Subject saml2Subject = new Saml2Subject();

            //NameID de Subject
            saml2NameIdentifier = new Saml2NameIdentifier(voir["agentTlsi"], new Uri(voir["SubjectFormat2"]));
            saml2Subject.NameId = saml2NameIdentifier;

            //Création de "SubjectConfirmation"
            Saml2SubjectConfirmationData subjectConfirmationData = new Saml2SubjectConfirmationData();
            Saml2SubjectConfirmation subjectConfirmation = new Saml2SubjectConfirmation(new Uri("urn:oasis:names:tc:SAML:2.0:cm:sender-vouches"));
            subjectConfirmation.SubjectConfirmationData = subjectConfirmationData;

            //Ajout de entityID de SPPSSODescriptor
            subjectConfirmationData.Recipient = new Uri(voir["recipient"]);

            //subjectConfirmationData 7jours et 5 minutes dérive des horloges des systèmes 
            subjectConfirmationData.NotOnOrAfter = saml2Assertion2.IssueInstant.AddDays(7).AddMinutes(5);

            saml2Subject.SubjectConfirmations.Add(subjectConfirmation);
            saml2Assertion2.Subject = saml2Subject;

            //Création de "Conditions"
            Saml2Conditions saml2Conditions = new Saml2Conditions()
            {
                NotBefore = saml2Assertion2.IssueInstant, // date de création
                NotOnOrAfter = subjectConfirmationData.NotOnOrAfter, //doit être égal à la valeur de l’élémentXML < SubjectConfirmationData@NotOnOrAfter >de la convention

            };
            //attribut XML serviceURI de l’élément XML <ServiceBinding>de la convention
            Saml2AudienceRestriction saml2AudienceRestriction = new Saml2AudienceRestriction(new Uri(voir["Audience"]));

            saml2Conditions.AudienceRestrictions.Add(saml2AudienceRestriction);
            saml2Assertion2.Conditions = saml2Conditions;
            //<AuthnContextClassRef> contient la méthode d’authentification de l’agent  liste d’éléments XML< AccessAuthentication >de la convention
            Saml2AuthenticationContext saml2AuthCtxt = new Saml2AuthenticationContext(new Uri(voir["AccessAuthentication"]));
            Saml2AuthenticationStatement saml2AuthStatement = new Saml2AuthenticationStatement(saml2AuthCtxt);

            //Cet élément XML contient l’identifiant de l’assertion SAML. Il est donc identique à l’attribut Assertion@ID de la convention
            saml2AuthStatement.SessionIndex = saml2Assertion2.Id.ToString();
            saml2AuthStatement.AuthenticationInstant = saml2Assertion2.IssueInstant;

            saml2Assertion2.Statements.Add(saml2AuthStatement);
            Saml2AttributeStatement saml2AttStatement = new Saml2AttributeStatement();

            // PAGM
            Saml2Attribute saml2AttributePagm = new Saml2Attribute("PAGM", voir["NodePAGM"]);
            Debug.WriteLine(DateTime.Now);


            saml2AttStatement.Attributes.Add(saml2AttributePagm);

            //identifiantFacturation
            if (voir.ContainsKey("identifiantFacturation"))
            {
                Saml2Attribute saml2AttributeIF = new Saml2Attribute("identifiantFacturation", voir["identifiantFacturation"]);
                saml2AttStatement.Attributes.Add(saml2AttributeIF);
            }


            // "Identifiant PS";
            if (voir.ContainsKey("IdPsDelegue"))
            {
                Saml2Attribute saml2AttributeID = new Saml2Attribute("identifiantDeleguant", voir["IdPsDelegue"]);
                saml2AttStatement.Attributes.Add(saml2AttributeID);
            }

            saml2Assertion2.Statements.Add(saml2AttStatement);
            Saml2SecurityToken samlToken2 = new Saml2SecurityToken(saml2Assertion2);

            //extraire l'assertion
            string ret = DupmToken(samlToken2);

            XmlDocument testassertsign = new XmlDocument();
            testassertsign.PreserveWhitespace = false;
            XmlDocument assertionStructure = new XmlDocument();
            assertionStructure.PreserveWhitespace = false;
            testassertsign.LoadXml(ret);
            // on signe l'assertion 
            assertionStructure = SignXmlDoc(testassertsign, cert, saml2Assertion2.Id.ToString());
            
            return assertionStructure;

        }
        /// <summary>
        /// extraire l'asertion  
        /// </summary>
        /// <param name="token">token comportant l'assertionSaml2</param>
        /// <returns>chaine de caractère représente la sortie</returns>
        public static string DupmToken(Saml2SecurityToken token)
        {
            var handler = new Saml2SecurityTokenHandler();
            var sw = new StringWriter();
            handler.WriteToken(new MyXmlWriter(sw), token);
            return (sw.ToString());
        }
        /// <summary>
        /// Fonction qui signe un document xml , elle renvoie un XmlDocument signé si le 
        /// document d'entré comporte une balise "Signature" si non elle renvoie que la signature.
        /// </summary>
        /// <param name="doc">Document a signé.</param>
        /// <param name="cert">Certificat comportant la clé privée.</param>
        /// <param name="id">l'Id de la balise à signé</param>
        /// <returns>XmlDocument</returns>
        public static XmlDocument SignXmlDoc(XmlDocument doc, X509Certificate2 cert, string id)
        {

            XmlNode parentPos = doc.FirstChild;

            //definiftion des methodes de hash , canoni, sign definies dans le cahier des charges
            string signatureCanonicalizationMethod = "http://www.w3.org/2001/10/xml-exc-c14n#";
            string signatureMethod = @"http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";
            string digestMethod = @"http://www.w3.org/2001/04/xmlenc#sha256";

            //utilisation de RSA SHA256            
            CryptoConfig.AddAlgorithm(typeof(RSAPKCS11SHA256SignatureDescription), @"http://www.w3.org/2001/04/xmldsig-more#rsa-sha256");


            // Génération de CSP qui va effectuer les calculs de chiffrement
            var cspParams = new CspParameters(24) { KeyContainerName = "XML_DISG_RSA_KEY" };

            // Génération de la clé de chiff et de déchiff asymétrique
            var key = new RSACryptoServiceProvider(cspParams);

            // Chargement de la clé privée du certif
            key.FromXmlString(cert.PrivateKey.ToXmlString(true));

            // Creation de l'instance SignedXml object.
            SignedXml signedXml = new SignedXml(doc);

            // Création de la balise keyInfo
            signedXml.KeyInfo = new KeyInfo();

            // Affectation de la valeur d'un identificateur global unique à la KeyInfo
            signedXml.KeyInfo.Id = "_" + Guid.NewGuid().ToString();

            // Ajout de la clé public à la balise  ( pour le verification)
            signedXml.KeyInfo.AddClause(new KeyInfoX509Data(cert));

            // Ajout de la clé privée pour signer le signedxml
            signedXml.SigningKey = key;

            // Définition de la méthode de canonicalisation
            signedXml.SignedInfo.CanonicalizationMethod = signatureCanonicalizationMethod;

            // Déclaration de la méthode de signature
            signedXml.SignedInfo.SignatureMethod = signatureMethod;

            // Création d'une référence du doc métier pour etre signée.
            Reference ref_Metier = new Reference();

            // URI référence = Id de balise à signer
            ref_Metier.Uri = "#" + id;

            // AJout de la transformation enveloppée ( envelopped transformation)
            XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform(true);
            //ref_Metier.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            ref_Metier.AddTransform(env);
            ref_Metier.AddTransform(new XmlDsigExcC14NTransform());
            ref_Metier.DigestMethod = digestMethod;

            // Ajout de la référence a l'objet SignedXml.
            signedXml.AddReference(ref_Metier);

            // Calcul de la signature.
            signedXml.ComputeSignature();

            // Extraction du format XML de la signature et la convertir en XmlElement
            XmlElement xmlDigitalSignature = signedXml.GetXml();




            //parentPos.AppendChild(doc.ImportNode(xmlDigitalSignature, true));
            parentPos.InsertAfter(xmlDigitalSignature, parentPos.FirstChild);
            System.Diagnostics.Debug.WriteLine("Fin de la signature ( XML + Signature).");
            return doc;


        }
    }
}
