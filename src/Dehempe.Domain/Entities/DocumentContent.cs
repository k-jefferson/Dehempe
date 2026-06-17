using Dehempe.Domain.ValueObjects;

namespace Dehempe.Domain.Entities;

/// <summary>
/// Contenu binaire d'un document récupéré via ITI-43 (Retrieve Document Set).
/// </summary>
public class DocumentContent
{
    public DocumentUniqueId UniqueId { get; init; } = null!;
    public RepositoryUniqueId RepositoryUniqueId { get; init; } = null!;
    public string MimeType { get; init; } = null!;
    public byte[] Data { get; init; } = null!;
}
