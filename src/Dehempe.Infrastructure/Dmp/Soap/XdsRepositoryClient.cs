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
        ILogger<XdsRepositoryClient> logger)
        : base(http, vihf, cpsAuth, logger)
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

        var response = await SendSoapAsync(
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

    private static DocumentContent ParseRetrieveResponse(
        XmlDocument response,
        DocumentUniqueId uniqueId,
        RepositoryUniqueId repositoryUniqueId)
    {
        var ns = new XmlNamespaceManager(response.NameTable);
        ns.AddNamespace("ihe", XdsConstants.IheXdsNs);

        var docNode = response.SelectSingleNode("//ihe:DocumentResponse", ns);
        if (docNode is null)
            throw new DmpDocumentNotFoundException(uniqueId.Value);

        var mimeType = docNode.SelectSingleNode("ihe:mimeType", ns)?.InnerText ?? "application/octet-stream";
        var docData  = docNode.SelectSingleNode("ihe:Document", ns)?.InnerText ?? string.Empty;

        return new DocumentContent
        {
            UniqueId           = uniqueId,
            RepositoryUniqueId = repositoryUniqueId,
            MimeType           = mimeType,
            Data               = Convert.FromBase64String(docData)
        };
    }
}
