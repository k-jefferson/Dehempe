using Dehempe.Domain.ValueObjects;

namespace Dehempe.Domain.Entities;

/// <summary>
/// Métadonnées d'un document XDS (ITI-18 RegisterDocumentSetResponse / AdhocQuery).
/// Correspond au DocumentEntry du registre XDS.b.
/// </summary>
public class DocumentEntry
{
    public DocumentUniqueId UniqueId { get; init; } = null!;
    public RepositoryUniqueId RepositoryUniqueId { get; init; } = null!;

    public string? EntryUuid { get; init; }
    public string? HomeCommunityId { get; init; }

    public Ins PatientIns { get; init; } = null!;

    public string? Title { get; init; }
    public DocumentStatus Status { get; init; }
    public string? ClassCode { get; init; }
    public string? TypeCode { get; init; }
    public string? FormatCode { get; init; }
    public string? MimeType { get; init; }
    public string? LanguageCode { get; init; }
    public string? PracticeSettingCode { get; init; }
    public string? HealthcareFacilityTypeCode { get; init; }

    public DateTimeOffset? CreationTime { get; init; }
    public DateTimeOffset? ServiceStartTime { get; init; }
    public DateTimeOffset? ServiceStopTime { get; init; }

    public string? AuthorInstitution { get; init; }
    public string? AuthorPerson { get; init; }

    public long? Size { get; init; }
    public string? Hash { get; init; }
}

public enum DocumentStatus
{
    Approved,
    Deprecated,
    Unknown
}
