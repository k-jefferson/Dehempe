using System;
using adxpcity = TLSiKitEditeur.GDPService.adxpcity;
using adxpcountry = TLSiKitEditeur.GDPService.adxpcountry;
using adxppostalCode = TLSiKitEditeur.GDPService.adxppostalCode;
using adxpstreetAddressLine = TLSiKitEditeur.GDPService.adxpstreetAddressLine;
using ActClassControlAct = TLSiKitEditeur.GDPService.ActClassControlAct;
using AD = TLSiKitEditeur.GDPService.AD;
using ADXP = TLSiKitEditeur.GDPService.ADXP;
using BL = TLSiKitEditeur.GDPService.BL;
using CD = TLSiKitEditeur.GDPService.CD;
using CE = TLSiKitEditeur.GDPService.CE;
using CommunicationFunctionType = TLSiKitEditeur.GDPService.CommunicationFunctionType;
using COCT_MT090003UV01AssignedEntity = TLSiKitEditeur.GDPService.COCT_MT090003UV01AssignedEntity;
using COCT_MT090003UV01Person = TLSiKitEditeur.GDPService.COCT_MT090003UV01Person;
using COCT_MT150003UV03ContactParty = TLSiKitEditeur.GDPService.COCT_MT150003UV03ContactParty;
using COCT_MT150003UV03Organization = TLSiKitEditeur.GDPService.COCT_MT150003UV03Organization;
using CS = TLSiKitEditeur.GDPService.CS;
using enfamily = TLSiKitEditeur.GDPService.enfamily;
using engiven = TLSiKitEditeur.GDPService.engiven;
using enprefix = TLSiKitEditeur.GDPService.enprefix;
using ED = TLSiKitEditeur.GDPService.ED;
using EntityClassDevice = TLSiKitEditeur.GDPService.EntityClassDevice;
using EN = TLSiKitEditeur.GDPService.EN;
using ENXP = TLSiKitEditeur.GDPService.ENXP;
using II = TLSiKitEditeur.GDPService.II;
using IVL_TS = TLSiKitEditeur.GDPService.IVL_TS;
using MCAI_MT900001UV01DetectedIssueEvent = TLSiKitEditeur.GDPService.MCAI_MT900001UV01DetectedIssueEvent;
using MCCI_MT000100UV01Device = TLSiKitEditeur.GDPService.MCCI_MT000100UV01Device;
using MCCI_MT000100UV01Receiver = TLSiKitEditeur.GDPService.MCCI_MT000100UV01Receiver;
using MCCI_MT000100UV01Sender = TLSiKitEditeur.GDPService.MCCI_MT000100UV01Sender;
using ON = TLSiKitEditeur.GDPService.ON;
using ParticipationTargetSubject = TLSiKitEditeur.GDPService.ParticipationTargetSubject;
using PN = TLSiKitEditeur.GDPService.PN;
using RoleClassContact = TLSiKitEditeur.GDPService.RoleClassContact;
using SC = TLSiKitEditeur.GDPService.SC;
using ST = TLSiKitEditeur.GDPService.ST;
using SXCM_TS = TLSiKitEditeur.GDPService.SXCM_TS;
using TEL = TLSiKitEditeur.GDPService.TEL;
using TS = TLSiKitEditeur.GDPService.TS;
using WebServiceResponse = TLSiKitEditeur.PatientSpecificService.WebServiceResponse;
using x_ActMoodIntentEvent = TLSiKitEditeur.GDPService.x_ActMoodIntentEvent;
using TLSiKitEditeur.GDPService;
using TLSiKitEditeur.PDQSupplierService;
using TLSiKitEditeur.Helpers;
using TLSiKitEditeur.DocumentRegistryService;
using System.Xml.Serialization;
using System.IO;

namespace TLSiKitEditeur.Messages
{
    public class CVAMessageBuilder
    {
        
        public static SubmitObjectsRequest DeleteMessage()
        {
            SubmitObjectsRequest sor = new SubmitObjectsRequest();
            RegistryPackageType rpt = new RegistryPackageType();
            rpt.id = "SubmissionSet01";
            rpt.Slot = new SlotType1[] { new SlotType1() { name = "submissionTime", ValueList = new ValueListType() { Value = new string[] { DateTime.UtcNow.ToString("yyyyMMddhhmmss") } } } };
            rpt.Name = new InternationalStringType() { LocalizedString = new LocalizedStringType[] { new LocalizedStringType() { value = "Compte rendu", charset = "UTF8", lang = "FR" } } };
            rpt.Classification = new ClassificationType[] {
                new ClassificationType()
                {
                    nodeRepresentation="",
                    classifiedObject="SubmissionSet01",
                    classificationScheme="urn:uuid:a7058bb9-b4e4-4307-ba5b-e3f0ab85e12d",
                    id="cla80",
                    Slot = new SlotType1[]
                    {
                        new SlotType1()
                        {
                            name="authorPerson",
                            ValueList = new ValueListType()
                            {
                                Value=new string[] {"00B1022322^CARDIO-CH2232^FELIX^^^^^^&1.2.250.1.71.4.2.1&ISO^D^^^IDNPS"}
                            }
                        },
                         new SlotType1()
                        {
                            name="authorSpecialty",
                            ValueList = new ValueListType()
                            {
                                Value=new string[] { "G15_10/" + Dmp.SpecCode + "^" + Dmp.SpecDisplayName + "^1.2.250.1.213.1.1.4.5" }
                            }
                        },
                          new SlotType1()
                        {
                            name="authorInstitution",
                            ValueList = new ValueListType()
                            {
                                Value=new string[] { "HOPITAL DES 3 VALLEES00771^^^^^&1.2.250.1.71.4.2.2&ISO^IDNST^^^10B0167011" }
                            }
                        }
                    }
                },
                new ClassificationType()
                {
                    nodeRepresentation="04",
                    classifiedObject="SubmissionSet01",
                    classificationScheme="urn:uuid:aa543740-bdda-424e-8c96-df4873be8500",
                    id="cla81",
                    Slot = new SlotType1[]
                    {
                        new SlotType1()
                        {
                            name="codingScheme",
                            ValueList = new ValueListType()
                            {
                                Value=new string[] { "1.2.250.1.213.2.2" }
                            }
                        }
                    },
                    Name = new InternationalStringType()
                    {
                        LocalizedString = new LocalizedStringType[] {
                            new LocalizedStringType()
                            {
                                value="Hospitalisation de jour",
                                charset="UTF8",
                                lang="FR"
                            }
                        }
                    }
                },
                new ClassificationType()
                {
                    classificationNode="urn:uuid:a54d6aa5-d40d-43f9-88c5-b4633d873bdd",
                    classifiedObject="SubmissionSet01",
                    id="cla82",
                }
            };
            rpt.ExternalIdentifier = new ExternalIdentifierType[] {

                new ExternalIdentifierType()
                {
                    value=Dmp.patientNirOD+"^^20100907",
                    identificationScheme="urn:uuid:6b5aea1a-874d-4603-a4bc-96a0a7b38446",
                    id="ei37",
                    Name = new InternationalStringType()
                    {
                        LocalizedString = new LocalizedStringType[] {
                            new LocalizedStringType()
                            {
                                value="XDSSubmissionSet.patientId",
                                charset="UTF8",
                                lang="FR"
                            }
                        }
                    }
                },
                 new ExternalIdentifierType()
                {
                    value="1.2.250.1.999.1.1.7898",
                    identificationScheme="urn:uuid:554ac39e-e3fe-47fe-b233-965d2a147832",
                    id="ei38",
                    Name = new InternationalStringType()
                    {
                        LocalizedString = new LocalizedStringType[] {
                            new LocalizedStringType()
                            {
                                value="XDSSubmissionSet.sourceId",
                                charset="UTF8",
                                lang="FR"
                            }
                        }
                    }
                },
                  new ExternalIdentifierType()
                {

                    value="1.2.250.1.999.1.1.7898.1."+new Random().Next(100000000,999999999).ToString(),
                    identificationScheme="urn:uuid:96fdda7c-d067-4183-912e-bf5ee74998a8",
                    id="ei39",
                    Name = new InternationalStringType()
                    {
                        LocalizedString = new LocalizedStringType[] {
                            new LocalizedStringType()
                            {
                                value="XDSSubmissionSet.uniqueId",
                                charset="UTF8",
                                lang="FR"
                            }
                        }
                    }
                },

            };

            //-------------------
            AssociationType1 ass = new AssociationType1();
            ass.targetObject = "urn:uuid:7c0aa394-9bc4-11ec-9b14-0050569e422e";
            ass.sourceObject = "SubmissionSet01";
            ass.associationType = "urn:ihe:iti:2010:AssociationType:UpdateAvailabilityStatus";
            ass.objectType = "urn:oasis:names:tc:ebxml-regrep:ObjectType:RegistryObject:Association";
            ass.id = "association1";
            //-------------------
            SlotType1 slot1 = new SlotType1();
            slot1.name = "OriginalStatus";
            ValueListType vt = new ValueListType();
            vt.Value = new string[] { "urn:oasis:names:tc:ebxml-regrep:StatusType:Archived" };
            slot1.ValueList = vt;
            SlotType1 slot2 = new SlotType1();
            slot2.name = "NewStatus";
            ValueListType vt1 = new ValueListType();
            vt1.Value = new string[] { "urn:asip:ci-sis:2010:StatusType:Deleted" };
            slot2.ValueList = vt1;
            ass.Slot = new SlotType1[]
            {
                slot1,slot2
            };
            sor.RegistryObjectList = new RegistryObjectType[]
            {
                rpt,ass
            };
            XmlSerializer mySerializer = new XmlSerializer(typeof(DocumentRegistryService.SubmitObjectsRequest));
            StreamWriter myWriter = new StreamWriter("myFileName.xml");
            mySerializer.Serialize(myWriter, sor);
            myWriter.Close();


            return sor;
        }
    }
}
