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
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xades;
using TLSiKitEditeur.DocumentRepositoryService;
using TLSiKitEditeur.Helpers;
using TLSiKitEditeur.Xades;

namespace TLSiKitEditeur.SoapServiceHelpers
{
    public static class XDSb_CVA
    {
        /// <summary>
        /// Fonction effectuant le chargement du document CDA
        /// </summary>
        /// <returns>le document charge</returns>
        public static XmlDocument loadCDADocument()
        {
            // OID racine d'exemple (à gérer dans le LPS par l'éditeur)
            String sample_sourceOID = "1.2.250.1.999.1.1.7898";


            String codecis = "";
            String efftime = "20220303";

            String doc1UniqueId;

            XmlDocument document = new XmlDocument();
            String fileData = File.ReadAllText(Dmp.CDA_TEMPLATE_CVA);
            String sampleNumericUid = DateTime.Now.Ticks.ToString();
            doc1UniqueId = sample_sourceOID + ".4." + sampleNumericUid;
            codecis = sampleNumericUid;
            efftime = "20190303";
            fileData = fileData.Replace("__documentUniqueId__", doc1UniqueId);
            fileData = fileData.Replace("__patientId__", Dmp.PatientIns);
            fileData = fileData.Replace("__idvac__", UUID.GenerateRandomUuid());
            fileData = fileData.Replace("__codecis__", codecis);
            fileData = fileData.Replace("__efftime__", efftime);
            Console.WriteLine("---------------fileData------------------------------ :" + fileData);
            using (StringReader sr = new StringReader(fileData))
            { document.Load(sr); }

            return document;
        }

        /// <summary>
        /// Fonction effectuant le chargement du document CDA
        /// </summary>
        /// <returns>le document charge</returns>
        public static XmlDocument loadCDADocumentUpdate()
        {
            // OID racine d'exemple (à gérer dans le LPS par l'éditeur)
            String sample_sourceOID = "1.2.250.1.999.1.1.7898";


            String codecis = "";
            String efftime = "20220303";

            String doc1UniqueId;

            XmlDocument document = new XmlDocument();
            String fileData = File.ReadAllText(Dmp.CDA_TEMPLATE_CVA_update);
            String sampleNumericUid = DateTime.Now.Ticks.ToString();
            doc1UniqueId = sample_sourceOID + ".4." + sampleNumericUid;
            codecis = sampleNumericUid;
            efftime = "20190302";
            fileData = fileData.Replace("__documentUniqueId__", doc1UniqueId);
            fileData = fileData.Replace("__documentParentUniqueId__", Dmp.XDS_CVA_UNIQUE_ID);
            fileData = fileData.Replace("__patientId__", Dmp.PatientIns);
            fileData = fileData.Replace("__idvac__", UUID.GenerateRandomUuid());
            fileData = fileData.Replace("__codecis__", codecis);
            fileData = fileData.Replace("__efftime__", efftime);
            Console.WriteLine("---------------fileData------------------------------ :" + fileData);
            using (StringReader sr = new StringReader(fileData))
            { document.Load(sr); }

             return document;
        }

        public static SubmitObjectsRequest LoadRepositoryXDSbRequest(String fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SubmitObjectsRequest),new XmlRootAttribute {
                ElementName = "SubmitObjectsRequest",
                Namespace = "urn:oasis:names:tc:ebxml-regrep:xsd:lcm:3.0"
            });
            StreamReader srdr = new StreamReader(fileName);
            SubmitObjectsRequest req = (SubmitObjectsRequest)serializer.Deserialize(srdr);
            srdr.Close();
            return req;
        }
        public static DocumentRegistryService.SubmitObjectsRequest LoadRegistryXDSbRequest(String fileName)
        {


            XmlSerializer serializer = new XmlSerializer(typeof(DocumentRegistryService.SubmitObjectsRequest));
            
            StreamReader srdr = new StreamReader(fileName);
            DocumentRegistryService.SubmitObjectsRequest req = (DocumentRegistryService.SubmitObjectsRequest)serializer.Deserialize(srdr);
            srdr.Close();
            return req;
        }
        public static DocumentRegistryService.AdhocQueryRequest LoadRegistryXdsbAdhocQueryRequest(String fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(DocumentRegistryService.AdhocQueryRequest), new XmlRootAttribute
            {
                ElementName = "AdhocQueryRequest",
                Namespace = "urn:oasis:names:tc:ebxml-regrep:xsd:query:3.0"
            });

            StreamReader srdr = new StreamReader(fileName);
            DocumentRegistryService.AdhocQueryRequest req = (DocumentRegistryService.AdhocQueryRequest)serializer.Deserialize(srdr);
            srdr.Close();
            return req;
        }

        /// <summary>
        /// Fonction convertissant le document XML passe en parametre en tableau de byte (UTF8)
        /// </summary>
        /// <param name="document">Le document XML</param>
        /// <returns>Document sous forme d'une suite de bytes</returns>
        private static byte[] getBytes(XmlDocument document)
        {
            return UTF8Encoding.UTF8.GetBytes(document.OuterXml);
        }


        #region Verification de la signature XADES

        /// <summary>
        /// Verification de la signature
        /// </summary>
        /// <param name="xmlDocument">Document contenant la signature</param>
        [Conditional("DEBUG")]
        public static void checkSignature(XmlDocument xmlDocument)
        {Logger.Log.Info("Debut de la verification de la signature XADES.");
        
            XmlNodeList signatureNodeList;
            XadesCheckSignatureMasks composedMask;
            bool checkResult;

            try
            {
                ExtendedSignedXml xadesSignedXml = new ExtendedSignedXml(); //Needed if it is a enveloped signature document
                signatureNodeList = xmlDocument.GetElementsByTagName("Signature");
                if (signatureNodeList.Count == 0)
                {
                    signatureNodeList = xmlDocument.GetElementsByTagName("Signature", SignedXml.XmlDsigNamespaceUrl);
                }
                xadesSignedXml.LoadXml((XmlElement)signatureNodeList[0]);

                composedMask = XadesCheckSignatureMasks.CheckXmldsigSignature;
                composedMask |= XadesCheckSignatureMasks.CheckSameCertificate;
                composedMask |= XadesCheckSignatureMasks.CheckQualifyingPropertiesTarget;
                composedMask |= XadesCheckSignatureMasks.CheckQualifyingProperties;

                checkResult = xadesSignedXml.XadesCheckSignature(composedMask);
                if (checkResult == false)
                {
                    Logger.Log.Error("Verification de la signature XADES non reussie.");
                }
                else
                {
                    Logger.Log.Info("Verification de la signature reussie.");
                }

                string signatureTypeLabel = xadesSignedXml.SignatureStandard.ToString();
            }
            catch (Exception exception)
            {
                if (exception.InnerException != null)
                {
                    Logger.Log.Error("Verification de la signature XADES non reussie : Exception : " + exception.Message + " -> " + exception.InnerException.Message);
                }
                else
                {
                    Logger.Log.Error("Verification de la signature XADES non reussie : Exception : " + exception.Message);
                }
            }
        }

        #endregion

        #region Signature XADES
        /// <summary>
        /// Applique le prefixe sur le noeud et les sous noeuds
        /// </summary>
        /// <param name="prefix">Prefixe a rajouter</param>
        /// <param name="node">Noeud sur lequel appliquer le prefix</param>
        public static void SetPrefix(String prefix, XmlNode node)
        {
            if (node.NamespaceURI == SignedXml.XmlDsigNamespaceUrl)
            {
                node.Prefix = prefix;
            }
            foreach (XmlNode child in node.ChildNodes)
            {
                SetPrefix(prefix, child);
            }
        }

        /// <summary>
        /// Fonction permettant d'obtenir la signature Xades d'un lot de soumission a un document
        /// </summary>
        /// <param name="document">Le document CDA</param>
        /// <param name="documentOID">L'OID du document</param>
        /// <param name="submissionSetOID">L'OID du lot</param>
        /// <param name="signatureOID">L'OID de la signature</param>
        /// <param name="signatureCertificate">Certificat utilise pour la signature</param>
        /// <returns>Signature sous forme d'une suite de bytes (UTF8)</returns>
        public static byte[] getXadesSignature(XmlDocument document, string documentOID, string submissionSetOID, string signatureOID, X509Certificate2 signatureCertificate)
        {
            string strxadesSignedXml = "";
            try
            {
                // Creation des info de cle
                KeyInfo keyInfo = new KeyInfo();
                keyInfo.AddClause(new KeyInfoX509Data((X509Certificate)signatureCertificate));

                // Creation du Manifest
                // Application de la transformation sur le document et génération du SHA
                XmlDsigC14NWithCommentsTransform xmlTrans = new XmlDsigC14NWithCommentsTransform();
                xmlTrans.LoadInput(document);
                SHA1 sha1 = SHA1.Create();
                byte[] hashFromNode = xmlTrans.GetDigestedOutput(sha1);

                // Creation de la reference sur le Document
                Reference RefDoc1 = new Reference();
                RefDoc1.DigestMethod = "http://www.w3.org/2000/09/xmldsig#sha1";
                RefDoc1.AddTransform(new XmlDsigC14NWithCommentsTransform());
                RefDoc1.Uri = "urn:oid:" + documentOID;
                RefDoc1.DigestValue = hashFromNode;

                // Creation de la reference sur le Lot
                Reference RefLot = new Reference();
                RefLot.DigestMethod = "http://www.w3.org/2000/09/xmldsig#sha1";
                RefLot.Uri = "urn:oid:" + submissionSetOID;
                RefLot.DigestValue = new byte[] { 0 };

                // Creation du XML du manifest
                XmlDocument XmlIHEManifest = new XmlDocument();
                XmlIHEManifest.PreserveWhitespace = true;
                XmlElement retVal = XmlIHEManifest.CreateElement("ds", "Manifest", SignedXml.XmlDsigNamespaceUrl);
                retVal.SetAttribute("Id", "IHEManifest");
                retVal.AppendChild(XmlIHEManifest.ImportNode(RefLot.GetXml(), true));
                retVal.AppendChild(XmlIHEManifest.ImportNode(RefDoc1.GetXml(), true));
                XmlIHEManifest.AppendChild(retVal);

                // creation de l'object xades
                ExtendedSignedXml xadesSignedXml = new ExtendedSignedXml();
                // on passe la cle de signature
                xadesSignedXml.SigningKey = signatureCertificate.PrivateKey;
                xadesSignedXml.KeyInfo = keyInfo;
                xadesSignedXml.SignedInfo.CanonicalizationMethod = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments";
                xadesSignedXml.Signature.Id = signatureOID;

                // ajout de l'object contenant le manifest
                SetPrefix("ds", XmlIHEManifest);
                DataObject dataObject = new DataObject();
                dataObject.Data = XmlIHEManifest.ChildNodes;
                xadesSignedXml.AddObject(dataObject);

                // ajout de la reference sur le manifest
                Reference manifestRef = new Reference();
                manifestRef.DigestMethod = "http://www.w3.org/2000/09/xmldsig#sha1";
                manifestRef.Type = "http://www.w3.org/2000/09/xmldsig#Manifest";
                manifestRef.Uri = "#IHEManifest";
                manifestRef.AddTransform(new XmlDsigC14NWithCommentsTransform());
                xadesSignedXml.AddReference(manifestRef);

                // ajout de l'object purposeOfSignature imposée par DSG
                XmlDocument xmlDocumentPurposeOfSignature = new XmlDocument();
                XmlElement xmlDocumentPurposeOfSignatureElement = xmlDocumentPurposeOfSignature.CreateElement("ds", "SignatureProperties", SignedXml.XmlDsigNamespaceUrl);
                XmlElement xmlDocumentPurposeOfSignatureElement2 = xmlDocumentPurposeOfSignature.CreateElement("ds", "SignatureProperty", SignedXml.XmlDsigNamespaceUrl);
                xmlDocumentPurposeOfSignatureElement2.SetAttribute("Id", "purposeOfSignature");
                xmlDocumentPurposeOfSignatureElement2.SetAttribute("Target", "#" + signatureOID);
                XmlText xmlDocumentPurposeOfSignatureText = xmlDocumentPurposeOfSignature.CreateTextNode("1.2.840.10065.1.12.1.14");
                xmlDocumentPurposeOfSignatureElement2.AppendChild(xmlDocumentPurposeOfSignatureText);
                xmlDocumentPurposeOfSignatureElement.AppendChild(xmlDocumentPurposeOfSignatureElement2);
                xmlDocumentPurposeOfSignature.AppendChild(xmlDocumentPurposeOfSignatureElement);
                System.Security.Cryptography.Xml.DataObject dataObjectPurposeOfSignature = new System.Security.Cryptography.Xml.DataObject();
                dataObjectPurposeOfSignature.Data = xmlDocumentPurposeOfSignature.ChildNodes;
                xadesSignedXml.AddObject(dataObjectPurposeOfSignature);

                // creation des propriété specifique a XAdES
                XadesObject xadesObject = new XadesObject();
                xadesObject.QualifyingProperties.Target = "#" + signatureOID; ;
                // construction de la chaine de certificat
                X509Chain Chain = new X509Chain();
                Chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                Chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                Chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 0, 30);
                Chain.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;
                Chain.Build(signatureCertificate);
                // ajout des certificats
                foreach (X509ChainElement element in Chain.ChainElements)
                {
                    Cert chainCert = new Cert();
                    //enleve les blanc apres la vigule
                    string strX509IssuerName = element.Certificate.IssuerName.Name;
                    string strX509IssuerName2 = strX509IssuerName.Replace(", ", ",");
                    chainCert.IssuerSerial.X509IssuerName = strX509IssuerName2;
                    // convertion du numero de serie d'hexa en decimal
                    byte[] SNByteArray1 = element.Certificate.GetSerialNumber();
                    Array.Reverse(SNByteArray1);
                    chainCert.IssuerSerial.X509SerialNumber = (new ScottGarland.BigInteger(SNByteArray1)).ToString(10);
                    chainCert.CertDigest.DigestMethod.Algorithm = SignedXml.XmlDsigSHA1Url;
                    chainCert.CertDigest.DigestValue = element.Certificate.GetCertHash();
                    xadesObject.QualifyingProperties.SignedProperties.SignedSignatureProperties.SigningCertificate.CertCollection.Add(chainCert);
                }
                xadesObject.QualifyingProperties.SignedProperties.SignedSignatureProperties.SigningTime = DateTime.Now;
                xadesObject.QualifyingProperties.SignedProperties.SignedSignatureProperties.SignaturePolicyIdentifier.SignaturePolicyImplied = true;
                // ajout de l'object contenant la propriete xades
                xadesSignedXml.AddXadesObject(xadesObject);

                // Generer la signature
                xadesSignedXml.ComputeSignature();

                // Ajout de la declaration XML
                XmlDocument xmlDoc = new XmlDocument();
                //xmlDoc.Normalize();
                XmlDeclaration xmldecl = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);

                strxadesSignedXml = xmldecl.OuterXml + xadesSignedXml.GetXml().OuterXml;
            }
            catch (Exception exception)
            {
                Logger.Log.Error("Exception dans la generation de la signature :" + exception.Message);
                Console.WriteLine(exception.Message);
            }

            return UTF8Encoding.UTF8.GetBytes(strxadesSignedXml);
        }
        #endregion

       
        /// <summary>
        /// Generation du XDSb
        /// </summary>
        /// <param name="patientID"></param>
        /// <param name="submissionSet_OID"></param>
        /// <param name="documentOID"></param>
        /// <param name="signatureOID"></param>
        /// <param name="authorPerson"></param>
        /// <param name="Identifiant_Structure"></param>
        /// <returns></returns>
        public static SubmitObjectsRequest createXDSb(string patientID, string submissionSet_OID, string documentOID, string signatureOID, string authorPerson, string Identifiant_Structure)
        {
            string xdsPatientID = patientID + "^^^&1.2.250.1.213.1.4.10&ISO^NH^^20100907";

            SubmitObjectsRequest request = new SubmitObjectsRequest();

            // Creation Slot

            SlotType1 Slot_authorPerson = createSlot("authorPerson", authorPerson);
            SlotType1 Slot_authorSpecialty = createSlot("authorSpecialty", "G15_10/" + Dmp.SpecCode + "^" + Dmp.SpecDisplayName + "^1.2.250.1.213.1.1.4.5");
            SlotType1 Slot_authorInstitution = createSlot("authorInstitution", "HOPITAL DES 3 VALLEES00771^^^^^&1.2.250.1.71.4.2.2&ISO^IDNST^^^" + Identifiant_Structure);
            SlotType1 Slot_submissionTime = new SlotType1();
            Slot_submissionTime.name = "submissionTime";
            Slot_submissionTime.ValueList = new ValueListType();
            Slot_submissionTime.ValueList.Value = new string[] { Time.GetConcatenatedActualTime() };

            //Creation RegistryPackage

            RegistryPackageType registryPackage = new RegistryPackageType();
            registryPackage.id = "SubmissionSet01";
            registryPackage.Name = createInternationalStringType("Compte rendu", "FR", "UTF8");
            registryPackage.Description = createInternationalStringType("Compte rendu de test", "FR", "UTF8");
            registryPackage.Slot = new SlotType1[] { Slot_submissionTime };

            ClassificationType classification1 = createClassificationScheme("urn:uuid:a7058bb9-b4e4-4307-ba5b-e3f0ab85e12d", "SubmissionSet01", "cla55", "");
            classification1.Slot = new SlotType1[]{
                Slot_authorPerson,
                Slot_authorSpecialty,
                Slot_authorInstitution
            };

            ClassificationType classification2 = createClassificationScheme("urn:uuid:aa543740-bdda-424e-8c96-df4873be8500", "SubmissionSet01", "cla56", "04");
            classification2.Slot = new SlotType1[] {
                createSlot("codingScheme","1.2.250.1.213.2.2"),
            };
            classification2.Name = createInternationalStringType("Hospitalisation de jour", "FR", "UTF8");

            ClassificationType classification3 = createClassificationNode("urn:uuid:a54d6aa5-d40d-43f9-88c5-b4633d873bdd", "SubmissionSet01", "cla57", null);

            registryPackage.Classification = new ClassificationType[] { classification1, classification2, classification3 };

            registryPackage.ExternalIdentifier = new ExternalIdentifierType[] {
                createExternalIdentifier("ei22","urn:uuid:6b5aea1a-874d-4603-a4bc-96a0a7b38446", "SubmissionSet01", xdsPatientID, "XDSSubmissionSet.patientId"),
                createExternalIdentifier("ei23","urn:uuid:554ac39e-e3fe-47fe-b233-965d2a147832","SubmissionSet01","1.2.250.1.999.1.1.7898","XDSSubmissionSet.sourceId"),
                createExternalIdentifier("ei24","urn:uuid:96fdda7c-d067-4183-912e-bf5ee74998a8","SubmissionSet01",submissionSet_OID,"XDSSubmissionSet.uniqueId")
            };

            // Creation Extrinsic Object Document

            String strDocCreationTime = "20100319183244";
            SlotType1[] slotsExtrinsicObjectDocument1 = new SlotType1[]{
                createSlot("creationTime",strDocCreationTime),
                createSlot("languageCode","fr-FR"),
                createSlot("legalAuthenticator","00B2800585^OPTOLUNE0058^SEBASTIEN^^^^^^&1.2.250.1.71.4.2.1&ISO^D^^^IDNPS"),
                createSlot("serviceStartTime",strDocCreationTime),
                createSlot("serviceStopTime",strDocCreationTime),
                createSlot("sourcePatientId","0456789999^^^&1.2.250.1.999.1.1.7898.2&ISO^PI"),
                createSlot("sourcePatientInfo",new string[]{"PID-5|MARTINQUARANTESIX^Simonquasix^^^^^L","PID-7|19760717","PID-8|F"})
            };

            SlotType1[] slotsClassificationsExtrinsicObjectDocument1 = new SlotType1[]{
                Slot_authorPerson,
                Slot_authorSpecialty,
                Slot_authorInstitution
            };

            ClassificationType[] classificationsExtrinsicObjectDocument1 = new ClassificationType[]{
                createClassification("urn:uuid:93606bcf-9494-43ec-9b4e-a7748d1a838d","Document01","cla58","", slotsClassificationsExtrinsicObjectDocument1, null),
                createClassification("urn:uuid:41a5887f-8865-4c09-adf7-e362475b143a","Document01","cla59","10",new SlotType1[]{createSlot("codingScheme","1.2.250.1.213.1.1.4.1")},"Compte rendu"),
                createClassification("urn:uuid:f4f85eac-e6cb-4883-b524-f2705394840f","Document01","cla60","N",new SlotType1[]{createSlot("codingScheme","2.16.840.1.113883.5.25")},"Normal"),
                createClassification("urn:uuid:f4f85eac-e6cb-4883-b524-f2705394840f","Document01","cla16","INVISIBLE_PATIENT",new SlotType1[]{createSlot("codingScheme","1.2.250.1.213.1.1.4.13")},"Non visible par le patient"),
                createClassification("urn:uuid:a09d5840-386c-46f2-b5ad-9c3699a4309d","Document01","cla61","urn:ihe:iti:xds-sd:text:2008",new SlotType1[]{createSlot("codingScheme","1.3.6.1.4.1.19376.1.2.3")},"Document à corps non structuré en texte brut"),
                createClassification("urn:uuid:f33fb8ac-18af-42cc-ae0e-ed0b0bdb91e1","Document01","cla63","SA01",new SlotType1[]{createSlot("codingScheme","1.2.250.1.71.4.2.4")},"Etablissement public de santé"),
                createClassification("urn:uuid:cccf5598-8b07-4b77-a05e-ae952c785ead","Document01","cla64","ETABLISSEMENT", new SlotType1[]{createSlot("codingScheme","1.2.250.1.213.1.1.4.9")},"Etablissement de santé"),
                createClassification("urn:uuid:f0306f51-975f-434e-a61c-c59651d33983","Document01","cla65","87273-9",new SlotType1[]{createSlot("codingScheme","2.16.840.1.113883.6.1")},"Note de vaccination")
            };

            ExternalIdentifierType[] externalIdentifierExtrinsicObjectDocument1 = new ExternalIdentifierType[]{
                createExternalIdentifier("ei25","urn:uuid:58a6f841-87b3-4a3e-92fd-a8ffeff98427","Document01",xdsPatientID,"XDSDocumentEntry.patientId"),
                createExternalIdentifier("ei26","urn:uuid:2e82c1f6-a085-4c72-9da3-8640a32e42ab","Document01",documentOID,"XDSDocumentEntry.uniqueId")
            };

            ExtrinsicObjectType extrinsicObject1 = createExtrinsicObject("Document01", "text/xml", "urn:uuid:7edca82f-054d-47f2-a032-9b2a5b5186c1", slotsExtrinsicObjectDocument1, "Titre du document de TEST KIT EDITEUR CSharp", "commentaire du document de TEST KIT EDITEUR CSharp", classificationsExtrinsicObjectDocument1, externalIdentifierExtrinsicObjectDocument1);

            // Signature

            String strSignatureTime = Time.GetConcatenatedActualTime();
            SlotType1[] slotsExtrinsicObjectSignature = new SlotType1[]{
                createSlot("creationTime",strSignatureTime),
                createSlot("languageCode","art"),
                createSlot("legalAuthenticator",authorPerson),
                createSlot("serviceStartTime",strSignatureTime),
                createSlot("serviceStopTime",strSignatureTime),
                createSlot("sourcePatientId",xdsPatientID)
            };

            SlotType1[] slotsClassificationsExtrinsicObjectSignature = new SlotType1[]{
                Slot_authorPerson,
                Slot_authorSpecialty,
                Slot_authorInstitution
            };

            ClassificationType[] classificationsExtrinsicObjectSignature = new ClassificationType[]{
                createClassification("urn:uuid:93606bcf-9494-43ec-9b4e-a7748d1a838d","Signature01","cla66","",slotsClassificationsExtrinsicObjectSignature,null),
                createClassification("urn:uuid:41a5887f-8865-4c09-adf7-e362475b143a","Signature01","cla67","urn:oid:1.3.6.1.4.1.19376.1.2.1.1.1",new SlotType1[]{createSlot("codingScheme","URN")}, "Digital Signature"),
                createClassification("urn:uuid:f4f85eac-e6cb-4883-b524-f2705394840f","Signature01","cla68","N",new SlotType1[]{createSlot("codingScheme","2.16.840.1.113883.5.25")},"Normal"),
                createClassification("urn:uuid:f4f85eac-e6cb-4883-b524-f2705394840f","Signature01","cla16","INVISIBLE_PATIENT",new SlotType1[]{createSlot("codingScheme","1.2.250.1.213.1.1.4.13")},"Non visible par le patient"),
                createClassification("urn:uuid:f4f85eac-e6cb-4883-b524-f2705394840f","Signature01","cla17","MASQUE_PS",new SlotType1[]{createSlot("codingScheme","1.2.250.1.213.1.1.4.13")},"Masqué aux professionnels de santé"),
                createClassification("urn:uuid:2c6b8cb7-8b2a-4051-b291-b1ae6a575ef4","Signature01","cla69","1.2.840.10065.1.12.1.14",new SlotType1[]{createSlot("codingScheme","1.2.840.10065.1.12")},"Source"),
                createClassification("urn:uuid:a09d5840-386c-46f2-b5ad-9c3699a4309d","Signature01","cla70","http://www.w3.org/2000/09/xmldsig#",new SlotType1[]{createSlot("codingScheme","URN")},"Default Signature Style"),
                createClassification("urn:uuid:f33fb8ac-18af-42cc-ae0e-ed0b0bdb91e1","Signature01","cla71","SA01",new SlotType1[]{createSlot("codingScheme","1.2.250.1.71.4.2.4")},"Etablissement public de santé"),
                createClassification("urn:uuid:cccf5598-8b07-4b77-a05e-ae952c785ead","Signature01","cla72","ETABLISSEMENT",new SlotType1[]{createSlot("codingScheme","1.2.250.1.213.1.1.4.9")},"Etablissement de santé"),
                createClassification("urn:uuid:f0306f51-975f-434e-a61c-c59651d33983","Signature01","cla73","E1762",new SlotType1[]{createSlot("codingScheme","ASTM")},"Full Document")
            };

            ExternalIdentifierType[] externalIdentifierExtrinsicObjectSignature = new ExternalIdentifierType[]{
                createExternalIdentifier("ei27","urn:uuid:58a6f841-87b3-4a3e-92fd-a8ffeff98427","Signature01",xdsPatientID,"XDSDocumentEntry.patientId"),
                createExternalIdentifier("ei28","urn:uuid:2e82c1f6-a085-4c72-9da3-8640a32e42ab","Signature01",signatureOID,"XDSDocumentEntry.uniqueId")
            };

            ExtrinsicObjectType extrinsicObjectSignature = createExtrinsicObject("Signature01", "text/xml", "urn:uuid:7edca82f-054d-47f2-a032-9b2a5b5186c1", slotsExtrinsicObjectSignature, "Source", null, classificationsExtrinsicObjectSignature, externalIdentifierExtrinsicObjectSignature);

            AssociationType1 association1 = createAssociation("urn:oasis:names:tc:ebxml-regrep:AssociationType:HasMember", "association1", "urn:oasis:names:tc:ebxml-regrep:ObjectType:RegistryObject:Association", "SubmissionSet01", "Document01", "SubmissionSetStatus", "Original");
            AssociationType1 association2 = createAssociation("urn:oasis:names:tc:ebxml-regrep:AssociationType:HasMember", "association2", "urn:oasis:names:tc:ebxml-regrep:ObjectType:RegistryObject:Association", "SubmissionSet01", "Signature01", "SubmissionSetStatus", "Original");
            AssociationType1 association3 = createAssociation("urn:ihe:iti:2007:AssociationType:signs", "association3", "urn:oasis:names:tc:ebxml-regrep:ObjectType:RegistryObject:Association", "Signature01", "SubmissionSet01", "SubmissionSetStatus", "Original");

            request.RegistryObjectList = new IdentifiableType[] { registryPackage, extrinsicObject1, extrinsicObjectSignature, association1, association2, association3 };

            return request;
        }

        /// <summary>
        /// Generation du XDSb
        /// </summary>
        /// <param name="patientID"></param>
        /// <param name="submissionSet_OID"></param>
        /// <param name="documentOID"></param>
        /// <param name="signatureOID"></param>
        /// <param name="authorPerson"></param>
        /// <param name="Identifiant_Structure"></param>
        /// <returns></returns>
        public static SubmitObjectsRequest createXDSbUpdate(string patientID, string submissionSet_OID, string documentOID, string signatureOID, string authorPerson, string Identifiant_Structure)
        {
            string xdsPatientID = patientID + "^^^&1.2.250.1.213.1.4.10&ISO^NH^^20100907";

            SubmitObjectsRequest request = new SubmitObjectsRequest();

            // Creation Slot

            SlotType1 Slot_authorPerson = createSlot("authorPerson", authorPerson);
            SlotType1 Slot_authorSpecialty = createSlot("authorSpecialty", "G15_10/SM07^Médecin - Chirurgie maxillo-faciale et stomatologie (SM)^1.2.250.1.213.1.1.4.5");
            SlotType1 Slot_authorInstitution = createSlot("authorInstitution", "HOPITAL DES 3 VALLEES00771^^^^^&1.2.250.1.71.4.2.2&ISO^IDNST^^^" + Identifiant_Structure);
            SlotType1 Slot_submissionTime = new SlotType1();
            Slot_submissionTime.name = "submissionTime";
            Slot_submissionTime.ValueList = new ValueListType();
            Slot_submissionTime.ValueList.Value = new string[] { Time.GetConcatenatedActualTime() };

            //Creation RegistryPackage

            RegistryPackageType registryPackage = new RegistryPackageType();
            registryPackage.id = "SubmissionSet01";
            registryPackage.Name = createInternationalStringType("Compte rendu", "FR", "UTF8");
            registryPackage.Description = createInternationalStringType("Compte rendu de test", "FR", "UTF8");
            registryPackage.Slot = new SlotType1[] { Slot_submissionTime };

            ClassificationType classification1 = createClassificationScheme("urn:uuid:a7058bb9-b4e4-4307-ba5b-e3f0ab85e12d", "SubmissionSet01", "cla55", "");
            classification1.Slot = new SlotType1[]{
                Slot_authorPerson,
                Slot_authorSpecialty,
                Slot_authorInstitution
            };

            ClassificationType classification2 = createClassificationScheme("urn:uuid:aa543740-bdda-424e-8c96-df4873be8500", "SubmissionSet01", "cla56", "04");
            classification2.Slot = new SlotType1[] {
                createSlot("codingScheme","1.2.250.1.213.2.2"),
            };
            classification2.Name = createInternationalStringType("Hospitalisation de jour", "FR", "UTF8");

            ClassificationType classification3 = createClassificationNode("urn:uuid:a54d6aa5-d40d-43f9-88c5-b4633d873bdd", "SubmissionSet01", "cla57", null);

            registryPackage.Classification = new ClassificationType[] { classification1, classification2, classification3 };

            registryPackage.ExternalIdentifier = new ExternalIdentifierType[] {
                createExternalIdentifier("ei22","urn:uuid:6b5aea1a-874d-4603-a4bc-96a0a7b38446", "SubmissionSet01", xdsPatientID, "XDSSubmissionSet.patientId"),
                createExternalIdentifier("ei23","urn:uuid:554ac39e-e3fe-47fe-b233-965d2a147832","SubmissionSet01","1.2.250.1.999.1.1.7898","XDSSubmissionSet.sourceId"),
                createExternalIdentifier("ei24","urn:uuid:96fdda7c-d067-4183-912e-bf5ee74998a8","SubmissionSet01",submissionSet_OID,"XDSSubmissionSet.uniqueId")
            };

            // Creation Extrinsic Object Document

            String strDocCreationTime = "20100319183244";
            SlotType1[] slotsExtrinsicObjectDocument1 = new SlotType1[]{
                createSlot("creationTime",strDocCreationTime),
                createSlot("languageCode","fr-FR"),
                createSlot("legalAuthenticator","00B2800585^OPTOLUNE0058^SEBASTIEN^^^^^^&1.2.250.1.71.4.2.1&ISO^D^^^IDNPS"),
                createSlot("serviceStartTime",strDocCreationTime),
                createSlot("serviceStopTime",strDocCreationTime),
                createSlot("sourcePatientId","0456789999^^^&1.2.250.1.999.1.1.7898.2&ISO^PI"),
                createSlot("sourcePatientInfo",new string[]{"PID-5|MARTINQUARANTESIX^Simonquasix^^^^^L","PID-7|19760717","PID-8|F"})
            };

            SlotType1[] slotsClassificationsExtrinsicObjectDocument1 = new SlotType1[]{
                Slot_authorPerson,
                Slot_authorSpecialty,
                Slot_authorInstitution
            };

            ClassificationType[] classificationsExtrinsicObjectDocument1 = new ClassificationType[]{
                createClassification("urn:uuid:93606bcf-9494-43ec-9b4e-a7748d1a838d","Document01","cla58","", slotsClassificationsExtrinsicObjectDocument1, null),
                createClassification("urn:uuid:41a5887f-8865-4c09-adf7-e362475b143a","Document01","cla59","10",new SlotType1[]{createSlot("codingScheme","1.2.250.1.213.1.1.4.1")},"Compte rendu"),
                createClassification("urn:uuid:f4f85eac-e6cb-4883-b524-f2705394840f","Document01","cla60","N",new SlotType1[]{createSlot("codingScheme","2.16.840.1.113883.5.25")},"Normal"),
                createClassification("urn:uuid:f4f85eac-e6cb-4883-b524-f2705394840f","Document01","cla16","INVISIBLE_PATIENT",new SlotType1[]{createSlot("codingScheme","1.2.250.1.213.1.1.4.13")},"Non visible par le patient"),
                createClassification("urn:uuid:a09d5840-386c-46f2-b5ad-9c3699a4309d","Document01","cla61","urn:ihe:iti:xds-sd:text:2008",new SlotType1[]{createSlot("codingScheme","1.3.6.1.4.1.19376.1.2.3")},"Document à corps non structuré en texte brut"),
                createClassification("urn:uuid:f33fb8ac-18af-42cc-ae0e-ed0b0bdb91e1","Document01","cla63","SA01",new SlotType1[]{createSlot("codingScheme","1.2.250.1.71.4.2.4")},"Etablissement public de santé"),
                createClassification("urn:uuid:cccf5598-8b07-4b77-a05e-ae952c785ead","Document01","cla64","ETABLISSEMENT", new SlotType1[]{createSlot("codingScheme","1.2.250.1.213.1.1.4.9")},"Etablissement de santé"),
                createClassification("urn:uuid:f0306f51-975f-434e-a61c-c59651d33983","Document01","cla65","87273-9",new SlotType1[]{createSlot("codingScheme","2.16.840.1.113883.6.1")},"Note de vaccination")
            };

            ExternalIdentifierType[] externalIdentifierExtrinsicObjectDocument1 = new ExternalIdentifierType[]{
                createExternalIdentifier("ei25","urn:uuid:58a6f841-87b3-4a3e-92fd-a8ffeff98427","Document01",xdsPatientID,"XDSDocumentEntry.patientId"),
                createExternalIdentifier("ei26","urn:uuid:2e82c1f6-a085-4c72-9da3-8640a32e42ab","Document01",documentOID,"XDSDocumentEntry.uniqueId")
            };

            ExtrinsicObjectType extrinsicObject1 = createExtrinsicObject("Document01", "text/xml", "urn:uuid:7edca82f-054d-47f2-a032-9b2a5b5186c1", slotsExtrinsicObjectDocument1, "Titre du document de TEST KIT EDITEUR CSharp", "commentaire du document de TEST KIT EDITEUR CSharp", classificationsExtrinsicObjectDocument1, externalIdentifierExtrinsicObjectDocument1);

            // Signature

            String strSignatureTime = Time.GetConcatenatedActualTime();
            SlotType1[] slotsExtrinsicObjectSignature = new SlotType1[]{
                createSlot("creationTime",strSignatureTime),
                createSlot("languageCode","art"),
                createSlot("legalAuthenticator",authorPerson),
                createSlot("serviceStartTime",strSignatureTime),
                createSlot("serviceStopTime",strSignatureTime),
                createSlot("sourcePatientId",xdsPatientID)
            };

            SlotType1[] slotsClassificationsExtrinsicObjectSignature = new SlotType1[]{
                Slot_authorPerson,
                Slot_authorSpecialty,
                Slot_authorInstitution
            };

            ClassificationType[] classificationsExtrinsicObjectSignature = new ClassificationType[]{
                createClassification("urn:uuid:93606bcf-9494-43ec-9b4e-a7748d1a838d","Signature01","cla66","",slotsClassificationsExtrinsicObjectSignature,null),
                createClassification("urn:uuid:41a5887f-8865-4c09-adf7-e362475b143a","Signature01","cla67","urn:oid:1.3.6.1.4.1.19376.1.2.1.1.1",new SlotType1[]{createSlot("codingScheme","URN")}, "Digital Signature"),
                createClassification("urn:uuid:f4f85eac-e6cb-4883-b524-f2705394840f","Signature01","cla68","N",new SlotType1[]{createSlot("codingScheme","2.16.840.1.113883.5.25")},"Normal"),
                createClassification("urn:uuid:f4f85eac-e6cb-4883-b524-f2705394840f","Signature01","cla16","INVISIBLE_PATIENT",new SlotType1[]{createSlot("codingScheme","1.2.250.1.213.1.1.4.13")},"Non visible par le patient"),
                createClassification("urn:uuid:f4f85eac-e6cb-4883-b524-f2705394840f","Signature01","cla17","MASQUE_PS",new SlotType1[]{createSlot("codingScheme","1.2.250.1.213.1.1.4.13")},"Masqué aux professionnels de santé"),
                createClassification("urn:uuid:2c6b8cb7-8b2a-4051-b291-b1ae6a575ef4","Signature01","cla69","1.2.840.10065.1.12.1.14",new SlotType1[]{createSlot("codingScheme","1.2.840.10065.1.12")},"Source"),
                createClassification("urn:uuid:a09d5840-386c-46f2-b5ad-9c3699a4309d","Signature01","cla70","http://www.w3.org/2000/09/xmldsig#",new SlotType1[]{createSlot("codingScheme","URN")},"Default Signature Style"),
                createClassification("urn:uuid:f33fb8ac-18af-42cc-ae0e-ed0b0bdb91e1","Signature01","cla71","SA01",new SlotType1[]{createSlot("codingScheme","1.2.250.1.71.4.2.4")},"Etablissement public de santé"),
                createClassification("urn:uuid:cccf5598-8b07-4b77-a05e-ae952c785ead","Signature01","cla72","ETABLISSEMENT",new SlotType1[]{createSlot("codingScheme","1.2.250.1.213.1.1.4.9")},"Etablissement de santé"),
                createClassification("urn:uuid:f0306f51-975f-434e-a61c-c59651d33983","Signature01","cla73","E1762",new SlotType1[]{createSlot("codingScheme","ASTM")},"Full Document")
            };

            ExternalIdentifierType[] externalIdentifierExtrinsicObjectSignature = new ExternalIdentifierType[]{
                createExternalIdentifier("ei27","urn:uuid:58a6f841-87b3-4a3e-92fd-a8ffeff98427","Signature01",xdsPatientID,"XDSDocumentEntry.patientId"),
                createExternalIdentifier("ei28","urn:uuid:2e82c1f6-a085-4c72-9da3-8640a32e42ab","Signature01",signatureOID,"XDSDocumentEntry.uniqueId")
            };

            ExtrinsicObjectType extrinsicObjectSignature = createExtrinsicObject("Signature01", "text/xml", "urn:uuid:7edca82f-054d-47f2-a032-9b2a5b5186c1", slotsExtrinsicObjectSignature, "Source", null, classificationsExtrinsicObjectSignature, externalIdentifierExtrinsicObjectSignature);

            AssociationType1 association1 = createAssociation("urn:oasis:names:tc:ebxml-regrep:AssociationType:HasMember", "association1", "urn:oasis:names:tc:ebxml-regrep:ObjectType:RegistryObject:Association", "SubmissionSet01", "Document01", "SubmissionSetStatus", "Original");
            AssociationType1 association2 = createAssociation("urn:oasis:names:tc:ebxml-regrep:AssociationType:HasMember", "association2", "urn:oasis:names:tc:ebxml-regrep:ObjectType:RegistryObject:Association", "SubmissionSet01", "Signature01", "SubmissionSetStatus", "Original");
            AssociationType1 association3 = createAssociation("urn:ihe:iti:2007:AssociationType:signs", "association3", "urn:oasis:names:tc:ebxml-regrep:ObjectType:RegistryObject:Association", "Signature01", "SubmissionSet01", "SubmissionSetStatus", "Original");
            AssociationType1 association4 = createAssociation("urn:ihe:iti:2010:AssociationType:RPLC", "association4", "urn:oasis:names:tc:ebxml-regrep:ObjectType:RegistryObject:Association", "Document01", Dmp.XDS_CVA_ENTRY_UUID, "SubmissionSetStatus", "Original");

            request.RegistryObjectList = new IdentifiableType[] { registryPackage, extrinsicObject1, extrinsicObjectSignature, association1, association2, association3, association4 };

            return request;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="associationType"></param>
        /// <param name="id"></param>
        /// <param name="objectType"></param>
        /// <param name="sourceObject"></param>
        /// <param name="targetObject"></param>
        /// <param name="slotName"></param>
        /// <param name="slotValue"></param>
        /// <returns></returns>
        private static AssociationType1 createAssociation(string associationType, string id, string objectType, string sourceObject, string targetObject, string slotName, string slotValue)
        {
            AssociationType1 association = new AssociationType1();
            association.associationType = associationType;
            association.id = id;
            association.objectType = objectType;
            association.sourceObject = sourceObject;
            association.targetObject = targetObject;
            association.Slot = new SlotType1[] { createSlot(slotName, slotValue) };

            return association;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="identificationScheme"></param>
        /// <param name="registryObject"></param>
        /// <param name="value"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static ExternalIdentifierType createExternalIdentifier(string id, string identificationScheme, string registryObject, string value, string name)
        {
            ExternalIdentifierType identifier = createExternalIdentifier(id, identificationScheme, registryObject, value);
            identifier.Name = createInternationalStringType(name, "FR", "UTF8");
            return identifier;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="identificationScheme"></param>
        /// <param name="registryObject"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static ExternalIdentifierType createExternalIdentifier(string id, string identificationScheme, string registryObject, string value)
        {
            ExternalIdentifierType identifier = new ExternalIdentifierType();
            identifier.id = id;
            identifier.identificationScheme = identificationScheme;
            identifier.registryObject = registryObject;
            identifier.value = value;

            return identifier;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="classificationScheme"></param>
        /// <param name="classifiedObject"></param>
        /// <param name="id"></param>
        /// <param name="nodeRepresentation"></param>
        /// <param name="slots"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static ClassificationType createClassification(string classificationScheme, string classifiedObject, string id, string nodeRepresentation, SlotType1[] slots, string name)
        {
            ClassificationType classification = createClassificationScheme(classificationScheme, classifiedObject, id, nodeRepresentation);
            classification.Slot = slots;
            if (name != null) classification.Name = createInternationalStringType(name, "FR", "UTF8");
            return classification;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="classificationScheme"></param>
        /// <param name="classifiedObject"></param>
        /// <param name="id"></param>
        /// <param name="nodeRepresentation"></param>
        /// <returns></returns>
        private static ClassificationType createClassificationScheme(string classificationScheme, string classifiedObject, string id, string nodeRepresentation)
        {
            ClassificationType classification = new ClassificationType();
            classification.classificationScheme = classificationScheme;
            classification.classifiedObject = classifiedObject;
            classification.id = id;
            if (nodeRepresentation != null) classification.nodeRepresentation = nodeRepresentation;

            return classification;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="classificationNode"></param>
        /// <param name="classifiedObject"></param>
        /// <param name="id"></param>
        /// <param name="nodeRepresentation"></param>
        /// <returns></returns>
        private static ClassificationType createClassificationNode(string classificationNode, string classifiedObject, string id, string nodeRepresentation)
        {
            ClassificationType classification = new ClassificationType();
            classification.classificationNode = classificationNode;
            classification.classifiedObject = classifiedObject;
            classification.id = id;
            if (nodeRepresentation != null) classification.nodeRepresentation = nodeRepresentation;

            return classification;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        private static SlotType1 createSlot(string name, string[] values)
        {
            SlotType1 slot = new SlotType1();
            if (name != null) slot.name = name;
            slot.ValueList = new ValueListType();
            slot.ValueList.Value = values;
            return slot;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static SlotType1 createSlot(string name, string value)
        {
            return createSlot(name, new string[] { value });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="lang"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        private static InternationalStringType createInternationalStringType(string value, string lang, string charset)
        {
            InternationalStringType retour = new InternationalStringType();
            retour.LocalizedString = new LocalizedStringType[1];
            retour.LocalizedString[0] = new LocalizedStringType();
            retour.LocalizedString[0].charset = charset;
            retour.LocalizedString[0].lang = lang;
            retour.LocalizedString[0].value = value;
            return retour;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mimeType"></param>
        /// <param name="objectType"></param>
        /// <param name="slots"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="classifications"></param>
        /// <param name="externalIdentifiers"></param>
        /// <returns></returns>
        private static ExtrinsicObjectType createExtrinsicObject(string id, string mimeType, string objectType, SlotType1[] slots, string name, string description, ClassificationType[] classifications, ExternalIdentifierType[] externalIdentifiers)
        {
            ExtrinsicObjectType extrinsicObject = new ExtrinsicObjectType();
            extrinsicObject.id = id;
            extrinsicObject.mimeType = mimeType;
            extrinsicObject.objectType = objectType;

            extrinsicObject.Slot = slots;

            extrinsicObject.Name = createInternationalStringType(name, "FR", "UTF8");
            extrinsicObject.Description = createInternationalStringType(description, "FR", "UTF8");
            extrinsicObject.Classification = classifications;
            extrinsicObject.ExternalIdentifier = externalIdentifiers;
            return extrinsicObject;
        }
    }
}
