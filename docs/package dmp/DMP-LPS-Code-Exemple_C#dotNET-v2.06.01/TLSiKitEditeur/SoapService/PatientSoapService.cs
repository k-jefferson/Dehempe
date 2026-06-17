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
using TLSiKitEditeur.GDPService;
using TLSiKitEditeur.Messages;
using TLSiKitEditeur.PatientCertificationService;
using TLSiKitEditeur.PatientSpecificService;
using TLSiKitEditeur.PDQSupplierService;
using TLSiKitEditeur.SoapServiceHelpers;
using WebServiceResponse = TLSiKitEditeur.PatientSpecificService.WebServiceResponse;

namespace TLSiKitEditeur.SoapService
{

    public class PatientSoapService : IDisposable
    {
        private GDP_PortTypeClient _pService;
        private PatientSpecificEndPointClient _psService;
        private PatientCertificationEndPointClient _pcService;
        private PDQSupplier_PortTypeClient _pdqService;
        public PatientSoapService()
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
        public PRPA_IN201311UV02_ResponseType CreateDMP()
        {Logger.Log.Info($"Appel de la fonction : {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            _pService.AddHeaders("urn:hl7-org:v3:PRPA_IN201311UV02");
            PRPA_IN201311UV02_ResponseType rep;
            string errorCode="", errorText="";
            try
            {
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
                        }  
                    }

                    else
                    {
                        var r = new PRPA_IN201313UV02();
                        r =(PRPA_IN201313UV02) rep.Item;
                        errorCode = r.acknowledgement[0].acknowledgementDetail[0].code.code;
                        errorText = r.acknowledgement[0].acknowledgementDetail[0].text.Text[0];
                        if (r.acknowledgement[0].acknowledgementDetail[0].code.code == "DMPFound")
                        {
                            Logger.Log.Info("DMP Existe");
                        }
                        else
                        {
                            Logger.Log.Error("Erreur DMP");
                            Logger.Log.Error($"Code Erreur : {errorCode}");
                            Logger.Log.Error($"Message Erreur : {errorText}");
                        }
                    }

                    return rep;
                }
                Logger.Log.Error("Erreur Systéme");
            }
            catch (Exception e)
            {
                Logger.Log.Error(e.Message);
            }
            return null;
        }
        public PRPA_IN201314UV02_ResponseType CloseDMP()
        {
            Logger.Log.Info($"Appel de la fonction : {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            _pService.AddHeaders("urn:hl7-org:v3:PRPA_IN201314UV02");
            PRPA_IN201314UV02_ResponseType rep;
            string errorCode = "", errorText = "";
            try
            {
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
                        }
                        else
                        {
                            Logger.Log.Error("Erreur DMP");
                            Logger.Log.Error($"Code Erreur : {errorCode}");
                            Logger.Log.Error($"Message Erreur : {errorText}");
                        }
                    }

                    return rep;
                }
                Logger.Log.Error("Erreur Systéme");
            }
            catch (Exception e)
            {
                Logger.Log.Error(e.Message);
            }
            return null;
        }
        public PRPA_IN201314UV02_ResponseType ReactivateDMP()
        {
            Logger.Log.Info($"Appel de la fonction : {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            _pService.AddHeaders("urn:hl7-org:v3:PRPA_IN201314UV02");
            PRPA_IN201314UV02_ResponseType rep;
            string errorCode = "", errorText = "";
            try
            {
                rep = _pService.PatientRevise_PRPA_IN201314UV02(MessageBuilder.ReactivateHl7V3Message());

                if (rep != null)
                {
                    if (rep.Item.GetType() == typeof(PRPA_IN201315UV02))
                    {
                        var r = new PRPA_IN201315UV02();
                        r = (PRPA_IN201315UV02)rep.Item;
                        if (r.acknowledgement[0].typeCode == AcknowledgementType.AA)
                        {
                            Logger.Log.Info("DMP Réactiver");
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
                        }
                        else
                        {
                            Logger.Log.Error("Erreur DMP");
                            Logger.Log.Error($"Code Erreur : {errorCode}");
                            Logger.Log.Error($"Message Erreur : {errorText}");
                        }
                    }

                    return rep;
                }
                Logger.Log.Error("Erreur Systéme");
            }
            catch (Exception e)
            {
                Logger.Log.Error(e.Message);
            }
            return null;
        }
        public PRPA_IN201314UV02_ResponseType ModifyDataDMP()
        {
            Logger.Log.Info($"Appel de la fonction : {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            _pService.AddHeaders("urn:hl7-org:v3:PRPA_IN201314UV02");
            PRPA_IN201314UV02_ResponseType rep;
            string errorCode = "", errorText = "";
            try
            {
                rep = _pService.PatientRevise_PRPA_IN201314UV02(MessageBuilder.ModifyDataHl7V3Message());

                if (rep != null)
                {
                    if (rep.Item.GetType() == typeof(PRPA_IN201315UV02))
                    {
                        var r = new PRPA_IN201315UV02();
                        r = (PRPA_IN201315UV02)rep.Item;
                        if (r.acknowledgement[0].typeCode == AcknowledgementType.AA)
                        {
                            Logger.Log.Info("DMP modifieé");
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

                    return rep;
                }
                Logger.Log.Error("Erreur Systéme");
            }
            catch (Exception e)
            {
                Logger.Log.Error(e.Message);
            }
            return null;
        }

        public PRPA_IN201308UV02 GetDataDMP()
        {
            Logger.Log.Info($"Appel de la fonction : {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            _pService.AddHeaders("urn:hl7-org:v3:PRPA_IN201308UV02");
            PRPA_IN201308UV02 rep;
            try
            {
                rep = _pService.PatientGetDemographics_PRPA_IN201307UV02(MessageBuilder.GetDataHl7V3Message(Dmp.PatientIns));
         
                if (rep != null)
                {
                  
                    if (rep.acknowledgement[0].typeCode== AcknowledgementType.AA)
                    {
                        Logger.Log.Info("Consultation réussie");
                        return rep;
                    }
                }
                    Logger.Log.Info("Consultation en échec");

                
            }
            catch (Exception e)
            {
                Logger.Log.Error(e.Message);
            }
           

            return null;
        }
        public bool IsDMPExist()
        {
            Logger.Log.Info($"Appel de la fonction : {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            PRPA_IN201308UV02 rep;
            _pService.AddHeaders("urn:hl7-org:v3:PRPA_IN201308UV02");
            try
            {
                rep = _pService.PatientGetDemographics_PRPA_IN201307UV02(MessageBuilder.IsDMPExistHl7V3Message(Dmp.PatientIns));
                if (rep != null)
                {
                        if (rep.acknowledgement[0].typeCode == AcknowledgementType.AA)

                        {
                        String code = rep.controlActProcess.queryAck.queryResponseCode.code;
                        if (code.Equals("NF"))
                        {
                            Logger.Log.Info("DMP inexistant");
                        }
                        else if (code.Equals("OK"))
                        {
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

                            return true;
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

                        Logger.Log.Info($"Message Erreur:{rep.acknowledgement[0].acknowledgementDetail[0].text.Text[0]} ");   
                    }

                }

            } catch (Exception exp) {
                Logger.Log.Error(exp.StackTrace);
            }
            return false;
        }

        public PatientAccessResponse CreatePatientAccess(otpChannelTypeEnum channelType,string channel)
        {
            Logger.Log.Info($"Appel de la fonction : {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            _psService.AddHeaders("urn:si-dmp-patients:PatientSpecificEndPoint:createPatientAccess");
            PatientAccessResponse rep;
            try
            {
                rep = _psService.createPatientAccess(channelType, channel);
                if (rep != null)
                {
                    if(rep.status.ToUpper() == "DMPOK")
                    {
                        Logger.Log.Info($"Username : {rep.login}");
                        return rep;
                    }
                        Logger.Log.Info($"Error : {rep.context}");
                }
                else
                {
                    Logger.Log.Error("pas de réponse serveur");
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e.Message);
            }
            return null;
        }
        public PatientAccessResponse UpdatePatientAccess()
        {
            Logger.Log.Info($"Appel de la fonction : {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            _psService.AddHeaders("urn:si-dmp-patients:PatientSpecificEndPoint:updatePatientAccess");
            PatientAccessResponse rep;
            try
            {
                rep = _psService.updatePatientAccess();
                if (rep != null)
                {
                    if (rep.status.ToUpper() == "DMPOK")
                    {
                        Logger.Log.Info("updatePatientAcces réussie");
                        Logger.Log.Info($"Username : {rep.login}");
                        return rep;
                    }
                    Logger.Log.Info("updatePatientAcces en échec");
                    Logger.Log.Info($"Error : {rep.context}");
                }
                else
                {
                    Logger.Log.Error("pas de réponse serveur");
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e.Message);
            }
            return null;
        }
        public WebServiceResponse PatientOtpUpdate(otpChannelTypeEnum channelType,actionTypeEnum actionType,string channel)
        {
            Logger.Log.Info($"Appel de la fonction : {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            _psService.AddHeaders("urn:si-dmp-patients:PatientSpecificEndPoint:patientOTPUpdate");
            WebServiceResponse rep;
            try
            {
                rep = _psService.patientOTPUpdate(channelType, channel, actionType);
                if (rep != null)
                {
                    if (rep.status == "DMPOK")
                    {
                        Logger.Log.Info("OTP Update réussie");
                        Logger.Log.Info($"Context : {rep.context}");
                        return rep;
                    }else
                    {
                        Logger.Log.Info("OTP Update en échec");
                        Logger.Log.Info($"Error : {rep.context}");
                    }
                   
                }
                else
                {
                    Logger.Log.Error("pas de réponse serveur");
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e.Message);
            }
            return null;

          
        }
        public PatientListDataResponse PatientList(string date)
        {
            Logger.Log.Info($"Appel de la fonction : {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            _psService.AddHeaders("urn:si-dmp-patients:PatientSpecificEndPoint:patientList");
            PatientListDataResponse rep;
            try
            {
                rep = _psService.patientList(date, dateTypeEnum.LASTAUTORIZATION);
                if (rep != null)
                {
                    if (rep.status == "DMPOk")
                    {
                        if (rep.patientlistData.Length>0)
                        {
                            foreach (PatientListData patientListData in rep.patientlistData)
                            {
                                
                                Logger.Log.Info($"firstName: {patientListData.firstName}");
                                Logger.Log.Info($"lastName: {patientListData.lastName}");
                                Logger.Log.Info($"dateOfBirth: {patientListData.dateOfBirth}");
                                Logger.Log.Info($"message: {patientListData.message}");
                            }
                           
                        }else
                        Logger.Log.Info($"Liste de Patient Vide");
                        return rep;
                    }
                    Logger.Log.Error($"Error : {rep.status}");
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e.Message);
            }
            return null;
        }

        public PatientCertificationResponse FindNIR(String patientNirOd, String dateNaissance, int rangNaissance, String patientNirIndividu="")
        {
            _pcService.AddHeaders("urn:si-dmp-patients:PatientCertificationEndPoint:identityCertification");
            // appel de la fonction findNIR-identityCertification
            PatientCertificationResponse resp;
            try
            {
                resp = _pcService.identityCertification(patientNirOd, dateNaissance, rangNaissance, patientNirIndividu);
                if (resp.status == "DMPOK")
                {
                    Logger.Log.Info($"Context : {resp.context}");
                    Logger.Log.Info($"Nir Individu : {resp.nirIndividu}");
                    Logger.Log.Info($"Prenom : {resp.prenom}");
                    Logger.Log.Info($"Nom : {resp.nomPatronymique}");
                    Logger.Log.Info($"Date Naissance : {resp.dateNaissance}");
                    return resp;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e.Message);
            }   
            return null;
        }

        public PRPA_IN201306UV02 FindCandidatesQueryPatient()
        {
            _pdqService.AddHeaders("urn:ihe:iti:pdqv3:2007:PDQSupplier_Service:findCandidatesQuery"); 
            PRPA_IN201306UV02 rep;
            try
            {
                rep = _pdqService.findCandidatesQuery(MessageBuilder.FindCandidatesQueryH17V3Message(Dmp.PatientIns));
                if (rep != null)
                {
                    if (rep.acknowledgement[0].typeCode.code == "AA")
                    {
                        if (rep.controlActProcess.queryAck.queryResponseCode.code.Equals("OK"))
                        {
                            Logger.Log.Info("Candidats found");
                            return rep;
                        }
                    }
                }
                Logger.Log.Error("Erreur DMP");
                Logger.Log.Error($"Code Erreur : {rep.acknowledgement[0].acknowledgementDetail[0].code.code}");
                Logger.Log.Error($"Message Erreur : {rep.acknowledgement[0].acknowledgementDetail[0].text.Text[0]}");

            }
            catch (Exception e)
            {
                Logger.Log.Error(e.Message);
            }
            return null;
        }

        #region IDisposable Support
        private bool disposedValue = false; // Pour détecter les appels redondants

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ((IDisposable)_pService).Dispose();
                    ((IDisposable)_psService).Dispose();
                    ((IDisposable)_pcService).Dispose();
                    ((IDisposable)_pdqService).Dispose();
                }

                // TODO: libérer les ressources non managées (objets non managés) et remplacer un finaliseur ci-dessous.
                // TODO: définir les champs de grande taille avec la valeur Null.

                disposedValue = true;
            }
        }

        // TODO: remplacer un finaliseur seulement si la fonction Dispose(bool disposing) ci-dessus a du code pour libérer les ressources non managées.
        // ~PatientSoapService() {
        //   // Ne modifiez pas ce code. Placez le code de nettoyage dans Dispose(bool disposing) ci-dessus.
        //   Dispose(false);
        // }

        // Ce code est ajouté pour implémenter correctement le modèle supprimable.
        public void Dispose()
        {
            // Ne modifiez pas ce code. Placez le code de nettoyage dans Dispose(bool disposing) ci-dessus.
            Dispose(true);
            // TODO: supprimer les marques de commentaire pour la ligne suivante si le finaliseur est remplacé ci-dessus.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
