using Dehempe.Application.Documents.DTOs;
using Dehempe.Domain.Entities;
using Dehempe.Domain.Interfaces;
using Dehempe.Domain.ValueObjects;
using MediatR;

namespace Dehempe.Application.Documents.Queries;

public record GetDocumentListQuery(
    string Ins,
    string InsOid,
    DateTimeOffset? CreatedAfter = null,
    DateTimeOffset? CreatedBefore = null,
    string? Status = null,
    IReadOnlyList<string>? ClassCodes = null
) : IRequest<IReadOnlyList<DocumentEntryDto>>;

internal sealed class GetDocumentListQueryHandler
    : IRequestHandler<GetDocumentListQuery, IReadOnlyList<DocumentEntryDto>>
{
    private readonly IDmpDocumentRepository _repository;

    public GetDocumentListQueryHandler(IDmpDocumentRepository repository)
        => _repository = repository;

    public async Task<IReadOnlyList<DocumentEntryDto>> Handle(
        GetDocumentListQuery request,
        CancellationToken cancellationToken)
    {
        var ins = request.InsOid == InsOidValues.Nir
            ? Ins.CreateNir(request.Ins)
            : Ins.CreateNia(request.Ins);

        DocumentStatus? statusFilter = request.Status?.ToUpperInvariant() switch
        {
            "APPROVED"    => DocumentStatus.Approved,
            "DEPRECATED"  => DocumentStatus.Deprecated,
            _             => null
        };

        var criteria = new DocumentSearchCriteria(
            CreatedAfter:  request.CreatedAfter,
            CreatedBefore: request.CreatedBefore,
            Status:        statusFilter,
            ClassCodes:    request.ClassCodes
        );

        var entries = await _repository.FindDocumentsAsync(ins, criteria, cancellationToken);
        return entries.Select(ToDto).ToList();
    }

    private static DocumentEntryDto ToDto(DocumentEntry e) => new(
        UniqueId:           e.UniqueId.Value,
        RepositoryUniqueId: e.RepositoryUniqueId.Value,
        HomeCommunityId:    e.HomeCommunityId,
        Title:              e.Title,
        Status:             e.Status.ToString(),
        ClassCode:          e.ClassCode,
        TypeCode:           e.TypeCode,
        FormatCode:         e.FormatCode,
        MimeType:           e.MimeType,
        CreationTime:       e.CreationTime,
        ServiceStartTime:   e.ServiceStartTime,
        ServiceStopTime:    e.ServiceStopTime,
        AuthorInstitution:  e.AuthorInstitution,
        AuthorPerson:       e.AuthorPerson
    );
}
