using Microsoft.VisualStudio.TestTools.UnitTesting;
using TLSiKitEditeur.SoapService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLSiKitEditeur.DocumentRegistryService;
using TLSiKitEditeur.SoapServiceHelpers;
using TLSiKitEditeur.Helpers;
using TLSiKitEditeur.CertificateHelpers;
using TLSiKitEditeur.Messages;

namespace TLSiKitEditeur.SoapService.Tests
{
    [TestClass()]
    public class DocumentRegistrySoapServiceTests
    {
        private DocumentRegistry_PortTypeClient _service;
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
            // creation du service
            _service = new DocumentRegistry_PortTypeClient();
            _service.Init("registry");
        }
       
        [TestMethod()]
        public void DeleteTest()
        {
            UpdateDocumentSetTest(MessageBuilder.DeleteMessage());
        }

        [TestMethod()]
        public void MaskTest()
        {
            UpdateDocumentSetTest(MessageBuilder.MaskMessage());
        }

        [TestMethod()]
        public void ArchiveTest()
        {
            UpdateDocumentSetTest(MessageBuilder.ArchiveMessage());
        }

        [TestMethod()]
        public void FindDocumentTest()
        {
            RegistryStoredQueryTest("Resources\\template_FindDocument.xml");
        }

        [TestMethod()]
        public void RegistryStoredQueryTest(string PathToXML)
        {
            Logger.Log.Info($"Appel de la fonction : {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            _service.AddHeaders("urn:ihe:iti:xds-b:2007:RegistryStoredQuery-b");
            var request = XDSb.LoadRegistryXdsbAdhocQueryRequest(PathToXML);
            var response = _service.DocumentRegistry_RegistryStoredQuery(request);
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
        public void UpdateDocumentSetTest(SubmitObjectsRequest req)
        {
            Logger.Log.Info($"Appel de la fonction : {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            _service.AddHeaders("urn:ihe:iti:xds-b:2007:UpdateDocumentSet-b");
            var response = _service.DocumentRegistry_UpdateDocumentSet(req);
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

    }
}