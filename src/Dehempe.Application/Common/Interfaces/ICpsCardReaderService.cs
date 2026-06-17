using Dehempe.Application.Cps.DTOs;

namespace Dehempe.Application.Common.Interfaces;

/// <summary>
/// Lit les données du porteur et de la carte CPS depuis le certificat X.509
/// publié par le middleware CPS dans le magasin du système.
/// </summary>
public interface ICpsCardReaderService
{
    Task<CpsCardDto> ReadCardAsync(CancellationToken ct = default);
}
