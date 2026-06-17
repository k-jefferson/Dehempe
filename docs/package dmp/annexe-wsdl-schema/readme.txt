=====================================

     Dossier Médical Personnel 

	schémas et WSDL

===================================
Version 2.01.04
Date de mise à jour le 11/06/2024

Mise à jour du WSDL PatientsSpecific.wsdl 

Ce WSDL a pour vocation de conserver uniquement la description technique de l’appel au service « PatientList » et « PatientListReponse » utilisés pour la TD0.4 : Lister les DMP autorisés
Dans cette version sont supprimés les descriptions techniques suivantes :
•	"createPatientAccess"/ "createPatientAccessResponse" : TD1.5a : Création du compte internet patient
•	"patientOTPUpdate"/ "patientOTPUpdateResponse" : TD1.5b : Ajout d'un canal OTP
•	"updatePatientAccess"/ "updatePatientAccessResponse" : TD1.5d : Maj des informations du compte internet


====================================


	VERSION 1.0.1

  Date de mise à jour : 03/11/2010

=====================================

Cette annexe regroupe les schémas et WSDL des WebService du DMP

Description des dossiers du package :

wsdl			: les wsdl et les opérations liées sont décrits dans le document de spécification des interfaces LPS
schema			: schémas XSD associés
	ebRS		: schémas ebXML
	HL7V3		: schémas HL7V3
		NE2008	: schémas HL7V3 pour PDQ HL7 V3 (extrait de HL7 Normative edition 2008)
		NE2009	: schémas HL7V3 pour les fonction de gestion administrative du dossier (extrait de HL7 Normative edition 2009)
	IHE		: schémas XDS.b


Historique des versions :
-------------------------

* 16/01/2019 : modification de schémas xsd (détail des modifications dans le document DMP_INF_005 du code exemple C#)

Pour exécuter correctement l’ajout des Références de Service sur l’ensemble des WSDL, les
fichiers suivants ont été modifiés pour que tous les types qui y sont déclarés appartiennent à
l’espace de nom HL7, dans les 2 répertoires schema\HL7V3\NE2008\coreschemas et
schema\HL7V3\NE2009\coreschemas :
 - Datatypes_base.xsd
 - Datatypes.xsd
 - InfrastructureRoot.xsd
 - NarrativeBlock.xsd
 - Voc.xsd
Pour chacun de ces fichiers, il est ajouté à la balise racine <xs :schema> les attributs
xmlns="urn:hl7-org:v3" targetNamespace="urn:hl7-org:v3" de manière à intégrer dans
cet espace de nom tous les types qui y sont définis


* 14/09/2010 : ajout du fichier manquant schema/ebRS/rim.xsd

* 03/11/2010 : 
Tous :			ajout de la version 1.0.0 en entête 

GestionDossierPatientPartage.wsdl : supression du choice.xsd (inclusion des éléments directement dans le wsdl)

Habilitation.wsdl : 	simplification des réponses
			mise à jour de l'opération listAuthorizationByPatientResponse (ajustements, changement des types date en string (format commun, en UTC))
			
Patientspecific.wsdl :	simplification des réponses
			ajout de l'opération patientList