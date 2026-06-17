using Dehempe.Application.Cps.DTOs;
using Dehempe.Application.Cps.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dehempe.API.Controllers;

[ApiController]
[Route("api/cps")]
public sealed class CpsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CpsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Lit les données de la carte CPS branchée. Récupère les informations
    /// du porteur (nom, prénom, RPPS, profession) et de la carte (numéro, dates).
    /// </summary>
    [HttpGet("card")]
    [ProducesResponseType(typeof(CpsCardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> GetCard(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCpsCardInfoQuery(), ct);
        return Ok(result);
    }
}
