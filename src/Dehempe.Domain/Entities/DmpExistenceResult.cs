using Dehempe.Domain.ValueObjects;

namespace Dehempe.Domain.Entities;

/// <summary>
/// Résultat de la TD 0.2 (Test d'existence d'un DMP).
/// Mappé depuis le message HL7 V3 <c>PRPA_IN201308UV02</c>.
/// </summary>
public sealed record DmpExistenceResult(
    Ins   PatientIns,
    bool  Exists,
    string QueryResponseCode,
    string AckTypeCode,
    bool? IsAuthorizationValid,
    bool? IsAttachedToTreatingPhysician,
    DmpPatientInfo? Patient,
    string? ErrorMessage);

/// <summary>Informations patient retournées par TD 0.2 quand le DMP existe.</summary>
public sealed record DmpPatientInfo(
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
