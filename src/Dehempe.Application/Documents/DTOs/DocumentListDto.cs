namespace Dehempe.Application.Documents.DTOs;

/// <summary>
/// Réponse de la liste de documents DMP (ITI-18).
/// Quand <see cref="Documents"/> est vide, <see cref="DmpRequest"/> et <see cref="DmpResponse"/>
/// portent le XML SOAP brut échangé avec le DMP (requête envoyée + réponse reçue) afin de
/// diagnostiquer l'absence de résultat. Ils valent <c>null</c> dès qu'au moins un document est renvoyé.
/// </summary>
public record DocumentListDto(
    IReadOnlyList<DocumentEntryDto> Documents,
    string? DmpRequest,
    string? DmpResponse);
