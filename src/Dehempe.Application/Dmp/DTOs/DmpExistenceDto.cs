namespace Dehempe.Application.Dmp.DTOs;

/// <summary>
/// Réponse de la TD 0.2 — test d'existence du DMP d'un patient.
/// </summary>
public record DmpExistenceDto(
    string PatientIns,
    bool   Exists,
    string QueryResponseCode,
    string AckTypeCode,
    bool?  IsAuthorizationValid,
    bool?  IsAttachedToTreatingPhysician,
    DmpPatientDto? Patient,
    string? ErrorMessage,
    string? Request);

public record DmpPatientDto(
    string?   InsC,
    string?   InsNir,
    string?   Status,
    string?   Prefix,
    string?   GivenName,
    string?   FamilyName,
    string?   Email,
    string?   Phone,
    string?   GenderCode,
    DateOnly? BirthDate,
    bool?     HasInternetAccount,
    bool?     IsAttachedToEns);
