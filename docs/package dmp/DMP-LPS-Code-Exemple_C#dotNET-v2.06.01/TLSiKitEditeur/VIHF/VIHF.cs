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

using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using TLSiKitEditeur.CertificateHelpers;
using TLSiKitEditeur.Helpers;

namespace TLSiKitEditeur.VIHF
{
  public  class VIHF
    {
        //O = obligatoire F = Facultatif

        //O Version du VIHF utilisée
        const string VIHF_Version = "3.0"; //version 3.0 pour le DMPv2
                
        //O Profession ou future profession de l’utilisateur
        //Format: Code^OID
        public string Profession { get; set; }
                
        //O Secteur d’activité dans lequel exerce l’utilisateur
        public string Secteur_Activite { get; set; }

        //O Ressource visée par l’utilisateur.
        public string Ressource_URN { get; set; }

        public string Ressource_Id { get; set; }

		//F Utilisé pour les mineurs
		public string Confidentiality_code { get; set; }

		//F Indique le mode d’accès demandé par l’utilisateur (normal, bris de glace ou centre 15).
		//Valeur par défaut = normal
		public string Mode_Acces { get; set; }

        //O Identifiant de l’utilisateur effectuant la requête.
        //public string Identifiant_Utilisateur { get; set; }

        //O Identité de l’utilisateur (ex. nom, prénom et/ou service au sein d’un établissement)
        public string Identite_Utilisateur { get; set; }

        //F Identifiant de l’établissement de santé ou du cabinet depuis lequel la requête est émise.
        public string Identifiant_Structure { get; set; }

        //F Nom et version du logiciel utilisé
        public string LPS_Type { get; set; }

        //F Numéro de série ou identifiant de l’installation du logiciel
        public string LPS_ID { get; set; }

        public string LPS_Nom { get; set; }

        public string LPS_ID_HOMOLOGATION_DMP { get; set; }

        public string LPS_Version { get; set; }
        
        public string Issuer { get; set; }
                
        LinkedList<Role> roles;
        public PurposeOfUse purposeOfuse { get; set; }

        public string NameId { get; set; }
        
        public string assertionID { get; set; }

        private XmlDocument assertion;
                
        public VIHF()
        {
            this.assertion = new XmlDocument();
            assertionID = "VIHF-" + UUID.GenerateRandomUuid();
            this.roles = new LinkedList<Role>();
        }

        public VIHF(XmlDocument document)
        {
            this.assertion = document;
            assertionID = "VIHF-" + UUID.GenerateRandomUuid();
            this.roles = new LinkedList<Role>();
        }

        /// <summary>
        /// Ajoute un Role au PS
        /// </summary>
        /// <param name="role"></param>
        public void addRole(Role role)
        {
            this.roles.AddLast(role);
        }

        /// <summary>
        /// Fonction ajoutant des Noeuds Attributs au noeud attributeStatement du jeton VIHF
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="attributeStatement"></param>
        private void addAttributeNode(string name, string value, XmlNode attributeStatement)
        {
            // creation d'un nouveau noeud attribut
            XmlElement attribute = this.assertion.CreateElement( "Attribute", "urn:oasis:names:tc:SAML:2.0:assertion");
                        
            // creation de l'attribut Name
            XmlAttribute attributeName = this.assertion.CreateAttribute("Name");
            attributeName.Value = name;
                        
            attribute.Attributes.Append(attributeName);

            // creation du noeud contenant la valeur associee a l'attribut
            XmlElement attributeValue = this.assertion.CreateElement( "AttributeValue", "urn:oasis:names:tc:SAML:2.0:assertion");
            attributeValue.InnerText = value;
            attribute.AppendChild(attributeValue);

            // attachement du noeud termine au noeud AttributeStatement
            attributeStatement.AppendChild(attribute);
        }

        /// <summary>
        /// Fonction ajoutant des Noeuds Attributs dont la valeur est un noeud egalement au noeud attributeStatement du jeton  VIHF
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="attributeStatement"></param>
        private void addAttributeNode(string name, XmlNode value, XmlNode attributeStatement)
        {
            // creation d'un nouveau noeud attribut
            XmlElement attribute = this.assertion.CreateElement("Attribute", "urn:oasis:names:tc:SAML:2.0:assertion");

            // creation de l'attribut Name
            XmlAttribute attributeName = this.assertion.CreateAttribute("Name");
            attributeName.Value = name;

            attribute.Attributes.Append(attributeName);

            // creation du noeud contenant la valeur associee a l'attribut
            XmlElement attributeValue = this.assertion.CreateElement("AttributeValue", "urn:oasis:names:tc:SAML:2.0:assertion");
            // ajout du noeud valeur
            attributeValue.AppendChild(value);
            attribute.AppendChild(attributeValue);

            attributeStatement.AppendChild(attribute);
        }

        private void addAttributeNode(string name, XmlNode[] values, XmlNode attributeStatement)
        {
            // creation d'un nouveau noeud attribut
            XmlElement attribute = this.assertion.CreateElement( "Attribute", "urn:oasis:names:tc:SAML:2.0:assertion");

            // creation de l'attribut Name
            XmlAttribute attributeName = this.assertion.CreateAttribute("Name");
            attributeName.Value = name;

            attribute.Attributes.Append(attributeName);

            // creation du noeud contenant la valeur associee a l'attribut
            foreach (XmlNode value in values)
            {
                XmlElement attributeValue = this.assertion.CreateElement( "AttributeValue", "urn:oasis:names:tc:SAML:2.0:assertion");
                // ajout du noeud valeur
                attributeValue.AppendChild(value);
                attribute.AppendChild(attributeValue);

                attributeStatement.AppendChild(attribute);
            }
        }

        public static string VihfBase()
        {
            var today = System.String.Format("{0:yyyy-MM-ddTHH:mm:ss.fff}Z",System.DateTime.UtcNow);
            StringBuilder tmp = new StringBuilder($"<Assertion xmlns=\"urn:oasis:names:tc:SAML:2.0:assertion\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" ID=\"\" IssueInstant=\"{today}\" Version=\"2.0\"><Issuer Format=\"urn:oasis:names:tc:SAML:1.1:nameid-format:X509SubjectName\">CN=Certificat Wizard DMP 2019-R2, OU=10B0167011,S=Rhône (69),O=CENTRE DE SANTE RPPS12718,C=FR</Issuer><Subject><NameID>10B0011797/00B1022322</NameID></Subject><AuthnStatement AuthnInstant=\"{today}\"><AuthnContext><AuthnContextClassRef>");
            if (Dmp.UseCPS)
            {
                tmp.Append("urn:oasis:names:tc:SAML:2.0:ac:classes:SmartcardPKI");
                tmp.Append("</AuthnContextClassRef>");
            }
            else
            {
                tmp.Append("urn:oasis:names:tc:SAML:2.0:ac:classes:Password");
                tmp.Append("</AuthnContextClassRef>");
                if (Dmp.UseAIR)
                {
                    tmp.Append("<AuthnContextDecl>APP_BROWSER_AUTH</AuthnContextDecl>");
                }
            }
            tmp.Append("</AuthnContext></AuthnStatement><AttributeStatement></AttributeStatement></Assertion>");
            return tmp.ToString();
        }

        /// <summary>
        /// Fonction permettant d'obtenir le jeton VIHF sous forme d'un noeud XML
        /// </summary>
        /// <param name="signatureCertificate"></param>
        /// <param name="signVihf"></param>
        /// <returns></returns>
        public XmlElement GetVihfXmlElement( bool signVihf)
        {
            // chargement du template de base
            this.assertion.LoadXml(VIHF.VihfBase());
          //var today = System.String.Format("{0:yyyy-MM-ddTHH:mm:ss}Z", System.DateTime.UtcNow
              //);
         //   var today = Time.GetActualTime();
            // on place toutes les valeurs du VIHF dans les noeuds correspondants
            //this.assertion.DocumentElement.Attributes.GetNamedItem("IssueInstant").Value = today;
            this.assertion.GetElementsByTagName("Issuer").Item(0).InnerText = Issuer;
            this.assertion.GetElementsByTagName("Assertion").Item(0).Attributes.GetNamedItem("ID").Value = this.assertionID;
            this.assertion.GetElementsByTagName("NameID").Item(0).InnerText = this.NameId;
            // recuperation du noeud attributeStatement dans lequel on va placer tous les attributs
            XmlNode attributeStatement = this.assertion.GetElementsByTagName("AttributeStatement").Item(0);
            // on place tous les attributs qui ont ete definis
            this.addAttributeNode("VIHF_Version", VIHF_Version.ToString(), attributeStatement);
            if (Dmp.UseAIR&&!Dmp.UseCPS)
            {
                this.addAttributeNode("Authentification_Mode", "INDIRECTE_RENFORCEE", attributeStatement);
            }

            if (Profession != null) this.addAttributeNode("Profession", Profession, attributeStatement);
						if (Confidentiality_code != null) this.addAttributeNode("Confidentiality_code", Confidentiality_code, attributeStatement);
						if (Secteur_Activite != null) this.addAttributeNode("Secteur_Activite", Secteur_Activite, attributeStatement);
            if (Ressource_Id != null) this.addAttributeNode("urn:oasis:names:tc:xacml:2.0:resource:resource-id", Ressource_Id, attributeStatement);
            if (Ressource_URN != null) this.addAttributeNode("Ressource_URN", Ressource_URN, attributeStatement);
            if (Mode_Acces != null) this.addAttributeNode("Mode_Acces", Mode_Acces, attributeStatement);
            if (Identite_Utilisateur != null) this.addAttributeNode("urn:oasis:names:tc:xspa:1.0:subject:subject-id", Identite_Utilisateur, attributeStatement);
            if (Identifiant_Structure != null) this.addAttributeNode("Identifiant_Structure", Identifiant_Structure, attributeStatement);
            if (LPS_Version != null) this.addAttributeNode("LPS_Version", LPS_Version, attributeStatement);
            if (LPS_Type != null) this.addAttributeNode("LPS_Type", LPS_Type, attributeStatement);
            if (LPS_ID != null) this.addAttributeNode("LPS_ID", LPS_ID, attributeStatement);
            if (LPS_Nom != null) this.addAttributeNode("LPS_Nom", LPS_Nom, attributeStatement);
            if (LPS_ID_HOMOLOGATION_DMP != null) this.addAttributeNode("LPS_ID_HOMOLOGATION_DMP", LPS_ID_HOMOLOGATION_DMP, attributeStatement);
            if (purposeOfuse != null) this.addAttributeNode("urn:oasis:names:tc:xspa:1.0:subject:purposeofuse", purposeOfuse.getXmlElement(this.assertion), attributeStatement);
            
            // on ajoute tous les roles
            XmlNode[] RolesNodes = new XmlNode[roles.Count];
            int i = 0;
            foreach (Role role in roles)
                RolesNodes[i++] = role.getXmlElement(this.assertion);
            this.addAttributeNode("urn:oasis:names:tc:xacml:2.0:subject:role", RolesNodes, attributeStatement);

            // insertion de la signature Xml du jeton VIHF
            if (signVihf)
            {
                XmlElement signature = this.getXmlSignature();
                                
                this.assertion.DocumentElement.InsertAfter(signature, this.assertion.GetElementsByTagName("Issuer").Item(0));
            }  
            return this.assertion.DocumentElement;
        }

        /// <summary>
        /// Fonction construisant la signature enveloppee du jeton VIHF
        /// </summary>
        /// <returns></returns>
        private XmlElement getXmlSignature()
        {
            //Program.ts.TraceEvent(TraceEventType.Information, 0, DateTime.Now.ToLongTimeString() + " Signature du VIHF");
            // on utilise un objet SignedXml capable de generer des signatures
            SignedXml signature = new SignedXml(this.assertion);

           // Program.ts.TraceEvent(TraceEventType.Information, 0, DateTime.Now.ToLongTimeString() + " XML à signer : " + this.assertion.OuterXml);

            // Creation de la reference vers l'assertion Saml (le jeton VIHF)
            Reference assertionReference = new Reference("#" + this.assertionID);

          //  Program.ts.TraceEvent(TraceEventType.Information, 0, DateTime.Now.ToLongTimeString() + " création référence vers " + this.assertionID);

            // ajout de la reference a la signature
            signature.AddReference(assertionReference);

          //  Program.ts.TraceEvent(TraceEventType.Information, 0, DateTime.Now.ToLongTimeString() + " référence ajoutée vers " + this.assertionID);

            // Creation des transformation propres au SignedInfo de la signature
            XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
            assertionReference.AddTransform(env);

            XmlDsigExcC14NTransform transform = new XmlDsigExcC14NTransform("ds saml #default xsi");
            assertionReference.AddTransform(transform);

            // on definit la cle de signature
            signature.SigningKey = CertificateHelper.SignatureCertificate.PrivateKey;

            // on definit la methode de canonisation
            signature.SignedInfo.CanonicalizationMethod = "http://www.w3.org/2001/10/xml-exc-c14n#";

            // creation de la balise KeyInfo
            KeyInfo keyInfo = new KeyInfo();
            KeyInfoX509Data x509Data = new KeyInfoX509Data(CertificateHelper.SignatureCertificate);
            keyInfo.AddClause(x509Data);

            // on rattache la balise a la signature
            signature.KeyInfo = keyInfo;

            // on calcule la signature
            signature.ComputeSignature();

           // Program.ts.TraceEvent(TraceEventType.Information, 0, DateTime.Now.ToLongTimeString() + " FIN Signature du VIHF");

            return signature.GetXml();
        }        
    }
}
