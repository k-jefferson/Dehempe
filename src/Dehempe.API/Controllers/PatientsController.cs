using System.ComponentModel;
using Dehempe.Application.Common.Interfaces;
using Dehempe.Application.Dmp.DTOs;
using Dehempe.Application.Dmp.Queries;
using Dehempe.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dehempe.API.Controllers;

[ApiController]
[Route("api/patients/{ins}")]
public sealed class PatientsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ISoapRequestCapture _soapCapture;

    public PatientsController(IMediator mediator, ISoapRequestCapture soapCapture)
    {
        _mediator    = mediator;
        _soapCapture = soapCapture;
    }

    /// <summary>
    /// TD 0.2 — Vérifie l'existence du DMP d'un patient.
    /// L'identité du praticien est lue automatiquement depuis la carte CPS branchée.
    /// </summary>
    /// <param name="ins">NIR (15 chiffres) ou NIA du patient.</param>
    /// <param name="insOid">OID de l'INS. Défaut : 1.2.250.1.213.1.4.8 (NIR).</param>
    /// <returns>Statut du DMP (existant / inexistant), informations patient si DMP trouvé. Le champ <c>request</c> contient toujours le XML SOAP brut envoyé au DMP.</returns>
    /// <response code="200">Test exécuté avec succès — voir <c>exists</c>.</response>
    /// <response code="400">INS invalide.</response>
    /// <response code="502">Erreur retournée par le DMP.</response>
    [HttpGet("dmp")]
    [Tags("TD0.2 - test d'existence")]
    [ProducesResponseType(typeof(DmpExistenceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> CheckDmpExists(
        [DefaultValue("277076322082910")] string ins,
        [FromQuery] string insOid = InsOidValues.Nir,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new CheckDmpExistsQuery(ins, insOid), ct);
        return Ok(result with { Request = _soapCapture.LastRequest });
    }
}
