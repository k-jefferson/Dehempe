using Dehempe.Domain.Entities;
using Dehempe.Domain.ValueObjects;

namespace Dehempe.Domain.Interfaces;

public interface IDmpDocumentRepository
{
    /// <summary>ITI-18 — Recherche les métadonnées des documents d'un patient.</summary>
    Task<IReadOnlyList<DocumentEntry>> FindDocumentsAsync(
        Ins patientIns,
        DocumentSearchCriteria? criteria = null,
        CancellationToken ct = default);

    /// <summary>ITI-43 — Récupère le contenu binaire d'un document.</summary>
    Task<DocumentContent> RetrieveDocumentAsync(
        DocumentUniqueId uniqueId,
        RepositoryUniqueId repositoryUniqueId,
        string? homeCommunityId = null,
        CancellationToken ct = default);
}

public record DocumentSearchCriteria(
    DateTimeOffset? CreatedAfter = null,
    DateTimeOffset? CreatedBefore = null,
    DocumentStatus? Status = null,
    IReadOnlyList<string>? ClassCodes = null,
    IReadOnlyList<string>? FormatCodes = null
);
