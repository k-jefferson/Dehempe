using System.Globalization;
using System.Xml;
using Dehempe.Application.Common.Interfaces;
using Dehempe.Domain.Entities;
using Dehempe.Domain.Exceptions;
using Dehempe.Domain.Interfaces;
using Dehempe.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dehempe.Infrastructure.Dmp.Soap;

/// <summary>
/// Client SOAP de la TD 0.2 (Test d'existence du DMP).
/// Construit un message HL7 V3 <c>PRPA_IN201307UV02</c> et parse la réponse
/// <c>PRPA_IN201308UV02</c>.
/// </summary>
internal sealed class GdpExistenceClient : XdsSoapClientBase, IDmpExistenceRepository
{
    private readonly DmpOptions _options;
    private readonly IVihfContextAccessor _vihfCtxAccessor;

    public GdpExistenceClient(
        HttpClient http,
        IVihfService vihf,
        ICpsAuthService cpsAuth,
        IOptions<DmpOptions> options,
        IVihfContextAccessor vihfCtxAccessor,
        ILogger<GdpExistenceClient> logger,
        ISoapRequestCapture capture)
        : base(http, vihf, cpsAuth, logger, capture)
    {
        _options         = options.Value;
        _vihfCtxAccessor = vihfCtxAccessor;
    }

    public async Task<DmpExistenceResult> CheckExistenceAsync(Ins patientIns, CancellationToken ct = default)
    {
        var doc     = new XmlDocument();
        var body    = BuildRequestBody(doc, patientIns);
        var vihfCtx = _vihfCtxAccessor.GetContext(patientIns);

        var response = await SendSoapAsync(
            _options.GdpEndpoint,
            XdsConstants.Iti0_2Action,
            body,
            vihfCtx,
            ct);

        return ParseResponse(response, patientIns);
    }

    // ── Request (PRPA_IN201307UV02) ────────────────────────────────────────────

    private XmlElement BuildRequestBody(XmlDocument doc, Ins patientIns)
    {
        var hl7 = XdsConstants.Hl7V3Ns;

        XmlElement El(string name) => doc.CreateElement(name, hl7);
        XmlElement ElAttr(string name, params (string attr, string value)[] attrs)
        {
            var e = doc.CreateElement(name, hl7);
            foreach (var (a, v) in attrs) e.SetAttribute(a, v);
            return e;
        }

        var root = ElAttr("PRPA_IN201307UV02", ("ITSVersion", "XML_1.0"));

        root.AppendChild(ElAttr("id",
            ("root", _options.LpsDeviceOid + ".1.1"),
            ("extension", Guid.NewGuid().ToString())));
        root.AppendChild(ElAttr("creationTime",
            ("value", DateTime.UtcNow.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture))));
        root.AppendChild(ElAttr("interactionId",
            ("root", XdsConstants.Hl7V3InteractionRoot),
            ("extension", "PRPA_IN201307UV02")));
        root.AppendChild(ElAttr("processingCode",     ("code", "D")));
        root.AppendChild(ElAttr("processingModeCode", ("code", "T")));
        root.AppendChild(ElAttr("acceptAckCode",      ("code", "AL")));

        // receiver (DMP)
        var receiver = ElAttr("receiver", ("typeCode", "RCV"));
        var receiverDevice = ElAttr("device", ("classCode", "DEV"), ("determinerCode", "INSTANCE"));
        receiverDevice.AppendChild(ElAttr("id", ("root", XdsConstants.GdpReceiverDeviceOid)));
        var receiverName = El("softwareName"); receiverName.InnerText = "DMP";
        receiverDevice.AppendChild(receiverName);
        receiver.AppendChild(receiverDevice);
        root.AppendChild(receiver);

        // sender (notre LPS)
        var sender = ElAttr("sender", ("typeCode", "SND"));
        var senderDevice = ElAttr("device", ("classCode", "DEV"), ("determinerCode", "INSTANCE"));
        senderDevice.AppendChild(ElAttr("id", ("root", _options.LpsDeviceOid)));
        var senderName = El("softwareName"); senderName.InnerText = _options.LpsSoftwareName;
        senderDevice.AppendChild(senderName);
        sender.AppendChild(senderDevice);
        root.AppendChild(sender);

        // controlActProcess avec reasonCode TEST_EXST
        var control = ElAttr("controlActProcess", ("classCode", "CACT"), ("moodCode", "EVN"));
        control.AppendChild(ElAttr("reasonCode",
            ("code",        XdsConstants.ReasonCodeTestExst),
            ("codeSystem",  XdsConstants.ReasonCodeSystemTestExst),
            ("displayName", "Test d'existence de dossier")));

        var queryByParam = El("queryByParameter");
        queryByParam.AppendChild(ElAttr("queryId",
            ("root",      _options.LpsDeviceOid + ".2"),
            ("extension", Guid.NewGuid().ToString())));
        queryByParam.AppendChild(ElAttr("statusCode", ("code", "new")));

        var paramList = El("parameterList");
        var patientIdentifier = El("patientIdentifier");
        patientIdentifier.AppendChild(ElAttr("value",
            ("root", XdsConstants.GdpInsRoot),
            ("extension", patientIns.Value)));
        var sem = El("semanticsText"); sem.InnerText = "Patient.id";
        patientIdentifier.AppendChild(sem);
        paramList.AppendChild(patientIdentifier);
        queryByParam.AppendChild(paramList);

        control.AppendChild(queryByParam);
        root.AppendChild(control);

        return root;
    }

    // ── Response (PRPA_IN201308UV02) ──────────────────────────────────────────

    private DmpExistenceResult ParseResponse(XmlDocument response, Ins patientIns)
    {
        var ns = new XmlNamespaceManager(response.NameTable);
        ns.AddNamespace("s",   XdsConstants.SoapNs);
        ns.AddNamespace("hl7", XdsConstants.Hl7V3Ns);

        var ack       = response.SelectSingleNode("//hl7:PRPA_IN201308UV02/hl7:acknowledgement", ns);
        var ackType   = ack?.Attributes?["typeCode"]?.Value ?? string.Empty;
        var queryAck  = response.SelectSingleNode("//hl7:queryAck/hl7:queryResponseCode/@code", ns)?.Value
                     ?? string.Empty;

        var exists    = string.Equals(queryAck, XdsConstants.QueryResponseOk, StringComparison.OrdinalIgnoreCase);

        bool? authorisationValid = ReadAttentionFlag(response, ns,
            XdsConstants.AttentionKeyAutorisation,
            v => string.Equals(v, XdsConstants.AttentionValueValide, StringComparison.OrdinalIgnoreCase));

        bool? statutMt = ReadAttentionBool(response, ns, XdsConstants.AttentionKeyStatutMt);

        string? errorMessage = null;
        if (!string.Equals(ackType, "AA", StringComparison.OrdinalIgnoreCase))
            errorMessage = response.SelectSingleNode(
                "//hl7:acknowledgement/hl7:acknowledgementDetail/hl7:text", ns)?.InnerText
                ?? $"Ack typeCode={ackType}";

        var patient = exists ? ParsePatient(response, ns) : null;

        Logger.LogInformation(
            "TD 0.2 — INS {Ins} : exists={Exists}, queryResponseCode={Code}, ack={Ack}",
            patientIns.Value, exists, queryAck, ackType);

        return new DmpExistenceResult(
            PatientIns:                    patientIns,
            Exists:                        exists,
            QueryResponseCode:             queryAck,
            AckTypeCode:                   ackType,
            IsAuthorizationValid:          authorisationValid,
            IsAttachedToTreatingPhysician: statutMt,
            Patient:                       patient,
            ErrorMessage:                  errorMessage);
    }

    private static bool? ReadAttentionFlag(
        XmlDocument doc, XmlNamespaceManager ns, string keyword, Func<string, bool> predicate)
    {
        var node = doc.SelectSingleNode(
            $"//hl7:attentionLine[hl7:keyWordText/@code='{keyword}']/hl7:value", ns);
        if (node is null) return null;
        var code = node.Attributes?["code"]?.Value ?? node.Attributes?["value"]?.Value;
        return code is null ? null : predicate(code);
    }

    private static bool? ReadAttentionBool(XmlDocument doc, XmlNamespaceManager ns, string keyword)
    {
        var value = doc.SelectSingleNode(
            $"//hl7:attentionLine[hl7:keyWordText/@code='{keyword}']/hl7:value/@value", ns)?.Value;
        return bool.TryParse(value, out var b) ? b : null;
    }

    private static DmpPatientInfo? ParsePatient(XmlDocument doc, XmlNamespaceManager ns)
    {
        var patient = doc.SelectSingleNode("//hl7:subject1/hl7:patient", ns);
        if (patient is null) return null;

        string? IdByRoot(string root) => patient
            .SelectSingleNode($"hl7:id[@root='{root}']/@extension", ns)?.Value;

        string? Text(string xpath) => patient.SelectSingleNode(xpath, ns)?.InnerText;
        string? Attr(string xpath) => patient.SelectSingleNode(xpath, ns)?.Value;

        var person = patient.SelectSingleNode("hl7:patientPerson", ns);
        var name   = person?.SelectSingleNode("hl7:name", ns);

        bool? hasInternet = ReadSubjectOfBool(patient, ns, XdsConstants.ObsCodeCompteInternet);
        bool? rattachEns  = ReadSubjectOfBool(patient, ns, XdsConstants.ObsCodeRattachementEns);

        DateOnly? birth = null;
        var birthValue  = patient.SelectSingleNode("hl7:patientPerson/hl7:birthTime/@value", ns)?.Value;
        if (!string.IsNullOrWhiteSpace(birthValue) &&
            DateOnly.TryParseExact(birthValue, "yyyyMMdd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var b)) birth = b;

        return new DmpPatientInfo(
            InsC:               IdByRoot(XdsConstants.InsCOid),
            InsNir:             IdByRoot(XdsConstants.GdpInsRoot),
            Status:             Attr("hl7:statusCode/@code"),
            Prefix:             name?.SelectSingleNode("hl7:prefix", ns)?.InnerText,
            GivenName:          name?.SelectSingleNode("hl7:given",  ns)?.InnerText,
            FamilyName:         name?.SelectSingleNode("hl7:family", ns)?.InnerText,
            Email:              ExtractTelecom(person, ns, "mailto:"),
            Phone:              ExtractTelecom(person, ns, "tel:"),
            GenderCode:         Attr("hl7:patientPerson/hl7:administrativeGenderCode/@code"),
            BirthDate:          birth,
            HasInternetAccount: hasInternet,
            IsAttachedToEns:    rattachEns);
    }

    private static string? ExtractTelecom(XmlNode? person, XmlNamespaceManager ns, string scheme)
    {
        if (person is null) return null;
        foreach (XmlNode tel in person.SelectNodes("hl7:telecom", ns)!)
        {
            var v = tel.Attributes?["value"]?.Value;
            if (v is not null && v.StartsWith(scheme, StringComparison.OrdinalIgnoreCase))
                return v[scheme.Length..];
        }
        return null;
    }

    private static bool? ReadSubjectOfBool(XmlNode patient, XmlNamespaceManager ns, string code)
    {
        var value = patient.SelectSingleNode(
            $"hl7:subjectOf/hl7:administrativeObservation[hl7:code/@code='{code}']/hl7:value/@value", ns)?.Value;
        return bool.TryParse(value, out var b) ? b : null;
    }
}
