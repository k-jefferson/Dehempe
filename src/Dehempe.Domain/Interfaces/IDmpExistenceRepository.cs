using Dehempe.Domain.Entities;
using Dehempe.Domain.ValueObjects;

namespace Dehempe.Domain.Interfaces;

/// <summary>
/// Service DMP de gestion de dossier patient partagé (TD 0.x).
/// </summary>
public interface IDmpExistenceRepository
{
    /// <summary>
    /// TD 0.2 — Teste l'existence du DMP d'un patient (message HL7 V3 <c>PRPA_IN201307UV02</c>).
    /// </summary>
    Task<DmpExistenceResult> CheckExistenceAsync(Ins patientIns, CancellationToken ct = default);
}
