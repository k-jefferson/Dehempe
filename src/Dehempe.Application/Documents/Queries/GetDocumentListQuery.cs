using Dehempe.Application.Documents.DTOs;
using Dehempe.Domain.Entities;
using Dehempe.Domain.Interfaces;
using Dehempe.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Dehempe.Application.Documents.Queries;

public record GetDocumentListQuery(
    string Ins,
    string InsOid,
    DateTimeOffset? CreatedAfter = null,
    DateTimeOffset? CreatedBefore = null,
    IReadOnlyList<string>? ClassCodes = null
) : IRequest<IReadOnlyList<DocumentEntryDto>>;

public sealed class GetDocumentListQueryValidator : AbstractValidator<GetDocumentListQuery>
{
    public GetDocumentListQueryValidator()
    {
        RuleFor(q => q.Ins)
            .NotEmpty()
            .Matches(@"^\d{15}$")
            .When(q => q.InsOid == InsOidValues.Nir)
            .WithMessage("Le NIR doit comporter exactement 15 chiffres.");

        RuleFor(q => q.InsOid)
            .Must(oid => oid == InsOidValues.Nir || oid == InsOidValues.Nia)
            .WithMessage($"L'OID INS doit être {InsOidValues.Nir} (NIR) ou {InsOidValues.Nia} (NIA).");

        RuleFor(q => q.CreatedAfter)
            .Must((q, _) => q.CreatedAfter <= q.CreatedBefore)
            .When(q => q.CreatedAfter.HasValue && q.CreatedBefore.HasValue)
            .WithMessage("createdAfter doit être antérieure ou égale à createdBefore.");
    }
}

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

        // L'API n'expose plus de filtre de statut : seuls les documents APPROVED sont interrogés.
        var criteria = new DocumentSearchCriteria(
            CreatedAfter:  request.CreatedAfter,
            CreatedBefore: request.CreatedBefore,
            Status:        DocumentStatus.Approved,
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
