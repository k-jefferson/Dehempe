using TLSiKitEditeur.SoapService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TLSiKitEditeur.CertificateHelpers;
using TLSiKitEditeur.GDPService;
using TLSiKitEditeur.Helpers;
using TLSiKitEditeur.PatientCertificationService;
using TLSiKitEditeur.PatientSpecificService;
using TLSiKitEditeur.PDQSupplierService;
using TLSiKitEditeur.SoapServiceHelpers;
using TLSiKitEditeur.Messages;

using System.Collections.Generic;

namespace TLSiKitEditeur.SoapService.Tests
{

    [TestClass()]
    public class PatientSoapServiceTests
    {
        private GDP_PortTypeClient _pService;
        private PatientSpecificEndPointClient _psService;
        private PatientCertificationEndPointClient _pcService;
        private PDQSupplier_PortTypeClient _pdqService;
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
            HabilitationSoapService service = new HabilitationSoapService();
            PatientSoapService ps = new PatientSoapService();
            DocumentRepositorySoapService rs = new DocumentRepositorySoapService();
            DocumentRegistrySoapService dr = new DocumentRegistrySoapService();
        }
        [TestInitialize]
        public void InitializeTests()
        {
            // creation du service
            _pService = new GDP_PortTypeClient();
            _pService.Init("patients");
            _psService = new PatientSpecificEndPointClient();
            _psService.Init("patientsSpecific");
            _pcService = new PatientCertificationEndPointClient();
            _pcService.Init("patientCertif");
            _pdqService = new PDQSupplier_PortTypeClient();
            _pdqService.Init("patientsPDQ");
        }

        [TestMethod]
        public void CreateDMPTest()
        {
            FindNIRTest();
            Logger.Log.Info($"Appel de la fonction : {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            _pService.AddHeaders("urn:hl7-org:v3:PRPA_IN201311UV02");
            PRPA_IN201311UV02_ResponseType rep;
            string errorCode = "", errorText = "";
            rep = _pService.PatientAdd_PRPA_IN201311UV02(MessageBuilder.CreateHl7V3Message());
            if (rep != null)
            {
                if (rep.Item.GetType() == typeof(PRPA_IN201312UV02))
                {
                    var r = new PRPA_IN201312UV02();
                    r = (PRPA_IN201312UV02)rep.Item;
                    if (r.acknowledgement[0].typeCode == AcknowledgementType.AA)
                    {
                        Logger.Log.Info("DMP crée");
                        Assert.IsTrue(true);
                        return;
                    }
                }

                else
                {
                    var r = new PRPA_IN201313UV02();
                    r = (PRPA_IN201313UV02)rep.Item;
                    errorCode = r.acknowledgement[0].acknowledgementDetail[0].code.code;
                    errorText = r.acknowledgement[0].acknowledgementDetail[0].text.Text[0];
                    if (r.acknowledgement[0].acknowledgementDetail[0].code.code == "DMPFound")
                    {
                        Logger.Log.Info("DMP Existe");
                        Assert.Inconclusive("DMP Existe");
                        return;
                    }
                    else
                    {
                        Logger.Log.Error("Erreur DMP");
                        Logger.Log.Error($"Code Erreur : {errorCode}");
                        Logger.Log.Error($"Message Erreur : {errorText}");
                    }
                }
            }
            Logger.Log.Error("Erreur DMP");
            Assert.Fail();
        }

        [TestMethod]
        public void IsDMPExistTest()
        {
            Logger.Log.Info($"Appel de la fonction : {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            PRPA_IN201308UV02 rep;
            _pService.AddHeaders("urn:hl7-org:v3:PRPA_IN201308UV02");
            rep = _pService.PatientGetDemographics_PRPA_IN201307UV02(MessageBuilder.IsDMPExistHl7V3Message(Dmp.PatientIns));
            if (rep != null)
            {
                if (rep.acknowledgement[0].typeCode == AcknowledgementType.AA)

                {
                    String code = rep.controlActProcess.queryAck.queryResponseCode.code;
                    if (code.Equals("NF"))
                    {
                        Logger.Log.Info("DMP inexistant");
                        Assert.Inconclusive("DMP inexistant");
                        return;
                    }
                    else if (code.Equals("OK"))
                    {
                        Assert.AreEqual(code, "OK");
                        Logger.Log.Info("DMP existant");
                        Logger.Log.Info($"Statut du DMP : {rep.controlActProcess.subject[0].registrationEvent.statusCode.code} ");
                        for (int i = 0; i < rep.controlActProcess.subject[0].registrationEvent.subject1.patient.id.Length; i++)
                        {
                            if (rep.controlActProcess.subject[0].registrationEvent.subject1.patient.id[i].root.Equals("1.2.250.1.213.1.4.10"))

                            {
                                Logger.Log.Info($"Patient INS-NIR : {rep.controlActProcess.subject[0].registrationEvent.subject1.patient.id[i].extension} ");
                            }
                            else if (rep.controlActProcess.subject[0].registrationEvent.subject1.patient.id[i].root.Equals("1.2.250.1.213.1.4.2"))
                            {
                                Logger.Log.Info($"Patient INS-C : {rep.controlActProcess.subject[0].registrationEvent.subject1.patient.id[i].extension} ");
                            }
                        }
                        Logger.Log.Info($"Statut du Patient(active/terminated) :{rep.controlActProcess.subject[0].registrationEvent.subject1.patient.statusCode.code} ");
                        return;
                    }
                    else
                    {
                        Logger.Log.Info($"erreur, valeur du statut inconnu : {code} ");
                    }
                }
                else
                {
                    Logger.Log.Info("Text d'existence en échec");
                    Logger.Log.Info($"Code Erreur: {rep.acknowledgement[0].acknowledgementDetail[0].code.code}");
                    Logger.Log.Info($"Message Erreur:{rep.acknowledgement[0].acknowledgementDetail[0].text.Text} ");
                }

            }
            Assert.Fail();
        }

        [TestMethod]
        public void CreatePatientAccessTest()
        {
            Logger.Log.Info($"Appel de la fonction : {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            _psService.AddHeaders("urn:si-dmp-patients:PatientSpecificEndPoint:createPatientAccess");
            PatientAccessResponse rep;

            rep = _psService.createPatientAccess(otpChannelTypeEnum.EMAIL, "alain@gmail.com");
            if (rep != null)
            {
                if (rep.status.ToUpper() == "DMPOK")
                {
                    Logger.Log.Info($"Username : {rep.login}");
                    Assert.IsTrue(true);
                    return;
                }
                else if (rep.status == "DMPPatientAccessAlreadyExists")
                {
                    Logger.Log.Info($"Error : {rep.context}");
                    Assert.Inconclusive("Access patient existe déja");
                    return;
                }
                Logger.Log.Info($"Error : {rep.context}");
            }
            else
            {
                Logger.Log.Error("pas de réponse serveur");
            }

            Assert.Fail();
        }

        [TestMethod]
        public void UpdatePatientAccessTest()
        {
            Logger.Log.Info($"Appel de la fonction : {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            _psService.AddHeaders("urn:si-dmp-patients:PatientSpecificEndPoint:updatePatientAccess");
            PatientAccessResponse rep;

            rep = _psService.updatePatientAccess();
            if (rep != null)
            {
                if (rep.status.ToUpper() == "DMPOK")
                {
                    Logger.Log.Info("updatePatientAcces réussie");
                    Logger.Log.Info($"Username : {rep.login}");
                    Assert.IsTrue(true);
                    return;
                }
                else
                {
                    Logger.Log.Info("updatePatientAcces en échec");
                    Logger.Log.Info($"Error : {rep.context}");

                }
            }
            else
            {
                Logger.Log.Error("pas de réponse serveur");
            }
            Assert.Fail();
        }

        [TestMethod]
        public void PatientOtpUpdateTest()
        {
            Logger.Log.Info($"Appel de la fonction : {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            _psService.AddHeaders("urn:si-dmp-patients:PatientSpecificEndPoint:patientOTPUpdate");
            PatientSpecificService.WebServiceResponse rep;
            //change actionTypeEnum to test this function if it says "Type de canal OTP deja Existant pour ce DMP"
            rep = _psService.patientOTPUpdate(otpChannelTypeEnum.EMAIL, "alain@gmail.com", actionTypeEnum.MODIFICATION);
            if (rep != null)
            {
                if (rep.status.ToUpper() == "DMPOK")
                {
                    Logger.Log.Info("OTP Update réussie");
                    Logger.Log.Info($"Context : {rep.context}");
                    Assert.IsTrue(true);
                    return;
                }
                else if (rep.status == "DMPPatientAccessOTPAlreadyExists")
                {
                    Logger.Log.Info("OTP Update en échec");
                    Logger.Log.Info($"{ rep.context}");
                    Assert.Inconclusive();
                    return;
                }
                else
                {
                    Logger.Log.Info("OTP Update en échec");
                    Logger.Log.Info($"Error : {rep.context}");
                }
            }
            else
            {
                Logger.Log.Error("pas de réponse serveur");
            }
            Assert.Fail();
        }

        [TestMethod]
        public void PatientListTest()
        {
            Logger.Log.Info($"Appel de la fonction : {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            _psService.AddHeaders("urn:si-dmp-patients:PatientSpecificEndPoint:patientList");
            PatientListDataResponse rep;
            rep = _psService.patientList("01012018", dateTypeEnum.LASTAUTORIZATION);
            if (rep != null)
            {
                if (rep.status.ToUpper() == "DMPOK")
                {
                    if (rep.patientlistData != null)
                    {
                        if (rep.patientlistData.Length > 0)
                        {
                            foreach (PatientListData patientListData in rep.patientlistData)
                            {
                                Logger.Log.Info($"firstName: {patientListData.firstName}");
                                Logger.Log.Info($"lastName: {patientListData.lastName}");
                                Logger.Log.Info($"dateOfBirth: {patientListData.dateOfBirth}");
                                Logger.Log.Info($"message: {patientListData.message}");
                            }
                            Assert.IsTrue(rep.patientlistData.Length > 0);
                            return;
                        }
                    }
                    Logger.Log.Info($"Liste de Patient Vide");
                    Assert.Inconclusive($"Liste de Patient Vide");
                    return;
                }
                Logger.Log.Error($"Error : {rep.status}");
            }
            Assert.Fail();
        }
        [TestMethod]
        public void FindNIRTest()
        {
            Logger.Log.Info($"Appel de la fonction : {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            _pcService.AddHeaders("urn:si-dmp-patients:PatientCertificationEndPoint:identityCertification");
            // appel de la fonction findNIR-identityCertification
            PatientCertificationResponse resp;

            resp = _pcService.identityCertification(Dmp.patientNirOD, Dmp.dateNaiss, 1, "");
            if (resp.status.ToUpper() == "DMPOK")
            {
                Logger.Log.Info($"Context : {resp.context}");
                Logger.Log.Info($"Nir Individu : {resp.nirIndividu}");
                Logger.Log.Info($"Prenom : {resp.prenom}");
                Logger.Log.Info($"Nom : {resp.nomPatronymique}");
                Logger.Log.Info($"Date Naissance : {resp.dateNaissance}");
                Assert.AreEqual(resp.status.ToUpper(), "DMPOK");
                return;
            }
            Assert.Fail();
        }
        [TestMethod]
        public void FindCandidatesQueryPatientTest()
        {
            Logger.Log.Info($"Appel de la fonction : {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            _pdqService.AddHeaders("urn:ihe:iti:pdqv3:2007:PDQSupplier_Service:findCandidatesQuery");
            PRPA_IN201306UV02 rep;
            rep = _pdqService.findCandidatesQuery(MessageBuilder.FindCandidatesQueryH17V3Message(Dmp.PatientIns));
            if (rep != null)
            {
                if (rep.acknowledgement[0].typeCode.code ==  "AA" )
                {
                    if (rep.controlActProcess.queryAck.queryResponseCode.code.Equals("OK"))
                    {
                        Logger.Log.Info("Candidats found");
                        Assert.IsTrue(true);
                        return;
                    }
                   
                }
            }
            Logger.Log.Error("Erreur DMP");
            Logger.Log.Error($"Code Erreur : {rep.acknowledgement[0].acknowledgementDetail[0].code.code}");
            Logger.Log.Error($"Message Erreur : {rep.acknowledgement[0].acknowledgementDetail[0].text.Text[0]}");
            Assert.Fail();
        }
        [TestMethod]
        public void ModifyDataDMPTest()
        {
            Logger.Log.Info($"Appel de la fonction : {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            _pService.AddHeaders("urn:hl7-org:v3:PRPA_IN201314UV02");
            PRPA_IN201314UV02_ResponseType rep;
            rep = _pService.PatientRevise_PRPA_IN201314UV02(MessageBuilder.ModifyDataHl7V3Message());
            string errorCode = "", errorText = "";
            if (rep != null)
            {

                if (rep.Item.GetType() == typeof(PRPA_IN201315UV02))
                {
                    var r = new PRPA_IN201315UV02();
                    r = (PRPA_IN201315UV02)rep.Item;
                    if (r.acknowledgement[0].typeCode == AcknowledgementType.AA)
                    {
                        Logger.Log.Info("DMP modifieé");
                        Assert.IsTrue(true);
                        return;
                    }
                }

                else
                {
                    var r = new PRPA_IN201316UV02();
                    r = (PRPA_IN201316UV02)rep.Item;
                    errorCode = r.acknowledgement[0].acknowledgementDetail[0].code.code;
                    errorText = r.acknowledgement[0].acknowledgementDetail[0].text.Text[0];
                    Logger.Log.Error("Erreur DMP");
                    Logger.Log.Error($"Code Erreur : {errorCode}");
                    Logger.Log.Error($"Message Erreur : {errorText}");

                }

            }
            else
                Logger.Log.Info("Pas de réponse serveur");

            Assert.Fail();
        }
        [TestMethod]
        public void CloseDMPTest()
        {
            Logger.Log.Info($"Appel de la fonction : {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            _pService.AddHeaders("urn:hl7-org:v3:PRPA_IN201314UV02");
            PRPA_IN201314UV02_ResponseType rep;
            string errorCode = "", errorText = "";
            rep = _pService.PatientRevise_PRPA_IN201314UV02(MessageBuilder.CloseHl7V3Message());
            if (rep != null)
            {
                if (rep.Item.GetType() == typeof(PRPA_IN201315UV02))
                {
                    var r = new PRPA_IN201315UV02();
                    r = (PRPA_IN201315UV02)rep.Item;
                    if (r.acknowledgement[0].typeCode == AcknowledgementType.AA)
                    {
                        Logger.Log.Info("DMP fermé");
                        Assert.IsTrue(true);
                        return;
                    }
                }

                else
                {
                    var r = new PRPA_IN201316UV02();
                    r = (PRPA_IN201316UV02)rep.Item;
                    errorCode = r.acknowledgement[0].acknowledgementDetail[0].code.code;
                    errorText = r.acknowledgement[0].acknowledgementDetail[0].text.Text[0];
                    if (r.acknowledgement[0].acknowledgementDetail[0].code.code == "DMPClosed")
                    {
                        Logger.Log.Info("DMP déja fermé");
                        Assert.Inconclusive("DMP déja fermé");
                        return;
                    }
                    else
                    {
                        Logger.Log.Error("Erreur DMP");
                        Logger.Log.Error($"Code Erreur : {errorCode}");
                        Logger.Log.Error($"Message Erreur : {errorText}");
                    }
                }
            }
            Logger.Log.Info("Pas de réponse serveur");
            Assert.Fail();
        }
        [TestMethod]
        public void ReactivateDMPTest()
        {
            Logger.Log.Info($"Appel de la fonction : {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            _pService.AddHeaders("urn:hl7-org:v3:PRPA_IN201314UV02");
            PRPA_IN201314UV02_ResponseType rep;
            rep = _pService.PatientRevise_PRPA_IN201314UV02(MessageBuilder.ReactivateHl7V3Message());
            string errorCode = "", errorText = "";
            if (rep != null)
            {
                if (rep.Item.GetType() == typeof(PRPA_IN201315UV02))
                {
                    var r = new PRPA_IN201315UV02();
                    r = (PRPA_IN201315UV02)rep.Item;
                    if (r.acknowledgement[0].typeCode == AcknowledgementType.AA)
                    {
                        Logger.Log.Info("DMP Réactiver");
                        Assert.IsTrue(true);
                        return;
                    }
                }
                else
                {
                    var r = new PRPA_IN201316UV02();
                    r = (PRPA_IN201316UV02)rep.Item;
                    errorCode = r.acknowledgement[0].acknowledgementDetail[0].code.code;
                    errorText = r.acknowledgement[0].acknowledgementDetail[0].text.Text[0];
                    if (r.acknowledgement[0].acknowledgementDetail[0].code.code == "DMPFound")
                    {
                        Logger.Log.Info("DMP Déja active");
                        Assert.Inconclusive("DMP déja active");
                        return;
                    }
                    else
                    {
                        Logger.Log.Error("Erreur DMP");
                        Logger.Log.Error($"Code Erreur : {errorCode}");
                        Logger.Log.Error($"Message Erreur : {errorText}");
                    
                      
                    }
                }
            }else
            Logger.Log.Info("Pas de réponse serveur");
            Assert.Fail();
        }

        [TestMethod()]
        public void GetDataDMPTest()
        {
            Logger.Log.Info($"Appel de la fonction : {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            _pService.AddHeaders("urn:hl7-org:v3:PRPA_IN201308UV02");
            PRPA_IN201308UV02 rep;
                rep = _pService.PatientGetDemographics_PRPA_IN201307UV02(MessageBuilder.GetDataHl7V3Message(Dmp.PatientIns));
                if (rep != null)
                {
                    if (rep.acknowledgement[0].typeCode == AcknowledgementType.AA)
                    {
                    Logger.Log.Info("Consultation réussie");
                    Assert.IsTrue(true);
                    return;
                    }
                }
                Logger.Log.Info("Consultation en échec");
            Assert.Fail();
        }







    }
}