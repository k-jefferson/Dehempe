namespace Dehempe.Infrastructure.Dmp.Soap;

internal static class XdsConstants
{
    // Namespaces SOAP / XDS
    public const string EbRim3Ns   = "urn:oasis:names:tc:ebxml-regrep:xsd:rim:3.0";
    public const string EbRs3Ns    = "urn:oasis:names:tc:ebxml-regrep:xsd:rs:3.0";
    public const string EbQ3Ns     = "urn:oasis:names:tc:ebxml-regrep:xsd:query:3.0";
    public const string IheXdsNs   = "urn:ihe:iti:xds-b:2007";
    public const string WsaNs      = "http://www.w3.org/2005/08/addressing";
    public const string WssNs      = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";
    public const string WsuNs      = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd";
    public const string SoapNs     = "http://www.w3.org/2003/05/soap-envelope";

    // ITI-18 — actions SOAP
    public const string Iti18Action  = "urn:ihe:iti:2007:RegistryStoredQuery";
    public const string Iti43Action  = "urn:ihe:iti:2007:RetrieveDocumentSet";
    public const string Iti41Action  = "urn:ihe:iti:2007:ProvideAndRegisterDocumentSet-b";

    // Stored Queries UUIDs (ITI-18)
    public const string FindDocumentsQuery   = "urn:uuid:14d4debf-8f97-4251-9a74-a90016b0af0d";
    public const string FindSubmissionSets   = "urn:uuid:f26abbcb-ac74-4422-8a30-edb644bbc1a9";
    public const string GetDocuments         = "urn:uuid:5c4f972b-d56b-40ac-a5fc-c8ca9b40b9d4";

    // Classification schemes (métadonnées XDS)
    public const string ClassCodeScheme              = "urn:uuid:41a5887f-8865-4c09-adf7-e362475b143a";
    public const string TypeCodeScheme               = "urn:uuid:f0306f51-975f-434e-a61c-c59651d33983";
    public const string FormatCodeScheme             = "urn:uuid:a09d5840-386c-46f2-b5ad-9fdf87b75cc4";
    public const string PracticeSettingCodeScheme    = "urn:uuid:cccf5598-8b07-4b77-a05e-ae952c785ead";
    public const string HealthcareFacilityTypeScheme = "urn:uuid:f33fb8ac-18af-42cc-ae0e-ed0b0bdb91e1";

    // Statuts document
    public const string StatusApproved   = "urn:oasis:names:tc:ebxml-regrep:StatusType:Approved";
    public const string StatusDeprecated = "urn:oasis:names:tc:ebxml-regrep:StatusType:Deprecated";

    // Slot names (ITI-18 query params)
    public const string SlotPatientId        = "$XDSDocumentEntryPatientId";
    public const string SlotStatus           = "$XDSDocumentEntryStatus";
    public const string SlotCreationTimeFrom = "$XDSDocumentEntryCreationTimeFrom";
    public const string SlotCreationTimeTo   = "$XDSDocumentEntryCreationTimeTo";
    public const string SlotClassCode        = "$XDSDocumentEntryClassCode";
    public const string SlotFormatCode       = "$XDSDocumentEntryFormatCode";

    // ── TD 0.2 (GestionDossierPatientPartage / GDP) — HL7 V3 PRPA_IN201307UV02 ──
    public const string Hl7V3Ns                = "urn:hl7-org:v3";
    public const string GdpServiceNs           = "asip:ci-sis:gdp:2010";
    public const string Iti0_2Action           = "urn:hl7-org:v3:PRPA_IN201307UV02";

    /// <summary>OID racine utilisée par le DMP pour identifier le patient dans les messages HL7 V3 GDP.</summary>
    public const string GdpInsRoot             = "1.2.250.1.213.1.4.10";

    /// <summary>OID du système destinataire DMP (champ <c>receiver/device/id</c>).</summary>
    public const string GdpReceiverDeviceOid   = "1.2.250.1.213.4.1.1.1";

    /// <summary>InteractionId HL7 V3 commun à PRPA_IN201307UV02 / PRPA_IN201308UV02.</summary>
    public const string Hl7V3InteractionRoot   = "2.16.840.1.113883.1.6";

    // reasonCode TD 0.2
    public const string ReasonCodeTestExst       = "TEST_EXST";
    public const string ReasonCodeSystemTestExst = "1.2.250.1.213.1.1.4.11";

    // Codes de réponse queryAck (TD 0.2)
    public const string QueryResponseOk       = "OK"; // DMP existe
    public const string QueryResponseNotFound = "NF"; // DMP n'existe pas

    // attentionLine — clés métier renvoyées par TD 0.2
    public const string AttentionKeyAutorisation = "AUTORISATION";
    public const string AttentionKeyStatutMt     = "STATUT_MT";
    public const string AttentionValueValide     = "VALIDE";

    // Codes spécifiques aux subjectOf de la réponse
    public const string ObsCodeCompteInternet    = "COMPTE_INTERNET_OUVERT";
    public const string ObsCodeRattachementEns   = "RATTACHEMENT_ENS";

    // OIDs INS utilisés en sortie HL7 V3 (côté DMP)
    public const string InsCOid                  = "1.2.250.1.213.1.4.2";
}
