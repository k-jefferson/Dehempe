using System.Linq;
using System.Xml;
using Dehempe.Application.Common.Interfaces;
using Dehempe.Domain.Entities;
using Dehempe.Domain.Exceptions;
using Dehempe.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dehempe.Infrastructure.Dmp.Soap;

/// <summary>
/// Client SOAP pour l'ITI-43 (Retrieve Document Set) et ITI-41 (Provide and Register).
/// </summary>
internal sealed class XdsRepositoryClient : XdsSoapClientBase
{
    private readonly DmpOptions _options;
    private readonly IVihfContextAccessor _vihfCtxAccessor;

    public XdsRepositoryClient(
        HttpClient http,
        IVihfService vihf,
        ICpsAuthService cpsAuth,
        IOptions<DmpOptions> options,
        IVihfContextAccessor vihfCtxAccessor,
        ILogger<XdsRepositoryClient> logger,
        ISoapRequestCapture capture)
        : base(http, vihf, cpsAuth, logger, capture)
    {
        _options = options.Value;
        _vihfCtxAccessor = vihfCtxAccessor;
    }

    public async Task<DocumentContent> RetrieveDocumentAsync(
        DocumentUniqueId uniqueId,
        RepositoryUniqueId repositoryUniqueId,
        string? homeCommunityId,
        Ins patientIns,
        CancellationToken ct)
    {
        var doc = new XmlDocument();
        var body = BuildRetrieveBody(doc, uniqueId, repositoryUniqueId, homeCommunityId ?? _options.HomeCommunityId);
        var vihfCtx = _vihfCtxAccessor.GetContext(patientIns);

        var response = await SendSoapWithAttachmentsAsync(
            _options.RepositoryEndpoint,
            XdsConstants.Iti43Action,
            body,
            vihfCtx,
            ct);

        return ParseRetrieveResponse(response, uniqueId, repositoryUniqueId);
    }

    private static XmlElement BuildRetrieveBody(
        XmlDocument doc,
        DocumentUniqueId uniqueId,
        RepositoryUniqueId repositoryUniqueId,
        string homeCommunityId)
    {
        var retrieve = doc.CreateElement("ihe", "RetrieveDocumentSetRequest", XdsConstants.IheXdsNs);
        retrieve.SetAttribute("xmlns:ihe", XdsConstants.IheXdsNs);

        var docReq = doc.CreateElement("ihe", "DocumentRequest", XdsConstants.IheXdsNs);
        retrieve.AppendChild(docReq);

        var hcid = doc.CreateElement("ihe", "HomeCommunityId", XdsConstants.IheXdsNs);
        hcid.InnerText = homeCommunityId;
        docReq.AppendChild(hcid);

        var repo = doc.CreateElement("ihe", "RepositoryUniqueId", XdsConstants.IheXdsNs);
        repo.InnerText = repositoryUniqueId.Value;
        docReq.AppendChild(repo);

        var did = doc.CreateElement("ihe", "DocumentUniqueId", XdsConstants.IheXdsNs);
        did.InnerText = uniqueId.Value;
        docReq.AppendChild(did);

        return retrieve;
    }

    private DocumentContent ParseRetrieveResponse(
        SoapResult response,
        DocumentUniqueId uniqueId,
        RepositoryUniqueId repositoryUniqueId)
    {
        var ns = new XmlNamespaceManager(response.Xml.NameTable);
        ns.AddNamespace("ihe", XdsConstants.IheXdsNs);
        ns.AddNamespace("xop", XdsConstants.XopNs);

        var docNode = response.Xml.SelectSingleNode("//ihe:DocumentResponse", ns);
        if (docNode is null)
            throw new DmpDocumentNotFoundException(uniqueId.Value);

        var mimeType = docNode.SelectSingleNode("ihe:mimeType", ns)?.InnerText ?? "application/octet-stream";

        // Le contenu peut être transmis de deux façons :
        //  - MTOM/XOP : <ihe:Document><xop:Include href="cid:..."/></ihe:Document> → octets en pièce jointe ;
        //  - base64 inline : <ihe:Document>BASE64</ihe:Document> (cas dégradé sans MTOM).
        var xopHref = docNode.SelectSingleNode("ihe:Document/xop:Include/@href", ns)?.Value;
        byte[] data;
        if (!string.IsNullOrEmpty(xopHref))
        {
            var cid = MtomParser.NormalizeCid(xopHref);
            if (!response.Attachments.TryGetValue(cid, out var bytes))
            {
                // Repli : si une seule pièce jointe, c'est forcément le document.
                if (response.Attachments.Count == 1)
                    bytes = response.Attachments.Values.First();
                else
                    throw new DmpException(
                        $"Pièce jointe MTOM introuvable pour le document (cid={cid}, " +
                        $"{response.Attachments.Count} pièce(s) jointe(s) reçue(s)).",
                        "MTOM_CID_NOT_FOUND");
            }
            data = bytes;
        }
        else
        {
            var docData = docNode.SelectSingleNode("ihe:Document", ns)?.InnerText?.Trim() ?? string.Empty;
            data = Convert.FromBase64String(docData);
        }

        Logger.LogInformation(
            "Document {UniqueId} récupéré ({Size} octets, {MimeType}).",
            uniqueId.Value, data.Length, mimeType);

        return new DocumentContent
        {
            UniqueId           = uniqueId,
            RepositoryUniqueId = repositoryUniqueId,
            MimeType           = mimeType,
            Data               = data
        };
    }
}
