using Microsoft.VisualStudio.TestTools.UnitTesting;
using TLSiKitEditeur.SoapService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLSiKitEditeur.DocumentRepositoryService;
using TLSiKitEditeur.CertificateHelpers;
using TLSiKitEditeur.Helpers;
using TLSiKitEditeur.SoapServiceHelpers;
using TLSiKitEditeur.VIHF;
using System.Xml;
using System.IO;

namespace TLSiKitEditeur.SoapService.Tests
{
    [TestClass()]
    public class DocumentRepositorySoapServiceTests
    {

        private DocumentRepository_PortTypeClient _service;
        private static TestContext _testContext;
        [ClassInitialize]
        public static void SetupTests(TestContext testContext)
        {
            _testContext = testContext;
            Logger.Log.Info("Initialization des certificats");
            CertificateHelper.InitCertificate();
            Logger.Log.Info("Vérification des certificats");
            CertificateHelper.CheckCertificates();
            Logger.Log.Info("Initialization de Proxy");
            WebProxyHelper.ConfigureProxy();
            DocumentRepositorySoapService rs = new DocumentRepositorySoapService();
            DocumentRegistrySoapService dr = new DocumentRegistrySoapService();
        }
        [TestInitialize]
        public void InitializeTests()
        {
            _service = new DocumentRepository_PortTypeClient();
            _service.Init("repository");
        }
     
        [TestMethod()]
        public void ProvideAndRegisterDocumentSetTest()
        {
            Logger.Log.Info($"Appel de la fonction : {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            _service.AddHeaders("urn:ihe:iti:xds-b:2007:ProvideAndRegisterDocumentSet-b");
            SubmitObjectsRequest request = XDSb.createXDSb(Dmp.PatientIns, Dmp.SubmissionSet_OID, Dmp.DocumentCDA_OID, Dmp.Signature_OID, Dmp.AuthorPerson, VihfProvider.Instance.GetVihf().Identifiant_Structure);
            ProvideAndRegisterDocumentSetRequestType document = new ProvideAndRegisterDocumentSetRequestType();
            XmlDocument documentCDA = XDSb.loadCDADocument();
            document.Document = new ProvideAndRegisterDocumentSetRequestTypeDocument[2];
            document.Document[0] = new ProvideAndRegisterDocumentSetRequestTypeDocument();
            document.Document[0].id = "Signature01";
            document.Document[0].Value = XDSb.getXadesSignature(documentCDA, Dmp.DocumentCDA_OID, Dmp.SubmissionSet_OID, Dmp.Signature_OID, CertificateHelper.SignatureCertificate);
#if DEBUG
            //dump xades
            FileStream fs = new FileStream("Xades_part.xml", FileMode.Create);
            BinaryWriter w = new BinaryWriter(fs);
            w.Write(document.Document[0].Value);
            fs.Close();
            XmlDocument xmlXades = new XmlDocument();
            xmlXades.Load("Xades_part.xml");
            XDSb.checkSignature(xmlXades);
#endif
            document.Document[1] = new ProvideAndRegisterDocumentSetRequestTypeDocument();
            document.Document[1].id = "Document01";
            document.Document[1].Value = Encoding.UTF8.GetBytes(documentCDA.OuterXml);
            document.SubmitObjectsRequest = request;

            var response = _service.DocumentRepository_ProvideAndRegisterDocumentSetb(document);
            Console.WriteLine("Statut de la reponse: " + response.status);
            if (response.status.Equals("urn:oasis:names:tc:ebxml-regrep:ResponseStatusType:Success"))
            {
                Assert.IsTrue(true);
                return;
            }
            for (int i = 0; ((response.RegistryErrorList != null) && (i < response.RegistryErrorList.RegistryError.Length)); i++)
            {
                Console.WriteLine("Probleme:");
                Console.WriteLine("Value: " + response.RegistryErrorList.RegistryError[i].Value);
                Console.WriteLine("Code: " + response.RegistryErrorList.RegistryError[i].errorCode);
                Console.WriteLine("location: " + response.RegistryErrorList.RegistryError[i].location);
                Console.WriteLine("severity: " + response.RegistryErrorList.RegistryError[i].severity);
                Console.WriteLine("codeContext: " + response.RegistryErrorList.RegistryError[i].codeContext);
                Console.WriteLine();
            }
            Assert.Fail();
        }

        [TestMethod()]
        public void RetrieveDocumentSetTest()
        {
            Logger.Log.Info($"Appel de la fonction : {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            _service.AddHeaders("urn:ihe:iti:2007:RetrieveDocumentSet");
            RetrieveDocumentSetRequestTypeDocumentRequest[] request = new[] { new RetrieveDocumentSetRequestTypeDocumentRequest() };

            request[0].DocumentUniqueId = Dmp.XDS_DOCUMENT_UNIQUE_ID;
            request[0].RepositoryUniqueId = "1.2.250.1.213.4.1.1.1.2";
            var response = _service.DocumentRepository_RetrieveDocumentSet(request);
            
            if (response.RegistryResponse.status.Equals("urn:oasis:names:tc:ebxml-regrep:ResponseStatusType:Success"))
            {
                Assert.IsTrue(true);
                return;
            }
            Console.WriteLine("Statut de la reponse: " + response.RegistryResponse.status);
            for (int i = 0; ((response.RegistryResponse.RegistryErrorList != null) && (i < response.RegistryResponse.RegistryErrorList.RegistryError.Length)); i++)
            {
                Console.WriteLine("Probleme:");
                Console.WriteLine("Value: " + response.RegistryResponse.RegistryErrorList.RegistryError[i].Value);
                Console.WriteLine("Code: " + response.RegistryResponse.RegistryErrorList.RegistryError[i].errorCode);
                Console.WriteLine("location: " + response.RegistryResponse.RegistryErrorList.RegistryError[i].location);
                Console.WriteLine("severity: " + response.RegistryResponse.RegistryErrorList.RegistryError[i].severity);
                Console.WriteLine("codeContext: " + response.RegistryResponse.RegistryErrorList.RegistryError[i].codeContext);
                Console.WriteLine();
            }
            Assert.Fail();
        }
    }
}