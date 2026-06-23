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

    public DocumentsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Liste les documents <c>Approved</c> du DMP d'un patient (TD3.1 / ITI-18 FindDocuments).
    /// </summary>
    /// <remarks>
    /// Le filtre temporel porte sur la date de soumission, approximée par la date de création XDS
    /// (<c>creationTime</c>). Le statut est toujours forcé à <c>Approved</c> (non paramétrable).
    /// Spécification de référence : <c>specs/recherche-documents.md</c>.
    /// </remarks>
    /// <param name="ins">NIR (15 chiffres) ou NIA du patient.</param>
    /// <param name="insOid">OID de l'INS. Défaut : 1.2.250.1.213.1.4.8 (NIR).</param>
    /// <param name="dateDebut">Borne basse de la fenêtre (ISO 8601). Défaut : aujourd'hui − 30 jours.</param>
    /// <param name="dateFin">Borne haute de la fenêtre (ISO 8601). Défaut : aujourd'hui.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<DocumentEntryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDocuments(
        string ins,
        [FromQuery] string insOid = InsOidValues.Nir,
        [FromQuery] DateTimeOffset? dateDebut = null,
        [FromQuery] DateTimeOffset? dateFin = null,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetDocumentListQuery(
            Ins: ins,
            InsOid: insOid,
            DateDebut: dateDebut,
            DateFin: dateFin
        ), ct);

        return Ok(result);
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
