using System.Xml;
using Dehempe.Application.Common.Interfaces;
using Dehempe.Domain.Entities;
using Dehempe.Domain.Interfaces;
using Dehempe.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dehempe.Infrastructure.Dmp.Soap;

/// <summary>
/// Client SOAP pour l'ITI-18 — Registry Stored Query.
/// Interroge le registre XDS.b du DMP pour récupérer les métadonnées de documents.
/// </summary>
internal sealed class XdsRegistryClient : XdsSoapClientBase
{
    private readonly DmpOptions _options;
    private readonly IVihfContextAccessor _vihfCtxAccessor;

    public XdsRegistryClient(
        HttpClient http,
        IVihfService vihf,
        ICpsAuthService cpsAuth,
        IOptions<DmpOptions> options,
        IVihfContextAccessor vihfCtxAccessor,
        ILogger<XdsRegistryClient> logger,
        ISoapRequestCapture capture)
        : base(http, vihf, cpsAuth, logger, capture)
    {
        _options = options.Value;
        _vihfCtxAccessor = vihfCtxAccessor;
    }

    public async Task<IReadOnlyList<DocumentEntry>> FindDocumentsAsync(
        Ins patientIns,
        DocumentSearchCriteria? criteria,
        CancellationToken ct)
    {
        var doc = new XmlDocument();
        var body = BuildFindDocumentsBody(doc, patientIns, criteria);
        var vihfCtx = _vihfCtxAccessor.GetContext(patientIns);

        var response = await SendSoapAsync(
            _options.RegistryEndpoint,
            XdsConstants.Iti18Action,
            body,
            vihfCtx,
            ct);

        return ParseDocumentEntries(response, patientIns);
    }

    private static XmlElement BuildFindDocumentsBody(
        XmlDocument doc,
        Ins patientIns,
        DocumentSearchCriteria? criteria)
    {
        var ns = new XmlNamespaceManager(doc.NameTable);
        ns.AddNamespace("rim", XdsConstants.EbRim3Ns);
        ns.AddNamespace("q",   XdsConstants.EbQ3Ns);

        var query = doc.CreateElement("q", "AdhocQueryRequest", XdsConstants.EbQ3Ns);
        query.SetAttribute("xmlns:rim", XdsConstants.EbRim3Ns);

        var responseOption = doc.CreateElement("q", "ResponseOption", XdsConstants.EbQ3Ns);
        responseOption.SetAttribute("returnComposedObjects", "true");
        responseOption.SetAttribute("returnType", "LeafClass");
        query.AppendChild(responseOption);

        var adhocQuery = doc.CreateElement("rim", "AdhocQuery", XdsConstants.EbRim3Ns);
        adhocQuery.SetAttribute("id", XdsConstants.FindDocumentsQuery);
        query.AppendChild(adhocQuery);

        AddSlot(doc, adhocQuery, XdsConstants.SlotPatientId,
            $"('{patientIns.Value}^^^&{patientIns.Oid.ToOidString()}&ISO')");

        var statusValues = new List<string>();
        if (criteria?.Status is DocumentStatus.Approved or null)
            statusValues.Add($"'{XdsConstants.StatusApproved}'");
        if (criteria?.Status is DocumentStatus.Deprecated)
            statusValues.Add($"'{XdsConstants.StatusDeprecated}'");
        if (statusValues.Count == 0)
            statusValues.Add($"'{XdsConstants.StatusApproved}'");
        AddSlot(doc, adhocQuery, XdsConstants.SlotStatus, $"({string.Join(",", statusValues)})");

        if (criteria?.CreatedAfter is not null)
            AddSlot(doc, adhocQuery, XdsConstants.SlotCreationTimeFrom,
                criteria.CreatedAfter.Value.ToString("yyyyMMddHHmmss"));

        if (criteria?.CreatedBefore is not null)
            AddSlot(doc, adhocQuery, XdsConstants.SlotCreationTimeTo,
                criteria.CreatedBefore.Value.ToString("yyyyMMddHHmmss"));

        if (criteria?.ClassCodes?.Count > 0)
            AddSlot(doc, adhocQuery, XdsConstants.SlotClassCode,
                $"({string.Join(",", criteria.ClassCodes.Select(c => $"'{c}'"))})");

        return query;
    }

    private static void AddSlot(XmlDocument doc, XmlElement parent, string name, string value)
    {
        var slot = doc.CreateElement("rim", "Slot", XdsConstants.EbRim3Ns);
        slot.SetAttribute("name", name);
        var valueList = doc.CreateElement("rim", "ValueList", XdsConstants.EbRim3Ns);
        var v = doc.CreateElement("rim", "Value", XdsConstants.EbRim3Ns);
        v.InnerText = value;
        valueList.AppendChild(v);
        slot.AppendChild(valueList);
        parent.AppendChild(slot);
    }

    private IReadOnlyList<DocumentEntry> ParseDocumentEntries(XmlDocument response, Ins patientIns)
    {
        var ns = new XmlNamespaceManager(response.NameTable);
        ns.AddNamespace("rim", XdsConstants.EbRim3Ns);
        ns.AddNamespace("rs",  XdsConstants.EbRs3Ns);
        ns.AddNamespace("q",   XdsConstants.EbQ3Ns);

        var entries = new List<DocumentEntry>();
        var nodes = response.SelectNodes("//rim:ExtrinsicObject", ns);
        if (nodes is null) return entries;

        foreach (XmlNode node in nodes)
        {
            try
            {
                entries.Add(MapToDocumentEntry(node, ns, patientIns));
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Impossible de mapper un ExtrinsicObject du DMP.");
            }
        }

        Logger.LogInformation("{Count} document(s) trouvé(s) pour le patient {Ins}", entries.Count, patientIns);
        return entries;
    }

    private static DocumentEntry MapToDocumentEntry(XmlNode node, XmlNamespaceManager ns, Ins ins)
    {
        string Slot(string name) =>
            node.SelectSingleNode($"rim:Slot[@name='{name}']/rim:ValueList/rim:Value", ns)?.InnerText
            ?? string.Empty;

        string Classification(string scheme) =>
            node.SelectSingleNode($"rim:Classification[@classificationScheme='{scheme}']/@nodeRepresentation", ns)?.Value
            ?? string.Empty;

        var status = node.Attributes?["status"]?.Value ?? string.Empty;

        return new DocumentEntry
        {
            UniqueId           = new DocumentUniqueId(node.Attributes?["id"]?.Value ?? Guid.NewGuid().ToString()),
            RepositoryUniqueId = new RepositoryUniqueId(Slot("repositoryUniqueId")),
            EntryUuid          = node.Attributes?["id"]?.Value,
            HomeCommunityId    = node.Attributes?["home"]?.Value,
            PatientIns         = ins,
            Title              = node.SelectSingleNode("rim:Name/rim:LocalizedString/@value", ns)?.Value,
            Status             = status.Contains("Approved") ? DocumentStatus.Approved
                               : status.Contains("Deprecated") ? DocumentStatus.Deprecated
                               : DocumentStatus.Unknown,
            ClassCode          = Classification(XdsConstants.ClassCodeScheme),
            TypeCode           = Classification(XdsConstants.TypeCodeScheme),
            FormatCode         = Classification(XdsConstants.FormatCodeScheme),
            MimeType           = node.Attributes?["mimeType"]?.Value,
            LanguageCode       = Slot("languageCode"),
            CreationTime       = ParseDtmTime(Slot("creationTime")),
            ServiceStartTime   = ParseDtmTime(Slot("serviceStartTime")),
            ServiceStopTime    = ParseDtmTime(Slot("serviceStopTime")),
            AuthorInstitution  = node.SelectSingleNode(
                $"rim:Classification/rim:Slot[@name='authorInstitution']/rim:ValueList/rim:Value", ns)?.InnerText,
            AuthorPerson       = node.SelectSingleNode(
                $"rim:Classification/rim:Slot[@name='authorPerson']/rim:ValueList/rim:Value", ns)?.InnerText,
            Size               = long.TryParse(Slot("size"), out var sz) ? sz : null,
            Hash               = Slot("hash")
        };
    }

    private static DateTimeOffset? ParseDtmTime(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        // Format DTM IHE: yyyyMMddHHmmss ou yyyyMMdd
        var formats = new[] { "yyyyMMddHHmmss", "yyyyMMddHHmm", "yyyyMMdd" };
        foreach (var fmt in formats)
            if (DateTimeOffset.TryParseExact(value, fmt,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.AssumeUniversal, out var dt))
                return dt;
        return null;
    }
}
