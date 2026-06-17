using Dehempe.Application.Common.Interfaces;
using Dehempe.Application.Cps.DTOs;
using MediatR;

namespace Dehempe.Application.Cps.Queries;

public record GetCpsCardInfoQuery : IRequest<CpsCardDto>;

internal sealed class GetCpsCardInfoQueryHandler
    : IRequestHandler<GetCpsCardInfoQuery, CpsCardDto>
{
    private readonly ICpsCardReaderService _reader;

    public GetCpsCardInfoQueryHandler(ICpsCardReaderService reader)
        => _reader = reader;

    public Task<CpsCardDto> Handle(GetCpsCardInfoQuery request, CancellationToken cancellationToken)
        => _reader.ReadCardAsync(cancellationToken);
}
