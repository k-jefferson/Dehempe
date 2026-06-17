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
    public class MessageBuilder
    {
        public static PRPA_IN201311UV02 CreateHl7V3Message()
        {
            PRPA_IN201311UV02 hl7creationMessage = new PRPA_IN201311UV02();

            hl7creationMessage.ITSVersion = "XML_1.0";

            II messageId = new II();
            messageId.root = "1.2.250.1.999.1.1.356.3";
            messageId.extension = UUID.GenerateRandomUuid();
            hl7creationMessage.id = messageId;

            TS creationTime = new TS();
            creationTime.value = "20100528";
            hl7creationMessage.creationTime = creationTime;
            II interactionId = new II();
            interactionId.root = "2.16.840.1.113883.1.6";
            interactionId.extension = "PRPA_IN201311UV02";

            hl7creationMessage.interactionId = interactionId;
            CS cs = new CS();
            cs.code = "P";
            hl7creationMessage.processingCode = cs;
            CS cs1 = new CS();
            cs1.code = "T";
            hl7creationMessage.processingModeCode = cs1;
            CS cs2 = new CS();
            cs2.code = "AL";
            hl7creationMessage.acceptAckCode = cs2;

            ///////////////////////////////////////
            // ajout du sender
            ///////////////////////////////////////
            MCCI_MT000100UV01Sender sender = new MCCI_MT000100UV01Sender();
            sender.typeCode = CommunicationFunctionType.SND;
            MCCI_MT000100UV01Device senderDevice = new MCCI_MT000100UV01Device();
            senderDevice.classCode = EntityClassDevice.DEV;
            senderDevice.determinerCode = EntityDeterminerSpecific.INSTANCE;

            II id = new II();
            id.root = "1.2.250.1.999.1.1.356";
            senderDevice.id = new II[1] { id };
            SC sc = new SC();
            sc.Text = new string[] { "Nom du LPS" };
            senderDevice.softwareName = sc;
            sender.device = senderDevice;
            hl7creationMessage.sender = sender;

            ///////////////////////////////////////
            // ajout du receiver
            ///////////////////////////////////////
            MCCI_MT000100UV01Receiver receiver = new MCCI_MT000100UV01Receiver();
            receiver.typeCode = CommunicationFunctionType.RCV;
            MCCI_MT000100UV01Device receiverDevice = new MCCI_MT000100UV01Device();
            receiverDevice.classCode = EntityClassDevice.DEV;
            receiverDevice.determinerCode = EntityDeterminerSpecific.INSTANCE;
            II idReceiver = new II();
            idReceiver.root = "1.2.250.1.213.4.1.1.1";
            receiverDevice.id = new II[] { idReceiver };
            SC scReceiver = new SC();
            scReceiver.Text = new string[] { "DMP" };
            receiverDevice.softwareName = scReceiver;
            receiver.device = receiverDevice;
            hl7creationMessage.receiver = new MCCI_MT000100UV01Receiver[] { receiver };

            ///////////////////////////////////////
            // ajout des donnees du dossier patient
            ///////////////////////////////////////
            PRPA_IN201311UV02MFMI_MT700721UV01ControlActProcess control = new PRPA_IN201311UV02MFMI_MT700721UV01ControlActProcess();
            control.classCode = ActClassControlAct.CACT;
            control.moodCode = x_ActMoodIntentEvent.EVN;
            CE reasonCode = new CE();
            reasonCode.code = "CREA_RD";
            reasonCode.codeSystem = "1.2.250.1.213.1.1.4.11";
            reasonCode.displayName = "Création de dossier";
            control.reasonCode = new CE[] { reasonCode };

            PRPA_IN201311UV02MFMI_MT700721UV01Subject1 subject = new PRPA_IN201311UV02MFMI_MT700721UV01Subject1();
            subject.typeCode = ActRelationshipHasSubject.SUBJ;
            subject.contextConductionInd = false;

            PRPA_IN201311UV02MFMI_MT700721UV01RegistrationRequest registration = new PRPA_IN201311UV02MFMI_MT700721UV01RegistrationRequest();
            registration.classCode = ActClassRegistration.REG;
            registration.moodCode = ActMoodRequest.RQO;

            CS statusCode = new CS();
            statusCode.code = "active";
            registration.statusCode = statusCode;
            PRPA_IN201311UV02MFMI_MT700721UV01Subject2 subject1 = new PRPA_IN201311UV02MFMI_MT700721UV01Subject2();
            subject1.typeCode = ParticipationTargetSubject.SBJ;

            ////////////////////////////////
            // creation du patient
            ////////////////////////////////
            PRPA_MT201301UV02Patient patient = new PRPA_MT201301UV02Patient();
            II patientId = new II();
            // patientId.root = "1.2.250.1.213.1.4.2";
            patientId.root = "1.2.250.1.213.1.4.10";
            patientId.extension = (Dmp.PatientIns);
            patient.id = new II[] { patientId };
            patient.classCode = RoleClassPatient.PAT;
            patient.statusCode = statusCode;

            // ajout des donnees administratives
            PRPA_MT201301UV02Person patientPerson = new PRPA_MT201301UV02Person();
            patientPerson.classCode = EntityClass.PSN;
            patientPerson.determinerCode = EntityDeterminer.INSTANCE;

            // sexe
            CE sexe = new CE();
            sexe.code = "F";
            patientPerson.administrativeGenderCode = sexe;

            // prefixe
            PN name = new PN();

            enprefix prefix = new enprefix();
            prefix.Text = new String[] { "M" };

            name.Items = new ENXP[4];
            name.Items[0] = prefix;

            //noms 
            enfamily familySP = new enfamily();
            familySP.Text = new string[] { "PAT-TROIS" };
            familySP.qualifier = new EntityNamePartQualifier[1];
            familySP.qualifier[0] = EntityNamePartQualifier.SP;

            name.Items[1] = familySP;

            // prenom
            engiven given = new engiven();
            given.Text = new string[] { "DOMINIQUE" };

            name.Items[2] = given;

            patientPerson.name = new PN[1] { name };

            // telephone
            TEL tel = new TEL();
            tel.use = new TelecommunicationAddressUse[1];
            tel.use[0] = TelecommunicationAddressUse.HP;
            tel.value = "tel:0102030405";
            patientPerson.telecom = new TEL[1] { tel };

            // date de naissance
            TS birthday = new TS();
            birthday.value = "19790328";
            patientPerson.birthTime = birthday;

            // address
            AD address = new AD();

            adxpstreetAddressLine street = new adxpstreetAddressLine();
            street.Text = new string[] { "1, rue du Chat qui Peche" };
            address.Items = new ADXP[4];
            address.Items[0] = street;

            adxppostalCode postalCode = new adxppostalCode();
            postalCode.Text = new string[] { "75005" };
            address.Items[1] = postalCode;

            adxpcity city = new adxpcity();
            city.Text = new string[] { "Paris" };
            address.Items[2] = city;

            adxpcountry country = new adxpcountry();
            country.Text = new string[] { "France" };
            address.Items[3] = country;

            patientPerson.addr = new AD[1] { address };

            ///////////////////////////////////////
            // ajout des donnees de la carte vitale
            ///////////////////////////////////////
            PRPA_MT201301UV02ContactParty contactParty = new PRPA_MT201301UV02ContactParty();
            contactParty.classCode = RoleClassContact.CON;
            CE contact = new CE();

            contact.code = "CARTE_SESAM_VITALE";
            contact.codeSystem = "1.2.250.1.213.4.1.2.5";
            COCT_MT030207UV07Person contactPerson = new COCT_MT030207UV07Person();

            PN nameCV = new PN();

            // noms 
            enfamily familyBRCV = new enfamily();
            familyBRCV.Text = new string[] { "DURAND" };
            familyBRCV.qualifier = new EntityNamePartQualifier[1];
            familyBRCV.qualifier[0] = EntityNamePartQualifier.BR;
            nameCV.Items = new ENXP[3];
            nameCV.Items[0] = familyBRCV;

            enfamily familySPCV = new enfamily();
            familySPCV.Text = new string[] { "MARTINQUARANTESIX" };
            familySPCV.qualifier = new EntityNamePartQualifier[1];
            familySPCV.qualifier[0] = EntityNamePartQualifier.SP;

            nameCV.Items[1] = familySPCV;

            // prenom
            engiven givenCV = new engiven();
            givenCV.Text = new string[] { "Mariequasix" };
            nameCV.Items[2] = givenCV;

            // ajout des donnees au contact
            contactPerson.name = new PN[] { nameCV };

            // date de naissance
            TS birthdayContact = new TS();
            birthdayContact.value = "780801";
            contactPerson.birthTime = birthdayContact;

            // ajout des donnees administratives au patient

            contactParty.Item = contactPerson;
            patientPerson.contactParty = new PRPA_MT201301UV02ContactParty[1] { contactParty };
            patient.Item = patientPerson;
            COCT_MT150003UV03Organization provider = new COCT_MT150003UV03Organization();
            provider.classCode = EntityClassOrganization.ORG;
            provider.determinerCode = EntityDeterminerSpecific.INSTANCE;
            patient.providerOrganization = provider;

            ///////////////////////////////////////
            // recueil de consentements et oppositions
            ///////////////////////////////////////
            // recueil de consentement ouverture
            PRPA_MT201301UV02Subject4 consentDMP = new PRPA_MT201301UV02Subject4();
            consentDMP.typeCode = ParticipationTargetSubject.SBJ;
            PRPA_MT201301UV02AdministrativeObservation adminObser = new PRPA_MT201301UV02AdministrativeObservation();
            adminObser.classCode = ActClassObservation.OBS;
            adminObser.moodCode = ActMood.EVN;

            CD adminCode = new CD();
            adminCode.code = "CONSENTEMENT_OUVERTURE_DMP";
            adminCode.codeSystem = "1.2.250.1.213.4.1.2.3";
            adminCode.displayName = "Consentement a l'ouverture du DMP";
            adminObser.code = adminCode;

            BL bl = new BL();
            bl.value = true;
            bl.valueSpecified = true;

            adminObser.value = bl;
            SXCM_TS adminObserEffectiveTime = new SXCM_TS();
            adminObserEffectiveTime.value = Time.GetActualDate();

            adminObser.effectiveTime = new SXCM_TS[1] { adminObserEffectiveTime };
            consentDMP.administrativeObservation = adminObser;
            patient.subjectOf = new PRPA_MT201301UV02Subject4[3];
            patient.subjectOf[0] = consentDMP;

            // opposition au bris de glace
            PRPA_MT201301UV02Subject4 consentDMPBDG = new PRPA_MT201301UV02Subject4();
            consentDMPBDG.typeCode = ParticipationTargetSubject.SBJ;
            PRPA_MT201301UV02AdministrativeObservation adminObserBDG = new PRPA_MT201301UV02AdministrativeObservation();
            adminObserBDG.classCode = ActClassObservation.OBS;
            adminObserBDG.moodCode = ActMood.EVN;

            CD adminCodeBDG = new CD();
            adminCodeBDG.code = "OPPOSITION_BRIS_DE_GLACE";
            adminCodeBDG.codeSystem = "1.2.250.1.213.4.1.2.3";
            adminCodeBDG.displayName = "opposition au mode bris de glace";
            adminObserBDG.code = adminCodeBDG;

            BL blBDG = new BL();
            blBDG.value = false;
            blBDG.valueSpecified = true;
            adminObserBDG.value = blBDG;
            consentDMPBDG.administrativeObservation = adminObserBDG;
            patient.subjectOf[1] = consentDMPBDG;

            // opposition urgence
            PRPA_MT201301UV02Subject4 consentDMPURG = new PRPA_MT201301UV02Subject4();
            consentDMPURG.typeCode = ParticipationTargetSubject.SBJ;
            PRPA_MT201301UV02AdministrativeObservation adminObserURG = new PRPA_MT201301UV02AdministrativeObservation();
            adminObserURG.classCode = ActClassObservation.OBS;
            adminObserURG.moodCode = ActMood.EVN;

            CD adminCodeURG = new CD();
            adminCodeURG.code = "OPPOSITION_ACCES_URGENCE";
            adminCodeURG.codeSystem = "1.2.250.1.213.4.1.2.3";
            adminCodeURG.displayName = "opposition à l'accès au mode Urgence";
            adminObserURG.code = adminCodeURG;

            BL blURG = new BL();
            blURG.value = false;
            blURG.valueSpecified = true;
            adminObserURG.value = blURG;
            consentDMPURG.administrativeObservation = adminObserURG;
            patient.subjectOf[2] = consentDMPURG;

            subject1.patient = patient;
            registration.subject1 = subject1;

            ///////////////////////////////////////
            // Creation de l'auteur
            ///////////////////////////////////////
            MFMI_MT700721UV01Author2 author = new MFMI_MT700721UV01Author2();
            author.typeCode = ParticipationAuthorOriginator.AUT;

            COCT_MT090003UV01AssignedEntity assigned = new COCT_MT090003UV01AssignedEntity();
            assigned.classCode = RoleClassAssignedEntity.ASSIGNED;
            II assignedId = new II();
            assignedId.root = "1.2.250.1.71.4.2.1";
            assignedId.extension = Dmp.PSIDNAT_AUTH_INDIRECT;
            assigned.id = new II[] { assignedId };
            CE assignedCode = new CE();
            assignedCode.code = "G15_10/SM04";
            assignedCode.displayName = "Cardiologie et maladies vasculaires (SM)";

            assigned.code = assignedCode;
            COCT_MT090003UV01Person assignedPerson = new COCT_MT090003UV01Person();
            assignedPerson.determinerCode = EntityDeterminerSpecific.INSTANCE;
            assignedPerson.classCode = EntityClassPerson.PSN;

            PN nameMedecin = new PN();

            enprefix prefixDr = new enprefix();
            prefixDr.Text = new string[] { "Dr." };
            nameMedecin.Items = new ENXP[3];
            nameMedecin.Items[0] = prefixDr;

            enfamily familyBRAuth = new enfamily();
            familyBRAuth.Text = new string[] { "CARDIO-CH2232" };
            nameMedecin.Items[1] = familyBRAuth;

            engiven givenAuth = new engiven();
            givenAuth.Text = new string[] { "FELIX" };
            nameMedecin.Items[2] = givenAuth;
            assignedPerson.name = new EN[1] { nameMedecin };

            //Structure de l'auteur
            COCT_MT150003UV03Organization representedOrg = new COCT_MT150003UV03Organization();
            representedOrg.classCode = EntityClassOrganization.ORG;
            representedOrg.determinerCode = EntityDeterminerSpecific.INSTANCE;
            II IdOrg = new II();
            IdOrg.root = "1.2.250.1.71.4.2.2";
            IdOrg.extension = Dmp.ID_STRUCTURE_CPS;
            representedOrg.id = new II[] { IdOrg };
            ON orgName = new ON();
            orgName.Text = new string[] { "Cabinet Dr. CARDIO-CH2232" };
            representedOrg.name = new ON[] { orgName };
            //On crée un ContactParty vide
            COCT_MT150003UV03ContactParty contactPartyVide = new COCT_MT150003UV03ContactParty();
            contactPartyVide.classCode = RoleClassContact.CON;
            contactPartyVide.contactPerson = null;
            representedOrg.contactParty = new COCT_MT150003UV03ContactParty[] { contactPartyVide };
            //ajout structure à assignedEntity 
            assigned.representedOrganization = representedOrg;

            // ajout de l'auteur au message
            assigned.Item = assignedPerson;
            author.assignedEntity = assigned;
            registration.author = author;
            subject.registrationRequest = registration;
            control.subject = subject;
            hl7creationMessage.controlActProcess = control;

            return hl7creationMessage;
        }
        public static PRPA_IN201314UV02 CloseHl7V3Message()
        {
            PRPA_IN201314UV02 closeHl7V3Message = new PRPA_IN201314UV02();

            closeHl7V3Message.ITSVersion = "XML_1.0";

            II messageId = new II();
            messageId.root = "1.2.250.1.999.1.1.356.3";
            messageId.extension = UUID.GenerateRandomUuid();
            closeHl7V3Message.id = messageId;

            TS creationTime = new TS();
            creationTime.value = "20100528";
            closeHl7V3Message.creationTime = creationTime;
            II interactionId = new II();
            interactionId.root = "2.16.840.1.113883.1.6";
            interactionId.extension = "PRPA_IN201314UV02";

            closeHl7V3Message.interactionId = interactionId;
            CS cs = new CS();
            cs.code = "P";
            closeHl7V3Message.processingCode = cs;
            CS cs1 = new CS();
            cs1.code = "T";
            closeHl7V3Message.processingModeCode = cs1;
            CS cs2 = new CS();
            cs2.code = "AL";
            closeHl7V3Message.acceptAckCode = cs2;

            ///////////////////////////////////////
            // ajout du sender
            ///////////////////////////////////////
            MCCI_MT000100UV01Sender sender = new MCCI_MT000100UV01Sender();
            sender.typeCode = CommunicationFunctionType.SND;
            MCCI_MT000100UV01Device senderDevice = new MCCI_MT000100UV01Device();
            senderDevice.classCode = EntityClassDevice.DEV;
            senderDevice.determinerCode = EntityDeterminerSpecific.INSTANCE;

            II id = new II();
            id.root = "1.3.6.1.4.1.48364";
            senderDevice.id = new II[1] { id };
            SC sc = new SC();
            sc.Text = new string[] { "LPS GIE-SV" };
            senderDevice.softwareName = sc;
            sender.device = senderDevice;
            closeHl7V3Message.sender = sender;

            ///////////////////////////////////////
            // ajout du receiver
            ///////////////////////////////////////
            MCCI_MT000100UV01Receiver receiver = new MCCI_MT000100UV01Receiver();
            receiver.typeCode = CommunicationFunctionType.RCV;
            MCCI_MT000100UV01Device receiverDevice = new MCCI_MT000100UV01Device();
            receiverDevice.classCode = EntityClassDevice.DEV;
            receiverDevice.determinerCode = EntityDeterminerSpecific.INSTANCE;
            II idReceiver = new II();
            idReceiver.root = "1.2.250.1.213.4.1.1.1";
            receiverDevice.id = new II[] { idReceiver };
            SC scReceiver = new SC();
            scReceiver.Text = new string[] { "DMP" };
            receiverDevice.softwareName = scReceiver;
            receiver.device = receiverDevice;
            closeHl7V3Message.receiver = new MCCI_MT000100UV01Receiver[] { receiver };

            ///////////////////////////////////////
            // ajout des donnees du dossier patient
            ///////////////////////////////////////
            PRPA_IN201314UV02MFMI_MT700721UV01ControlActProcess control = new PRPA_IN201314UV02MFMI_MT700721UV01ControlActProcess();
            control.classCode = ActClassControlAct.CACT;
            control.moodCode = x_ActMoodIntentEvent.EVN;
            CE reasonCode = new CE();
            reasonCode.code = "FERM";
            reasonCode.codeSystem = "1.2.250.1.213.1.1.4.11";
            reasonCode.displayName = "Fermeture de dossier";
            control.reasonCode = new CE[] { reasonCode };

            PRPA_IN201314UV02MFMI_MT700721UV01Subject1 subject = new PRPA_IN201314UV02MFMI_MT700721UV01Subject1();
            subject.typeCode = ActRelationshipHasSubject.SUBJ;
            subject.contextConductionInd = false;

            PRPA_IN201314UV02MFMI_MT700721UV01RegistrationRequest registration = new PRPA_IN201314UV02MFMI_MT700721UV01RegistrationRequest();
            registration.classCode = ActClassRegistration.REG;
            registration.moodCode = ActMoodRequest.RQO;
            var value = new IVL_TS();
            value.value = "20100528";
            registration.effectiveTime = value;

            PRPA_MT201302UV02PatientstatusCode statusCode = new PRPA_MT201302UV02PatientstatusCode();
            statusCode.code = "terminated";
            CS statusCodeReg = new CS();
            statusCodeReg.code = "completed";
            registration.statusCode = statusCodeReg;
            PRPA_IN201314UV02MFMI_MT700721UV01Subject2 subject1 = new PRPA_IN201314UV02MFMI_MT700721UV01Subject2();
            subject1.typeCode = ParticipationTargetSubject.SBJ;

            ////////////////////////////////
            // creation du patient
            ////////////////////////////////
            PRPA_MT201302UV02Patient patient = new PRPA_MT201302UV02Patient();
            PRPA_MT201302UV02Patientid patientId = new PRPA_MT201302UV02Patientid();
            patientId.root = "1.2.250.1.213.1.4.10";
            patientId.extension = (Dmp.PatientIns);
            patient.id = new PRPA_MT201302UV02Patientid[] { patientId };
            patient.classCode = RoleClassPatient.PAT;
            patient.statusCode = statusCode;

            // ajout des donnees administratives
            PRPA_MT201302UV02Person patientPerson = new PRPA_MT201302UV02Person();
            patientPerson.classCode = EntityClass.PSN;
            patientPerson.determinerCode = EntityDeterminer.INSTANCE;


            // prefixe
            PN name = new PN();

            enprefix prefix = new enprefix();
            prefix.Text = new String[] { "M" };

            name.Items = new ENXP[4];
            name.Items[0] = prefix;

            //noms 
            enfamily familyBR = new enfamily();
            familyBR.Text = new string[] { "PAT-TROIS" };
            familyBR.qualifier = new EntityNamePartQualifier[1];
            familyBR.qualifier[0] = EntityNamePartQualifier.BR;

            name.Items[1] = familyBR;

            enfamily familySP = new enfamily();
            familySP.Text = new string[] { "PAT-TROIS" };
            familySP.qualifier = new EntityNamePartQualifier[1];
            familySP.qualifier[0] = EntityNamePartQualifier.SP;

            name.Items[2] = familySP;

            // prenom
            engiven given = new engiven();
            given.Text = new string[] { "DOMINIQUE" };

            name.Items[3] = given;

            patientPerson.name = new PN[1] { name };

            patient.Item = patientPerson;
            subject1.patient = patient;
            registration.subject1 = subject1;




            ///////////////////////////////////////
            // Creation de l'auteur
            ///////////////////////////////////////
            MFMI_MT700721UV01Author2 author = new MFMI_MT700721UV01Author2();
            author.typeCode = ParticipationAuthorOriginator.AUT;

            COCT_MT090003UV01AssignedEntity assigned = new COCT_MT090003UV01AssignedEntity();
            assigned.classCode = RoleClassAssignedEntity.ASSIGNED;
            II assignedId = new II();
            assignedId.root = "1.2.250.1.71.4.2.1";
            assignedId.extension = Dmp.PSIDNAT_AUTH_INDIRECT;
            assigned.id = new II[] { assignedId };
            CE assignedCode = new CE();
            assignedCode.code = "G15_10/SM04";
            assignedCode.displayName = "Cardiologie et maladies vasculaires (SM)";

            assigned.code = assignedCode;
            COCT_MT090003UV01Person assignedPerson = new COCT_MT090003UV01Person();
            assignedPerson.determinerCode = EntityDeterminerSpecific.INSTANCE;
            assignedPerson.classCode = EntityClassPerson.PSN;

            PN nameMedecin = new PN();

            enprefix prefixDr = new enprefix();
            prefixDr.Text = new string[] { "Dr." };
            nameMedecin.Items = new ENXP[3];
            nameMedecin.Items[0] = prefixDr;

            enfamily familyBRAuth = new enfamily();
            familyBRAuth.Text = new string[] { "MAX" };
            nameMedecin.Items[1] = familyBRAuth;

            engiven givenAuth = new engiven();
            givenAuth.Text = new string[] { "LIBRE" };
            nameMedecin.Items[2] = givenAuth;
            assignedPerson.name = new EN[1] { nameMedecin };

            //Structure de l'auteur
            COCT_MT150003UV03Organization representedOrg = new COCT_MT150003UV03Organization();
            representedOrg.classCode = EntityClassOrganization.ORG;
            representedOrg.determinerCode = EntityDeterminerSpecific.INSTANCE;
            II IdOrg = new II();
            IdOrg.root = "1.2.250.1.71.4.2.2";
            IdOrg.extension = Dmp.ID_STRUCTURE_CPS;
            representedOrg.id = new II[] { IdOrg };
            ON orgName = new ON();
            orgName.Text = new string[] { "CABINET DR LIBRE" };
            representedOrg.name = new ON[] { orgName };
            //On crée un ContactParty vide
            COCT_MT150003UV03ContactParty contactPartyVide = new COCT_MT150003UV03ContactParty();
            contactPartyVide.classCode = RoleClassContact.CON;
            representedOrg.contactParty = new COCT_MT150003UV03ContactParty[] { contactPartyVide };
            //ajout structure à assignedEntity 
            assigned.representedOrganization = representedOrg;

            // ajout de l'auteur au message
            assigned.Item = assignedPerson;
            author.assignedEntity = assigned;
            registration.author = author;
            subject.registrationRequest = registration;
            control.subject = subject;

            MFMI_MT700721UV01Reason reason = new MFMI_MT700721UV01Reason();
            reason.typeCode = ActRelationshipReason.RSON;
            MCAI_MT900001UV01DetectedIssueEvent detectedEvent = new MCAI_MT900001UV01DetectedIssueEvent();
            detectedEvent.classCode = ActClassDetectedIssue.ALRT;
            detectedEvent.moodCode = ActMoodEventOccurrence.EVN;
            CD code = new CD();
            code.code = "FERMETURE_DEMANDE_PATIENT";
            code.codeSystem = "1.2.250.1.213.4.1.2.4";
            detectedEvent.code = code;
            ED text = new ED();
            text.Text = new string[] { "Raison de cloture sur demande du patient" };
            detectedEvent.text = text;
            reason.detectedIssueEvent = detectedEvent;
            control.reasonOf = new[] { reason };
            closeHl7V3Message.controlActProcess = control;

            return closeHl7V3Message;
        }
        public static PRPA_IN201314UV02 ReactivateHl7V3Message()
        {
            PRPA_IN201314UV02 reactivateHl7V3Message = new PRPA_IN201314UV02();

            reactivateHl7V3Message.ITSVersion = "XML_1.0";

            II messageId = new II();
            messageId.root = "1.2.250.1.999.1.1.356.3";
            messageId.extension = UUID.GenerateRandomUuid();
            reactivateHl7V3Message.id = messageId;

            TS creationTime = new TS();
            creationTime.value = "20100528";
            reactivateHl7V3Message.creationTime = creationTime;
            II interactionId = new II();
            interactionId.root = "2.16.840.1.113883.1.6";
            interactionId.extension = "PRPA_IN201314UV02";

            reactivateHl7V3Message.interactionId = interactionId;
            CS cs = new CS();
            cs.code = "P";
            reactivateHl7V3Message.processingCode = cs;
            CS cs1 = new CS();
            cs1.code = "T";
            reactivateHl7V3Message.processingModeCode = cs1;
            CS cs2 = new CS();
            cs2.code = "AL";
            reactivateHl7V3Message.acceptAckCode = cs2;

            ///////////////////////////////////////
            // ajout du sender
            ///////////////////////////////////////
            MCCI_MT000100UV01Sender sender = new MCCI_MT000100UV01Sender();
            sender.typeCode = CommunicationFunctionType.SND;
            MCCI_MT000100UV01Device senderDevice = new MCCI_MT000100UV01Device();
            senderDevice.classCode = EntityClassDevice.DEV;
            senderDevice.determinerCode = EntityDeterminerSpecific.INSTANCE;

            II id = new II();
            id.root = "1.2.250.1.999.1.1.356";
            senderDevice.id = new II[1] { id };
            SC sc = new SC();
            sc.Text = new string[] { "Nom du LPS" };
            senderDevice.softwareName = sc;
            sender.device = senderDevice;
            reactivateHl7V3Message.sender = sender;

            ///////////////////////////////////////
            // ajout du receiver
            ///////////////////////////////////////
            MCCI_MT000100UV01Receiver receiver = new MCCI_MT000100UV01Receiver();
            receiver.typeCode = CommunicationFunctionType.RCV;
            MCCI_MT000100UV01Device receiverDevice = new MCCI_MT000100UV01Device();
            receiverDevice.classCode = EntityClassDevice.DEV;
            receiverDevice.determinerCode = EntityDeterminerSpecific.INSTANCE;
            II idReceiver = new II();
            idReceiver.root = "1.2.250.1.213.4.1.1.1";
            receiverDevice.id = new II[] { idReceiver };
            SC scReceiver = new SC();
            scReceiver.Text = new string[] { "DMP" };
            receiverDevice.softwareName = scReceiver;
            receiver.device = receiverDevice;
            reactivateHl7V3Message.receiver = new MCCI_MT000100UV01Receiver[] { receiver };

            ///////////////////////////////////////
            // ajout des donnees du dossier patient
            ///////////////////////////////////////
            PRPA_IN201314UV02MFMI_MT700721UV01ControlActProcess control = new PRPA_IN201314UV02MFMI_MT700721UV01ControlActProcess();
            control.classCode = ActClassControlAct.CACT;
            control.moodCode = x_ActMoodIntentEvent.EVN;
            CE reasonCode = new CE();
            reasonCode.code = "REAC";
            reasonCode.codeSystem = "1.2.250.1.213.1.1.4.11";
            reasonCode.displayName = "Réactivation de dossier";
            control.reasonCode = new CE[] { reasonCode };

            PRPA_IN201314UV02MFMI_MT700721UV01Subject1 subject = new PRPA_IN201314UV02MFMI_MT700721UV01Subject1();
            subject.typeCode = ActRelationshipHasSubject.SUBJ;
            
            subject.contextConductionInd = false;

            PRPA_IN201314UV02MFMI_MT700721UV01RegistrationRequest registration = new PRPA_IN201314UV02MFMI_MT700721UV01RegistrationRequest();
            registration.classCode = ActClassRegistration.REG;
            registration.moodCode = ActMoodRequest.RQO;
            PRPA_MT201302UV02PatientstatusCode statusCode = new PRPA_MT201302UV02PatientstatusCode();
            statusCode.code = "active";
            CS statusCodeReg = new CS();
            statusCodeReg.code = "active";
            registration.statusCode = statusCodeReg;
            PRPA_IN201314UV02MFMI_MT700721UV01Subject2 subject1 = new PRPA_IN201314UV02MFMI_MT700721UV01Subject2();
            subject1.typeCode = ParticipationTargetSubject.SBJ;

            ////////////////////////////////
            // creation du patient
            ////////////////////////////////
            PRPA_MT201302UV02Patient patient = new PRPA_MT201302UV02Patient();
            PRPA_MT201302UV02Patientid patientId = new PRPA_MT201302UV02Patientid();
            patientId.root = "1.2.250.1.213.1.4.10";
            patientId.extension = (Dmp.PatientIns);
            patient.id = new PRPA_MT201302UV02Patientid[] { patientId };
            patient.classCode = RoleClassPatient.PAT;
            patient.statusCode = statusCode;

            // ajout des donnees administratives
            PRPA_MT201302UV02Person patientPerson = new PRPA_MT201302UV02Person();
            patientPerson.classCode = EntityClass.PSN;
            patientPerson.determinerCode = EntityDeterminer.INSTANCE;

            // prefixe
            PN name = new PN();
            name.Items = new ENXP[3];
        
            //noms 
            enfamily familybr = new enfamily();
            familybr.Text = new string[] { "DURAND" };
            familybr.qualifier = new EntityNamePartQualifier[1];
            familybr.qualifier[0] = EntityNamePartQualifier.BR;
            name.Items[0] = familybr;
            enfamily familySP = new enfamily();
            familySP.Text = new string[] { "MARTINQUARANTESIX" };
            familySP.qualifier = new EntityNamePartQualifier[1];
            familySP.qualifier[0] = EntityNamePartQualifier.SP;
            name.Items[1] = familySP;
            
            // prenom
            engiven given = new engiven();
            given.Text = new string[] { "Mariequasix" };
            name.Items[2] = given;

            patientPerson.name = new PN[1] { name };

            patient.Item = patientPerson;

            // /////////////////////////////////////
            // recueil de consentements et oppositions
            // /////////////////////////////////////
            PRPA_MT201302UV02Subject4 consentDMPBDG = new PRPA_MT201302UV02Subject4();
            consentDMPBDG.typeCode = ParticipationTargetSubject.SBJ;
            PRPA_MT201302UV02AdministrativeObservation adminObserBDG = new PRPA_MT201302UV02AdministrativeObservation();
            adminObserBDG.classCode = ActClassObservation.OBS;
            adminObserBDG.moodCode = ActMood.EVN;

            CD adminCodeBDG = new CD();
            adminCodeBDG.code = "CONSENTEMENT_REACTIVATION_DMP";
            adminCodeBDG.codeSystem = "1.2.250.1.213.4.1.2.3";
            adminCodeBDG.displayName = "Consentement Ã  la réactivation du DMP";

            adminObserBDG.code = adminCodeBDG;
            SXCM_TS effectiveTime = new SXCM_TS();
            effectiveTime.value = "20100528";
            adminObserBDG.effectiveTime = new SXCM_TS[1] { effectiveTime };

            BL blBDG = new BL();
            blBDG.value = true;
            blBDG.valueSpecified = true;
            adminObserBDG.value = blBDG;

            consentDMPBDG.administrativeObservation = adminObserBDG;
            patient.subjectOf = new PRPA_MT201302UV02Subject4[1] { consentDMPBDG };
            subject1.patient = patient;
            registration.subject1 = subject1;

            ///////////////////////////////////////
            // Creation de l'auteur
            ///////////////////////////////////////
            MFMI_MT700721UV01Author2 author = new MFMI_MT700721UV01Author2();
            author.typeCode = ParticipationAuthorOriginator.AUT;
            COCT_MT090003UV01AssignedEntity assigned = new COCT_MT090003UV01AssignedEntity();
            assigned.classCode = RoleClassAssignedEntity.ASSIGNED;
            II assignedId = new II();
            assignedId.root = "1.2.250.1.71.4.2.1";
            assignedId.extension = "899900063480";
            assigned.id = new II[] { assignedId };
            CE assignedCode = new CE();
            assignedCode.code = "G15_10/SM04";
            assignedCode.displayName = "Cardiologie et maladies vasculaires (SM)";
            assignedCode.codeSystem = "1.2.250.1.213.1.1.4.5";
            assigned.code = assignedCode;
            COCT_MT090003UV01Person assignedPerson = new COCT_MT090003UV01Person();
            assignedPerson.determinerCode = EntityDeterminerSpecific.INSTANCE;
            assignedPerson.classCode = EntityClassPerson.PSN;
            PN nameMedecin = new PN();
            enprefix prefixDr = new enprefix();
            prefixDr.Text = new string[] { "Dr." };
            nameMedecin.Items = new ENXP[3];
            nameMedecin.Items[0] = prefixDr;
            enfamily familyBRAuth = new enfamily();
            familyBRAuth.Text = new string[] { "CARDIO-CH2232" };
            nameMedecin.Items[1] = familyBRAuth;

            engiven givenAuth = new engiven();
            givenAuth.Text = new string[] { "FELIX" };
            nameMedecin.Items[2] = givenAuth;
            assignedPerson.name = new EN[1] { nameMedecin };

            //Structure de l'auteur
            COCT_MT150003UV03Organization representedOrg = new COCT_MT150003UV03Organization();
            representedOrg.classCode = EntityClassOrganization.ORG;
            representedOrg.determinerCode = EntityDeterminerSpecific.INSTANCE;
            II IdOrg = new II();
            IdOrg.root = "1.2.250.1.71.4.2.2";
            IdOrg.extension = "10B0167011";
            representedOrg.id = new II[] { IdOrg };
            ON orgName = new ON();
            orgName.Text = new string[] { "NOM-STRUCTURE-EXEMPLE-KIT-EDITEUR-A-REMPLACER" };
            representedOrg.name = new ON[] { orgName };
            //On crée un ContactParty vide
            COCT_MT150003UV03ContactParty contactPartyVide = new COCT_MT150003UV03ContactParty();
            contactPartyVide.classCode = RoleClassContact.CON;
            representedOrg.contactParty = new COCT_MT150003UV03ContactParty[] { contactPartyVide };
            //ajout structure à assignedEntity 
            assigned.representedOrganization = representedOrg;
            // ajout de l'auteur au message
            assigned.Item = assignedPerson;
            author.assignedEntity = assigned;
            registration.author = author;
            subject.registrationRequest = registration;
            control.subject = subject;
            reactivateHl7V3Message.controlActProcess = control;
            return reactivateHl7V3Message;
        }
        public static PRPA_IN201314UV02 ModifyDataHl7V3Message()
        {
            PRPA_IN201314UV02 modifyHl7V3Message = new PRPA_IN201314UV02();

            modifyHl7V3Message.ITSVersion = "XML_1.0";

            II messageId = new II();
            messageId.root = "1.3.6.1.4.1.48364.1.1";
            messageId.extension = UUID.GenerateRandomUuid();
            modifyHl7V3Message.id = messageId;

            TS creationTime = new TS();
            creationTime.value = "20100528";
            modifyHl7V3Message.creationTime = creationTime;
            II interactionId = new II();
            interactionId.root = "2.16.840.1.113883.1.6";
            interactionId.extension = "PRPA_IN201314UV02";

            modifyHl7V3Message.interactionId = interactionId;
            CS cs = new CS();
            cs.code = "D";
            modifyHl7V3Message.processingCode = cs;
            CS cs1 = new CS();
            cs1.code = "T";
            modifyHl7V3Message.processingModeCode = cs1;
            CS cs2 = new CS();
            cs2.code = "AL";
            modifyHl7V3Message.acceptAckCode = cs2;

            ///////////////////////////////////////
            // ajout du sender
            ///////////////////////////////////////
            MCCI_MT000100UV01Sender sender = new MCCI_MT000100UV01Sender();
            sender.typeCode = CommunicationFunctionType.SND;
            MCCI_MT000100UV01Device senderDevice = new MCCI_MT000100UV01Device();
            senderDevice.classCode = EntityClassDevice.DEV;
            senderDevice.determinerCode = EntityDeterminerSpecific.INSTANCE;

            II id = new II();
            id.root = "1.3.6.1.4.1.48364";
            senderDevice.id = new II[1] { id };
            SC sc = new SC();
            sc.Text = new string[] { "Nom du LPS" };
            senderDevice.softwareName = sc;
            sender.device = senderDevice;
            modifyHl7V3Message.sender = sender;

            ///////////////////////////////////////
            // ajout du receiver
            ///////////////////////////////////////
            MCCI_MT000100UV01Receiver receiver = new MCCI_MT000100UV01Receiver();
            receiver.typeCode = CommunicationFunctionType.RCV;
            MCCI_MT000100UV01Device receiverDevice = new MCCI_MT000100UV01Device();
            receiverDevice.classCode = EntityClassDevice.DEV;
            receiverDevice.determinerCode = EntityDeterminerSpecific.INSTANCE;
            II idReceiver = new II();
            idReceiver.root = "1.2.250.1.213.4.1.1.1";
            receiverDevice.id = new II[] { idReceiver };
            SC scReceiver = new SC();
            scReceiver.Text = new string[] { "DMP" };
            receiverDevice.softwareName = scReceiver;
            receiver.device = receiverDevice;
            modifyHl7V3Message.receiver = new MCCI_MT000100UV01Receiver[] { receiver };

            ///////////////////////////////////////
            // ajout des donnees du dossier patient
            ///////////////////////////////////////
            PRPA_IN201314UV02MFMI_MT700721UV01ControlActProcess control = new PRPA_IN201314UV02MFMI_MT700721UV01ControlActProcess();
            control.classCode = ActClassControlAct.CACT;
            control.moodCode = x_ActMoodIntentEvent.EVN;
            CE reasonCode = new CE();
            reasonCode.code = "MODIF_DATA";
            reasonCode.codeSystem = "1.2.250.1.213.1.1.4.11";
            reasonCode.displayName = "Modification des données de gestion du dossier";
            control.reasonCode = new CE[] { reasonCode };
            PRPA_IN201314UV02MFMI_MT700721UV01Subject1 subject = new PRPA_IN201314UV02MFMI_MT700721UV01Subject1();
            subject.typeCode = ActRelationshipHasSubject.SUBJ;
            subject.contextConductionInd = false;

            PRPA_IN201314UV02MFMI_MT700721UV01RegistrationRequest registration = new PRPA_IN201314UV02MFMI_MT700721UV01RegistrationRequest();
            registration.classCode = ActClassRegistration.REG;
            registration.moodCode = ActMoodRequest.RQO;
            PRPA_MT201302UV02PatientstatusCode statusCode = new PRPA_MT201302UV02PatientstatusCode();
            statusCode.code = "active";
            CS statusCodeReg = new CS();
            statusCodeReg.code = "active";
            registration.statusCode = statusCodeReg;
            PRPA_IN201314UV02MFMI_MT700721UV01Subject2 subject1 = new PRPA_IN201314UV02MFMI_MT700721UV01Subject2();
            subject1.typeCode = ParticipationTargetSubject.SBJ;

            ////////////////////////////////
            // creation du patient
            ////////////////////////////////
            PRPA_MT201302UV02Patient patient = new PRPA_MT201302UV02Patient();
            PRPA_MT201302UV02Patientid patientId = new PRPA_MT201302UV02Patientid();
            patientId.root = "1.2.250.1.213.1.4.10";
            patientId.extension = (Dmp.PatientIns);
            patient.id = new PRPA_MT201302UV02Patientid[] { patientId };
            patient.classCode = RoleClassPatient.PAT;
            patient.statusCode = statusCode;

            // ajout des donnees administratives
            PRPA_MT201302UV02Person patientPerson = new PRPA_MT201302UV02Person();
            patientPerson.classCode = EntityClass.PSN;
            patientPerson.determinerCode = EntityDeterminer.INSTANCE;

            // sexe
            CE sexe = new CE();
            sexe.code = "F";
            patientPerson.administrativeGenderCode = sexe;

            // prefixe
            PN name = new PN();
            enprefix prefix = new enprefix();
            prefix.Text = new String[] { "M" };
            name.Items = new ENXP[4];
            name.Items[0] = prefix;

            //noms 
            enfamily familySP = new enfamily();
            familySP.Text = new string[] { "PAT-TROIS" };
            familySP.qualifier = new EntityNamePartQualifier[1];
            familySP.qualifier[0] = EntityNamePartQualifier.SP;
            name.Items[1] = familySP;
            // prenom
            engiven given = new engiven();
            given.Text = new string[] { "DOMINIQUE" };
            name.Items[2] = given;
            patientPerson.name = new PN[1] { name };
            // telephone
            TEL tel = new TEL();
            tel.use = new TelecommunicationAddressUse[1];
            tel.use[0] = TelecommunicationAddressUse.HP;
            tel.value = "tel:0102030405";
            TEL tel1 = new TEL();
            tel1.use = new TelecommunicationAddressUse[1];
            tel1.use[0] = TelecommunicationAddressUse.MC;
            tel1.value = "tel:0607080901";
            patientPerson.telecom = new TEL[2] { tel,tel1 };
            // date de naissance
            TS birthday = new TS();
            birthday.value = "19790328";
            patientPerson.birthTime = birthday;
            // address
            AD address = new AD();
            adxpstreetAddressLine street = new adxpstreetAddressLine();
            street.Text = new string[] { "1, rue du Chat qui Peche" };
            address.Items = new ADXP[4];
            address.Items[0] = street;
            adxppostalCode postalCode = new adxppostalCode();
            postalCode.Text = new string[] { "75005" };
            address.Items[1] = postalCode;
            adxpcity city = new adxpcity();
            city.Text = new string[] { "Paris" };
            address.Items[2] = city;

            adxpcountry country = new adxpcountry();
            country.Text = new string[] { "France" };
            address.Items[3] = country;
            patientPerson.addr = new AD[1] { address };
            patient.Item = patientPerson;
            subject1.patient = patient;
            registration.subject1 = subject1;
            ///////////////////////////////////////
            // Creation de l'auteur
            ///////////////////////////////////////
            MFMI_MT700721UV01Author2 author = new MFMI_MT700721UV01Author2();
            author.typeCode = ParticipationAuthorOriginator.AUT;

            COCT_MT090003UV01AssignedEntity assigned = new COCT_MT090003UV01AssignedEntity();
            assigned.classCode = RoleClassAssignedEntity.ASSIGNED;
            II assignedId = new II();
            assignedId.root = "1.2.250.1.71.4.2.1";
            assignedId.extension = "00B1022322";
            assigned.id = new II[] { assignedId };
            CE assignedCode = new CE();
            assignedCode.code = "G15_10/SM04";
            assignedCode.displayName = "Cardiologie et maladies vasculaires (SM)";
            assignedCode.codeSystem = "1.2.250.1.213.1.1.4.5";

            assigned.code = assignedCode;
            COCT_MT090003UV01Person assignedPerson = new COCT_MT090003UV01Person();
            assignedPerson.determinerCode = EntityDeterminerSpecific.INSTANCE;
            assignedPerson.classCode = EntityClassPerson.PSN;
            PN nameMedecin = new PN();
            enprefix prefixDr = new enprefix();
            prefixDr.Text = new string[] { "Dr." };
            nameMedecin.Items = new ENXP[3];
            nameMedecin.Items[0] = prefixDr;
            enfamily familyBRAuth = new enfamily();
            familyBRAuth.Text = new string[] { "CARDIO-CH2232" };
            nameMedecin.Items[1] = familyBRAuth;
            engiven givenAuth = new engiven();
            givenAuth.Text = new string[] { "FELIX" };
            nameMedecin.Items[2] = givenAuth;
            assignedPerson.name = new EN[1] { nameMedecin };
            //Structure de l'auteur
            COCT_MT150003UV03Organization representedOrg = new COCT_MT150003UV03Organization();
            representedOrg.classCode = EntityClassOrganization.ORG;
            representedOrg.determinerCode = EntityDeterminerSpecific.INSTANCE;
            II IdOrg = new II();
            IdOrg.root = "1.2.250.1.71.4.2.2";
            IdOrg.extension = "00B102232200";
            representedOrg.id = new II[] { IdOrg };
            ON orgName = new ON();
            orgName.Text = new string[] { "Cabinet Dr. CARDIO-CH2232" };
            representedOrg.name = new ON[] { orgName };
            //On crée un ContactParty vide
            COCT_MT150003UV03ContactParty contactPartyVide = new COCT_MT150003UV03ContactParty();
            contactPartyVide.classCode = RoleClassContact.CON;
            contactPartyVide.contactPerson = null;
            representedOrg.contactParty = new COCT_MT150003UV03ContactParty[] { contactPartyVide };
            //ajout structure à assignedEntity 
            assigned.representedOrganization = representedOrg;
            // ajout de l'auteur au message
            assigned.Item = assignedPerson;
            author.assignedEntity = assigned;
            registration.author = author;
            subject.registrationRequest = registration;
            control.subject = subject;
            modifyHl7V3Message.controlActProcess = control;
            return modifyHl7V3Message;
        }
        public static PRPA_IN201305UV02 FindCandidatesQueryH17V3Message(string patientIns)
        {
            PRPA_IN201305UV02 H17V3Message = new PRPA_IN201305UV02();
            //  /////////////////////////////////////
            //  creation de l'enveloppe du message HL7
            //  /////////////////////////////////////
            H17V3Message.ITSVersion = "XML_1.0";
            PDQSupplierService.II messageId = new PDQSupplierService.II();
            messageId.root = "1.2.250.1.999.1.1.356.3";
            messageId.extension = UUID.GenerateRandomUuid();
            H17V3Message.id = messageId;
            PDQSupplierService.TS creationTime = new PDQSupplierService.TS();
            creationTime.value = "20100528";
            H17V3Message.creationTime = creationTime;
            PDQSupplierService.II interactionId = new PDQSupplierService.II();
            interactionId.root = "2.16.840.1.113883.1.6";
            interactionId.extension = "PRPA_IN201305UV02";
            H17V3Message.interactionId = interactionId;
            PDQSupplierService.CS cs = new PDQSupplierService.CS();
            cs.code = "P";
            H17V3Message.processingCode = cs;
            PDQSupplierService.CS cs1 = new PDQSupplierService.CS();
            cs1.code = "T";
            H17V3Message.processingModeCode = cs1;
            PDQSupplierService.CS cs2 = new PDQSupplierService.CS();
            cs2.code = "AL";
            H17V3Message.acceptAckCode = cs2;
            PDQSupplierService.MCCI_MT000100UV01Sender sender = new PDQSupplierService.MCCI_MT000100UV01Sender();
            sender.typeCode = PDQSupplierService.CommunicationFunctionType.SND;
            PDQSupplierService.MCCI_MT000100UV01Device senderDevice = new PDQSupplierService.MCCI_MT000100UV01Device();
            senderDevice.classCode = PDQSupplierService.EntityClassDevice.DEV;
            senderDevice.determinerCode = EntityDeterminerSpecific.INSTANCE.ToString();
            PDQSupplierService.II id = new PDQSupplierService.II();
            id.root = "1.2.250.1.999.1.1.356";
            senderDevice.id = new PDQSupplierService.II[] { id };
            PDQSupplierService.SC sc = new PDQSupplierService.SC();
            sc.Text = new string[] { "Nom du LPS" };
            senderDevice.softwareName = sc;
            sender.device = senderDevice;
            H17V3Message.sender = sender;

            PDQSupplierService.MCCI_MT000100UV01Receiver receiver = new PDQSupplierService.MCCI_MT000100UV01Receiver();
            receiver.typeCode = PDQSupplierService.CommunicationFunctionType.RCV;
            PDQSupplierService.MCCI_MT000100UV01Device receiverDevice = new PDQSupplierService.MCCI_MT000100UV01Device();
            receiverDevice.classCode = PDQSupplierService.EntityClassDevice.DEV;
            receiverDevice.determinerCode = EntityDeterminerSpecific.INSTANCE.ToString();
            PDQSupplierService.II idReceiver = new PDQSupplierService.II();
            idReceiver.root = "1.2.250.1.213.4.1.1.1";
            receiverDevice.id = new PDQSupplierService.II[] { idReceiver };
            PDQSupplierService.SC scReceiver = new PDQSupplierService.SC();
            scReceiver.Text = new string[] { "DMP" };
            receiverDevice.softwareName = scReceiver;
            receiver.device = receiverDevice;
            H17V3Message.receiver = new PDQSupplierService.MCCI_MT000100UV01Receiver[] { receiver };

            PRPA_IN201305UV02QUQI_MT021001UV01ControlActProcess controlActProcess = new PRPA_IN201305UV02QUQI_MT021001UV01ControlActProcess();

            controlActProcess.classCode = PDQSupplierService.ActClassControlAct.CACT;
            controlActProcess.moodCode = PDQSupplierService.x_ActMoodIntentEvent.EVN;
            PDQSupplierService.CD code = new PDQSupplierService.CD();
            code.code = "PRPA_TE201305UV02";
            code.codeSystem = "2.16.840.1.113883.1.6";
            controlActProcess.code = code;

            PRPA_MT201306UV02QueryByParameter queryByParameter = new PRPA_MT201306UV02QueryByParameter();
            PDQSupplierService.II queryId = new PDQSupplierService.II();
            queryId.extension = UUID.GenerateRandomUuid();
            queryId.root = "1.3.6.1.4.1.48364.2";
            queryByParameter.queryId = queryId;
            PDQSupplierService.CS statusCode = new PDQSupplierService.CS();
            statusCode.code = "new";
            queryByParameter.statusCode = statusCode;
            


            PRPA_MT201306UV02ParameterList parameterList = new PRPA_MT201306UV02ParameterList();
            PRPA_MT201306UV02LivingSubjectName livingSubjectName = new PRPA_MT201306UV02LivingSubjectName();
            //todo : check this !!
            PDQSupplierService.EN value = new PDQSupplierService.EN();
            value.Items = new PDQSupplierService.ENXP[1];
            PDQSupplierService.enfamily familySP = new PDQSupplierService.enfamily();
            familySP.Text = new string[] { "DECHAINE" };
            value.Items[0] = familySP;


            livingSubjectName.value = new[] { value };

            PDQSupplierService.ST semanticsText = new PDQSupplierService.ST();
            semanticsText.Text = new string[] { "LivingSubject.name" };
            livingSubjectName.semanticsText = semanticsText;

            parameterList.livingSubjectName = new[] { livingSubjectName };


            queryByParameter.parameterList = parameterList;
            //PRPA_MT201306UV02QueryByParameter queryByParameterJax = new PRPA_MT201306UV02QueryByParameter();
            //queryByParameterJax = queryByParameter;
            controlActProcess.queryByParameter = queryByParameter;
            H17V3Message.controlActProcess = controlActProcess;
            return H17V3Message;
        }
        public static PRPA_IN201307UV02 GetDataHl7V3Message(string patientIns)
        {
            PRPA_IN201307UV02 H17V3Message = new PRPA_IN201307UV02();
            //  /////////////////////////////////////
            //  creation de l'enveloppe du message HL7
            //  /////////////////////////////////////
            H17V3Message.ITSVersion = "XML_1.0";
            II messageId = new II();
            messageId.root = "1.2.250.1.999.1.1.356.3";
            messageId.extension = UUID.GenerateRandomUuid();
            H17V3Message.id = messageId;
            TS creationTime = new TS();
            creationTime.value = "20100528";
            H17V3Message.creationTime = creationTime;
            II interactionId = new II();
            interactionId.root = "2.16.840.1.113883.1.6";
            interactionId.extension = "PRPA_IN201307UV02";
            H17V3Message.interactionId = interactionId;
            CS cs = new CS();
            cs.code = "P";
            H17V3Message.processingCode = cs;
            CS cs1 = new CS();
            cs1.code = "T";
            H17V3Message.processingModeCode = cs1;
            CS cs2 = new CS();
            cs2.code = "AL";
            H17V3Message.acceptAckCode = cs2;
            // /////////////////////////////////////
            // ajout du sender
            // /////////////////////////////////////
            MCCI_MT000100UV01Sender sender = new MCCI_MT000100UV01Sender();
            sender.typeCode = CommunicationFunctionType.SND;
            MCCI_MT000100UV01Device senderDevice = new MCCI_MT000100UV01Device();
            senderDevice.classCode = EntityClassDevice.DEV;
            senderDevice.determinerCode = EntityDeterminerSpecific.INSTANCE;
            II id = new II();
            id.root = "1.2.250.1.999.1.1.356";
            senderDevice.id = new[] { id };
            SC sc = new SC();
            sc.Text = new string[] { "Nom du LPS" };
            senderDevice.softwareName = sc;
            sender.device = senderDevice;
            H17V3Message.sender = sender;
            // /////////////////////////////////////
            // ajout du receiver
            // /////////////////////////////////////
            MCCI_MT000100UV01Receiver receiver = new MCCI_MT000100UV01Receiver();
            receiver.typeCode = CommunicationFunctionType.RCV;
            MCCI_MT000100UV01Device receiverDevice = new MCCI_MT000100UV01Device();
            receiverDevice.classCode = EntityClassDevice.DEV;
            receiverDevice.determinerCode = EntityDeterminerSpecific.INSTANCE;
            II idReceiver = new II();
            idReceiver.root = "1.2.250.1.213.4.1.1.1";
            receiverDevice.id = new[] { idReceiver };
            SC scReceiver = new SC();
            scReceiver.Text = new string[] { "DMP" };
            receiverDevice.softwareName = scReceiver;
            receiver.device = receiverDevice;
            H17V3Message.receiver = new[] { receiver };

            PRPA_IN201307UV02QUQI_MT021001UV01ControlActProcess controlActProcess = new PRPA_IN201307UV02QUQI_MT021001UV01ControlActProcess();

            controlActProcess.classCode = ActClassControlAct.CACT;
            controlActProcess.moodCode = x_ActMoodIntentEvent.EVN;
            //  /////////////////////////////////////
            //  Sp�cification de la requ�te effectu�e
            //  code = {TEST_EXST,CNSLT_DATA}
            //  /////////////////////////////////////
            controlActProcess.reasonCode = new[] { createReasonCode("CNSLT_DATA") };
            PRPA_MT201307UV02QueryByParameter queryByParameter = new PRPA_MT201307UV02QueryByParameter();
            II queryId = new II();
            queryId.extension = UUID.GenerateRandomUuid();
            queryId.root = "1.2.250.1.999.1.1.356.4";
            queryByParameter.queryId = queryId;
            CS statusCode = new CS();
            statusCode.code = "new";
            queryByParameter.statusCode = statusCode;
            PRPA_MT201307UV02ParameterList parameterList = new PRPA_MT201307UV02ParameterList();
            PRPA_MT201307UV02PatientIdentifier patientId = new PRPA_MT201307UV02PatientIdentifier();
            // /////////////////////////////////////
            // SpÃ©cification de l'INS requÃªtÃ©
            // /////////////////////////////////////
            II value = new II();
            value.root = "1.2.250.1.213.1.4.10";
            // root INS-NIR
            value.extension = Dmp.PatientIns;
            patientId.value = new[] { value };
            ST semanticsText = new ST();
            semanticsText.Text = new string[] { "Patient.id" };
            patientId.semanticsText = semanticsText;
            parameterList.patientIdentifier = new[] { patientId };
            queryByParameter.parameterList = parameterList;

            controlActProcess.queryByParameter = queryByParameter;
            H17V3Message.controlActProcess = controlActProcess;

            return H17V3Message;
        }
        public static PRPA_IN201307UV02 IsDMPExistHl7V3Message(string patientIns)
        {
            PRPA_IN201307UV02 H17V3Message = new PRPA_IN201307UV02();
            //  /////////////////////////////////////
            //  creation de l'enveloppe du message HL7
            //  /////////////////////////////////////
            H17V3Message.ITSVersion = "XML_1.0";
            II messageId = new II();
            messageId.root = "1.2.250.1.999.1.1.356.3";
            messageId.extension = UUID.GenerateRandomUuid();
            H17V3Message.id = messageId;

            TS creationTime = new TS();
            creationTime.value = "20100528";
            H17V3Message.creationTime = creationTime;
            II interactionId = new II();
            interactionId.root = "2.16.840.1.113883.1.6";
            interactionId.extension = "PRPA_IN201307UV02";

            H17V3Message.interactionId = interactionId;
            CS cs = new CS();
            cs.code = "P";
            H17V3Message.processingCode = cs;
            CS cs1 = new CS();
            cs1.code = "T";
            H17V3Message.processingModeCode = cs1;
            CS cs2 = new CS();
            cs2.code = "AL";
            H17V3Message.acceptAckCode = cs2;

            MCCI_MT000100UV01Sender sender = new MCCI_MT000100UV01Sender();
            sender.typeCode = CommunicationFunctionType.SND;
            MCCI_MT000100UV01Device senderDevice = new MCCI_MT000100UV01Device();
            senderDevice.classCode = EntityClassDevice.DEV;
            senderDevice.determinerCode = EntityDeterminerSpecific.INSTANCE;
            II id = new II();
            id.root = "1.2.250.1.999.1.1.356";
            senderDevice.id = new[] { id };
            SC sc = new SC();
            sc.Text = new string[] { "Nom du LPS" };
            senderDevice.softwareName = sc;
            sender.device = senderDevice;
            H17V3Message.sender = sender;

            MCCI_MT000100UV01Receiver receiver = new MCCI_MT000100UV01Receiver();
            receiver.typeCode = CommunicationFunctionType.RCV;
            MCCI_MT000100UV01Device receiverDevice = new MCCI_MT000100UV01Device();
            receiverDevice.classCode = EntityClassDevice.DEV;
            receiverDevice.determinerCode = EntityDeterminerSpecific.INSTANCE;
            II idReceiver = new II();
            idReceiver.root = "1.2.250.1.213.4.1.1.1";
            receiverDevice.id = new[] { idReceiver };
            SC scReceiver = new SC();
            scReceiver.Text = new string[] { "DMP" };
            receiverDevice.softwareName = scReceiver;
            receiver.device = receiverDevice;
            H17V3Message.receiver = new[] { receiver };

            PRPA_IN201307UV02QUQI_MT021001UV01ControlActProcess controlActProcess = new PRPA_IN201307UV02QUQI_MT021001UV01ControlActProcess();

            controlActProcess.classCode = ActClassControlAct.CACT;
            controlActProcess.moodCode = x_ActMoodIntentEvent.EVN;
            //  /////////////////////////////////////
            //  Sp�cification de la requ�te effectu�e
            //  code = {TEST_EXST,CNSLT_DATA}
            //  /////////////////////////////////////
            controlActProcess.reasonCode = new[] { createReasonCode("TEST_EXST") };
            PRPA_MT201307UV02QueryByParameter queryByParameter = new PRPA_MT201307UV02QueryByParameter();
            II queryId = new II();
            queryId.extension = UUID.GenerateRandomUuid();
            queryId.root = "1.2.250.1.999.1.1.356.4";
            queryByParameter.queryId = queryId;
            CS statusCode = new CS();
            statusCode.code = "new";
            queryByParameter.statusCode = statusCode;
            PRPA_MT201307UV02ParameterList parameterList = new PRPA_MT201307UV02ParameterList();
            PRPA_MT201307UV02PatientIdentifier patientId = new PRPA_MT201307UV02PatientIdentifier();
            II value = new II();
            value.root = "1.2.250.1.213.1.4.10";
            // root INS-NIR
            value.extension = Dmp.PatientIns;
            patientId.value = new[] { value };
            ST semanticsText = new ST();
            semanticsText.Text = new string[] { "Patient.id" };
            patientId.semanticsText = semanticsText;
            parameterList.patientIdentifier = new[] { patientId };
            queryByParameter.parameterList = parameterList;

            controlActProcess.queryByParameter = queryByParameter;
            H17V3Message.controlActProcess = controlActProcess;
            return H17V3Message;
        }
        private static CE createReasonCode(String code)
        {
            CE reasonCode = new CE();
            if (code.Contains("TEST_EXST"))
            {
                reasonCode.code = "TEST_EXST";
                reasonCode.displayName = "Test d'existence de dossier";
            }
            else if (code.Contains("CNSLT_DATA"))
            {
                reasonCode.code = "CNSLT_DATA";
                reasonCode
                    .displayName = "Consultation des données de gestion de dossier";
            }
            reasonCode.codeSystem = "1.2.250.1.213.1.1.4.11";
            return reasonCode;
        }
        public static SubmitObjectsRequest MaskMessage()
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
                                Value=new string[] { "HOPITAL DES 3 VALLEES00771^^^^^&1.2.250.1.71.4.2.2&ISO^IDNST^^^10B0007712" }
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
            ass.targetObject = "Document01";
            ass.sourceObject = "SubmissionSet01";
            ass.associationType = "urn:oasis:names:tc:ebxml-regrep:AssociationType:HasMember";
            ass.objectType = "urn:oasis:names:tc:ebxml-regrep:ObjectType:RegistryObject:Association";
            ass.id = "association1";
            //-------------------
            SlotType1 slot1 = new SlotType1();
            slot1.name = "SubmissionSetStatus";
            ValueListType vt = new ValueListType();
            vt.Value = new string[] { "Original" };
            slot1.ValueList = vt;
            SlotType1 slot2 = new SlotType1();
            slot2.name = "PreviousVersion";
            ValueListType vt1 = new ValueListType();
            vt1.Value = new string[] { "5" };
            slot2.ValueList = vt1;
            ass.Slot = new SlotType1[]
            {
                slot1,slot2
            };

            //--------------
            ExtrinsicObjectType eot = new ExtrinsicObjectType()
            {
                id = "Document01",
                isOpaque = false,
                lid = "urn:uuid:4f196ae8-5549-11e9-9478-0050569e422e",
                mimeType = "text/xml",
                objectType = "urn:uuid:7edca82f-054d-47f2-a032-9b2a5b5186c1",
                status = "urn:oasis:names:tc:ebxml-regrep:StatusType:Approved"
            };
            eot.Slot = new SlotType1[]
            {
                new SlotType1()
                {
                    name="creationTime",
                    ValueList = new ValueListType()
                            {
                                Value=new string[] { "20190402151411" }
                            }
                },
                 new SlotType1()
                {
                    name="hash",
                    ValueList = new ValueListType()
                            {
                                Value=new string[] { "2c0b4b3964c9056675d427b4204f40ca4132a9c1" }
                            }
                },
                  new SlotType1()
                {
                    name="languageCode",
                    ValueList = new ValueListType()
                            {
                                Value=new string[] { "fr-FR" }
                            }
                },
                   new SlotType1()
                {
                    name="legalAuthenticator",
                    ValueList = new ValueListType()
                            {
                                Value=new string[] { "899900063480^MAX^LIBRE^^^^^^&1.2.250.1.71.4.2.1&ISO^D^^^IDNPS" }
                            }
                },
                    new SlotType1()
                {
                    name="repositoryUniqueId",
                    ValueList = new ValueListType()
                            {
                                Value=new string[] { "1.2.250.1.213.4.1.1.1.2" }
                            }
                },
                     new SlotType1()
                {
                    name="serviceStartTime",
                    ValueList = new ValueListType()
                            {
                                Value=new string[] { "20190402151411" }
                            }
                },
                      new SlotType1()
                {
                    name="serviceStopTime",
                    ValueList = new ValueListType()
                            {
                                Value=new string[] { "20190402151411" }
                            }
                },
                       new SlotType1()
                {
                    name="size",
                    ValueList = new ValueListType()
                            {
                                Value=new string[] { "4267" }
                            }
                },
                        new SlotType1()
                {
                    name="sourcePatientId",
                    ValueList = new ValueListType()
                            {
                                Value=new string[] { "0456789999^^^&1.2.250.1.999.1.1.7898.2&ISO^PI" }
                            }
                },
                         new SlotType1()
                {
                    name="sourcePatientInfo",
                    ValueList = new ValueListType()
                            {
                                Value=new string[] { "PID-7|19790328", "PID-8|F", "PID-5|PAT-TROIS^DOMINIQUE^^^^^L" }
                            }
                }

            };

            eot.Name = new InternationalStringType()
            {
                LocalizedString = new LocalizedStringType[] {
                            new LocalizedStringType()
                            {
                                value="Document 1",
                                charset="UTF8",
                                lang="FR"
                            }
                        }
            };
            eot.Description = new InternationalStringType()
            {
                LocalizedString = new LocalizedStringType[] {
                            new LocalizedStringType()
                            {
                                value="commentaire sur document",
                                charset="UTF8",
                                lang="FR"
                            }
                        }
            };

            eot.VersionInfo = new VersionInfoType() { versionName = "1" };

            eot.Classification = new ClassificationType[] {
                new ClassificationType()
                {
                    nodeRepresentation="",
                    classifiedObject="urn:uuid:4f196ae8-5549-11e9-9478-0050569e422e",
                    classificationScheme="urn:uuid:93606bcf-9494-43ec-9b4e-a7748d1a838d",
                    id="urn:uuid:c6994492-bab9-4683-a2ef-bcd8e6399801",
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
                                Value=new string[] { "HOPITAL DES 3 VALLEES00771^^^^^&1.2.250.1.71.4.2.2&ISO^IDNST^^^10B0007712" }
                            }
                        }
                    }
                },
                new ClassificationType()
                {
                    nodeRepresentation="10",
                    classifiedObject="urn:uuid:4f196ae8-5549-11e9-9478-0050569e422e",
                    classificationScheme="urn:uuid:41a5887f-8865-4c09-adf7-e362475b143a",
                    id="urn:uuid:70147131-00c7-4292-8635-24cecd547036",
                    Slot = new SlotType1[]
                    {
                        new SlotType1()
                        {
                            name="codingScheme",
                            ValueList = new ValueListType()
                            {
                                Value=new string[] { "1.2.250.1.213.1.1.4.1" }
                            }
                        }
                    },
                    Name = new InternationalStringType()
                    {
                        LocalizedString = new LocalizedStringType[] {
                            new LocalizedStringType()
                            {
                                value="Compte rendu",
                                charset="UTF8",
                                lang="FR"
                            }
                        }
                    }
                },
                   new ClassificationType()
                {
                    nodeRepresentation="N",
                    classifiedObject="urn:uuid:4f196ae8-5549-11e9-9478-0050569e422e",
                    classificationScheme="urn:uuid:f4f85eac-e6cb-4883-b524-f2705394840f",
                    id="urn:uuid:ac6ce528-4eac-48a5-8ff0-f14df9177c3e",
                    Slot = new SlotType1[]
                    {
                        new SlotType1()
                        {
                            name="codingScheme",
                            ValueList = new ValueListType()
                            {
                                Value=new string[] { "2.16.840.1.113883.5.25" }
                            }
                        }
                    },
                    Name = new InternationalStringType()
                    {
                        LocalizedString = new LocalizedStringType[] {
                            new LocalizedStringType()
                            {
                                value="Normal",
                                charset="UTF8",
                                lang="FR"
                            }
                        }
                    }
                },
                    new ClassificationType()
                {
                    nodeRepresentation="MASQUE_PS",
                    classifiedObject="urn:uuid:4f196ae8-5549-11e9-9478-0050569e422e",
                    classificationScheme="urn:uuid:f4f85eac-e6cb-4883-b524-f2705394840f",
                    id="urn:uuid:ac6ce528-4eac-48a5-8ff0-f14df9177c3e",
                    Slot = new SlotType1[]
                    {
                        new SlotType1()
                        {
                            name="codingScheme",
                            ValueList = new ValueListType()
                            {
                                Value=new string[] { "1.2.250.1.213.1.1.4.13" }
                            }
                        }
                    },
                    Name = new InternationalStringType()
                    {
                        LocalizedString = new LocalizedStringType[] {
                            new LocalizedStringType()
                            {
                                value="Document masqué aux professionnels de santé",
                                charset="UTF8",
                                lang="FR"
                            }
                        }
                    }
                },
                     new ClassificationType()
                {
                    nodeRepresentation="18724-5",
                    classifiedObject="urn:uuid:4f196ae8-5549-11e9-9478-0050569e422e",
                    classificationScheme="urn:uuid:2c6b8cb7-8b2a-4051-b291-b1ae6a575ef4",
                    id="urn:uuid:a720ae4d-1997-4f4b-80e6-34f34d3eede6",
                    Slot = new SlotType1[]
                    {
                        new SlotType1()
                        {
                            name="codingScheme",
                            ValueList = new ValueListType()
                            {
                                Value=new string[] { "2.16.840.1.113883.6.1" }
                            }
                        }
                    },
                    Name = new InternationalStringType()
                    {
                        LocalizedString = new LocalizedStringType[] {
                            new LocalizedStringType()
                            {
                                value="HLA",
                                charset="UTF8",
                                lang="FR"
                            }
                        }
                    }
                },
                      new ClassificationType()
                {
                    nodeRepresentation="urn:ihe:iti:xds-sd:text:2008",
                    classifiedObject="urn:uuid:4f196ae8-5549-11e9-9478-0050569e422e",
                    classificationScheme="urn:uuid:a09d5840-386c-46f2-b5ad-9c3699a4309d",
                    id="urn:uuid:061c556f-583c-4359-930d-bb6a704a5415",
                    Slot = new SlotType1[]
                    {
                        new SlotType1()
                        {
                            name="codingScheme",
                            ValueList = new ValueListType()
                            {
                                Value=new string[] { "1.3.6.1.4.1.19376.1.2.3" }
                            }
                        }
                    },
                    Name = new InternationalStringType()
                    {
                        LocalizedString = new LocalizedStringType[] {
                            new LocalizedStringType()
                            {
                                value="Document à corps non structuré en texte brut",
                                charset="UTF8",
                                lang="FR"
                            }
                        }
                    }
                },
                       new ClassificationType()
                {
                    nodeRepresentation="SA01",
                    classifiedObject="urn:uuid:4f196ae8-5549-11e9-9478-0050569e422e",
                    classificationScheme="urn:uuid:f33fb8ac-18af-42cc-ae0e-ed0b0bdb91e1",
                    id="urn:uuid:00b837a1-fc69-4913-8b25-e739bf7f1ec7",
                    Slot = new SlotType1[]
                    {
                        new SlotType1()
                        {
                            name="codingScheme",
                            ValueList = new ValueListType()
                            {
                                Value=new string[] { "1.2.250.1.71.4.2.4" }
                            }
                        }
                    },
                    Name = new InternationalStringType()
                    {
                        LocalizedString = new LocalizedStringType[] {
                            new LocalizedStringType()
                            {
                                value="Etablissement public de santé",
                                charset="UTF8",
                                lang="FR"
                            }
                        }
                    }
                },
                        new ClassificationType()
                {
                    nodeRepresentation="ETABLISSEMENT",
                    classifiedObject="urn:uuid:4f196ae8-5549-11e9-9478-0050569e422e",
                    classificationScheme="urn:uuid:cccf5598-8b07-4b77-a05e-ae952c785ead",
                    id="urn:uuid:d56bcaf3-4208-41e0-8d2a-6b4b581effc5",
                    Slot = new SlotType1[]
                    {
                        new SlotType1()
                        {
                            name="codingScheme",
                            ValueList = new ValueListType()
                            {
                                Value=new string[] { "21.2.250.1.213.1.1.4.9" }
                            }
                        }
                    },
                    Name = new InternationalStringType()
                    {
                        LocalizedString = new LocalizedStringType[] {
                            new LocalizedStringType()
                            {
                                value="Etablissement de santé",
                                charset="UTF8",
                                lang="FR"
                            }
                        }
                    }
                },
                         new ClassificationType()
                {
                    nodeRepresentation="34874-8",
                    classifiedObject="urn:uuid:4f196ae8-5549-11e9-9478-0050569e422e",
                    classificationScheme="urn:uuid:f0306f51-975f-434e-a61c-c59651d33983",
                    id="urn:uuid:f67ccb90-2e11-4857-bbf6-9db0c0a97572",
                    Slot = new SlotType1[]
                    {
                        new SlotType1()
                        {
                            name="codingScheme",
                            ValueList = new ValueListType()
                            {
                                Value=new string[] { "2.16.840.1.113883.6.1 "}
                            }
                        }
                    },
                    Name = new InternationalStringType()
                    {
                        LocalizedString = new LocalizedStringType[] {
                            new LocalizedStringType()
                            {
                                value="CR opératoire",
                                charset="UTF8",
                                lang="FR"
                            }
                        }
                    }
                },
            };

            eot.ExternalIdentifier = new ExternalIdentifierType[] {

                new ExternalIdentifierType()
                {
                    value=Dmp.patientNirOD+"^^20100907",
                    identificationScheme="urn:uuid:58a6f841-87b3-4a3e-92fd-a8ffeff98427",
                    id="urn:uuid:a7dba492-0d12-45ed-a9aa-6d95fa8ca42e",
                    registryObject="urn:uuid:4f196ae8-5549-11e9-9478-0050569e422e",
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
                    value="1.2.250.1.999.1.1.7898.4.9956570602",
                     registryObject="urn:uuid:4f196ae8-5549-11e9-9478-0050569e422e",
                    identificationScheme="urn:uuid:2e82c1f6-a085-4c72-9da3-8640a32e42ab",
                    id="urn:uuid:12c95b1b-8263-46da-931a-caf3b5f1923c",
                    Name = new InternationalStringType()
                    {
                        LocalizedString = new LocalizedStringType[] {
                            new LocalizedStringType()
                            {
                                value="XDSDocumentEntry.uniqueId",
                                charset="UTF8",
                                lang="FR"
                            }
                        }
                    }
                }

            };

            sor.RegistryObjectList = new RegistryObjectType[]
            {
                rpt
            };
            return sor;
        }
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
                                Value=new string[] { "HOPITAL DES 3 VALLEES00771^^^^^&1.2.250.1.71.4.2.2&ISO^IDNST^^^10B0007712" }
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
            ass.targetObject = "urn:uuid:41b6cd53-a85f-464c-aebe-ed0bf5c2c85a";
            ass.sourceObject = "SubmissionSet01";
            ass.associationType = "urn:ihe:iti:2010:AssociationType:UpdateAvailabilityStatus";
            ass.objectType = "urn:oasis:names:tc:ebxml-regrep:ObjectType:RegistryObject:Association";
            ass.id = "association1";
            //-------------------
            SlotType1 slot1 = new SlotType1();
            slot1.name = "OriginalStatus";
            ValueListType vt = new ValueListType();
            vt.Value = new string[] { "urn:oasis:names:tc:ebxml-regrep:StatusType:Approved" };
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
                rpt
            };
            XmlSerializer mySerializer = new XmlSerializer(typeof(DocumentRegistryService.SubmitObjectsRequest));
            StreamWriter myWriter = new StreamWriter("myFileName.xml");
            mySerializer.Serialize(myWriter, sor);
                        myWriter.Close();
           

            return sor;
        }
        public static SubmitObjectsRequest ArchiveMessage()
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
                    id="cla62",
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
                                Value=new string[] { "HOPITAL DES 3 VALLEES00771^^^^^&1.2.250.1.71.4.2.2&ISO^IDNST^^^10B0007712" }
                            }
                        }
                    }
                },
                new ClassificationType()
                {
                    nodeRepresentation="04",
                    classifiedObject="SubmissionSet01",
                    classificationScheme="urn:uuid:aa543740-bdda-424e-8c96-df4873be8500",
                    id="cla63",
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
                    id="cla64",
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
            ass.targetObject = "urn:uuid:41b6cd53-a85f-464c-aebe-ed0bf5c2c85a";
            ass.sourceObject = "SubmissionSet01";
            ass.associationType = "urn:ihe:iti:2010:AssociationType:UpdateAvailabilityStatus";
            ass.objectType = "urn:oasis:names:tc:ebxml-regrep:ObjectType:RegistryObject:Association";
            ass.id = "association1";
            //-------------------
            SlotType1 slot1 = new SlotType1();
            slot1.name = "OriginalStatus";
            ValueListType vt = new ValueListType();
            vt.Value = new string[] { "urn:asip:ci-sis:2010:StatusType:Deleted" };
            slot1.ValueList = vt;
            SlotType1 slot2 = new SlotType1();
            slot2.name = "NewStatus";
            ValueListType vt1 = new ValueListType();
            vt1.Value = new string[] { "urn:asip:ci-sis:2010:StatusType:Archived" };
            slot2.ValueList = vt1;
            ass.Slot = new SlotType1[]
            {
                slot1,slot2
            };
            sor.RegistryObjectList = new RegistryObjectType[]
            {
                rpt
            };
            return sor;
        }
    }
}
