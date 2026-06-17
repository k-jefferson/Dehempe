namespace Dehempe.Application.Documents.DTOs;

public record DocumentEntryDto(
    string UniqueId,
    string RepositoryUniqueId,
    string? HomeCommunityId,
    string? Title,
    string Status,
    string? ClassCode,
    string? TypeCode,
    string? FormatCode,
    string? MimeType,
    DateTimeOffset? CreationTime,
    DateTimeOffset? ServiceStartTime,
    DateTimeOffset? ServiceStopTime,
    string? AuthorInstitution,
    string? AuthorPerson
);
