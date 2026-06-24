using Dehempe.Application.Documents.DTOs;
using Dehempe.Domain.Entities;
using Dehempe.Domain.Interfaces;
using Dehempe.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Dehempe.Application.Documents.Queries;

/// <summary>
/// TD3.1 (ITI-18 / FindDocuments) — liste les documents <c>Approved</c> du DMP d'un patient
/// sur une fenêtre temporelle.
/// Le filtre temporel porte sur la date de soumission, approximée par la <c>creationTime</c> XDS.
/// Spécification de référence : <c>specs/recherche-documents.md</c>.
/// </summary>
/// <param name="DateDebut">Borne basse de la fenêtre. Défaut appliqué côté handler : aujourd'hui − 30 jours.</param>
/// <param name="DateFin">Borne haute de la fenêtre. Défaut appliqué côté handler : aujourd'hui.</param>
public record GetDocumentListQuery(
    string Ins,
    string InsOid,
    DateTimeOffset? DateDebut = null,
    DateTimeOffset? DateFin = null
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

        RuleFor(q => q.DateDebut)
            .Must((q, _) => q.DateDebut <= q.DateFin)
            .When(q => q.DateDebut.HasValue && q.DateFin.HasValue)
            .WithMessage("dateDebut doit être antérieure ou égale à dateFin.");
    }
}

internal sealed class GetDocumentListQueryHandler
    : IRequestHandler<GetDocumentListQuery, IReadOnlyList<DocumentEntryDto>>
{
    /// <summary>Fenêtre par défaut (en jours) appliquée quand aucune date n'est fournie (cf. spec §3.2).</summary>
    private const int DefaultWindowDays = 30;

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

        // Fenêtre par défaut : 30 derniers jours glissants, ancrés en UTC (cf. spec §3.2).
        var now       = DateTimeOffset.UtcNow;
        var dateDebut = request.DateDebut ?? now.AddDays(-DefaultWindowDays);
        var dateFin   = request.DateFin   ?? now;

        // Statut toujours forcé à Approved (EX_3.1-1030 ; cf. spec §3.2) — non paramétrable.
        var criteria = new DocumentSearchCriteria(
            CreatedAfter:  dateDebut,
            CreatedBefore: dateFin,
            Status:        DocumentStatus.Approved);

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
