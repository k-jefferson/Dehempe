using Dehempe.Application.Common.Interfaces;
using Dehempe.Application.Documents.DTOs;
using Dehempe.Application.Documents.Queries;
using Dehempe.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dehempe.API.Controllers;

[ApiController]
[Route("api/patients/{ins}/documents")]
public sealed class DocumentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ISoapRequestCapture _soapCapture;

    public DocumentsController(IMediator mediator, ISoapRequestCapture soapCapture)
    {
        _mediator    = mediator;
        _soapCapture = soapCapture;
    }

    /// <summary>
    /// Liste les métadonnées des documents DMP d'un patient (ITI-18).
    /// Seuls les documents au statut <c>APPROVED</c> sont interrogés.
    /// </summary>
    /// <param name="ins">NIR ou NIA du patient (15 chiffres pour le NIR).</param>
    /// <param name="insOid">OID de l'INS. Défaut : 1.2.250.1.213.1.4.8 (NIR).</param>
    /// <param name="createdAfter">Filtre sur la date de création (ISO 8601). Défaut Swagger : aujourd'hui − 30 jours.</param>
    /// <param name="createdBefore">Filtre sur la date de création (ISO 8601). Défaut Swagger : aujourd'hui.</param>
    /// <param name="classCode">Un ou plusieurs codes de classe (répéter le paramètre).</param>
    [HttpGet]
    [ProducesResponseType(typeof(DocumentListDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDocuments(
        string ins,
        [FromQuery] string insOid = InsOidValues.Nir,
        [FromQuery] DateTimeOffset? createdAfter = null,
        [FromQuery] DateTimeOffset? createdBefore = null,
        [FromQuery(Name = "classCode")] List<string>? classCode = null,
        CancellationToken ct = default)
    {
        var documents = await _mediator.Send(new GetDocumentListQuery(
            Ins: ins,
            InsOid: insOid,
            CreatedAfter: createdAfter,
            CreatedBefore: createdBefore,
            ClassCodes: classCode
        ), ct);

        // Diagnostic : si aucun document n'est renvoyé, on joint le XML SOAP brut
        // (requête envoyée + réponse reçue) échangé avec le DMP pour comprendre pourquoi
        // le registre ne renvoie rien. Champs null dès qu'au moins un document est présent.
        var empty = documents.Count == 0;
        return Ok(new DocumentListDto(
            Documents:   documents,
            DmpRequest:  empty ? _soapCapture.LastRequest  : null,
            DmpResponse: empty ? _soapCapture.LastResponse : null));
    }

    /// <summary>
    /// Récupère le contenu binaire d'un document DMP (ITI-43).
    /// </summary>
    /// <param name="ins">NIR ou NIA du patient.</param>
    /// <param name="uniqueId">Identifiant unique du document (DocumentUniqueId XDS).</param>
    /// <param name="repositoryUniqueId">OID du dépôt XDS.</param>
    /// <param name="homeCommunityId">HomeCommunityId optionnel.</param>
    [HttpGet("{uniqueId}/content")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDocumentContent(
        string ins,
        string uniqueId,
        [FromQuery] string repositoryUniqueId,
        [FromQuery] string? homeCommunityId = null,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetDocumentContentQuery(
            UniqueId: Uri.UnescapeDataString(uniqueId),
            RepositoryUniqueId: repositoryUniqueId,
            HomeCommunityId: homeCommunityId
        ), ct);

        return File(result.Data, result.MimeType);
    }
}
