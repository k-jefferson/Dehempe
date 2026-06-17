using Dehempe.Application.Dmp.DTOs;
using Dehempe.Domain.Entities;
using Dehempe.Domain.Interfaces;
using Dehempe.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Dehempe.Application.Dmp.Queries;

/// <summary>
/// TD 0.2 — Vérifie si un patient identifié par son INS possède un DMP.
/// La carte CPS branchée fournit l'identité du praticien (via VIHF).
/// </summary>
public record CheckDmpExistsQuery(string Ins, string InsOid) : IRequest<DmpExistenceDto>;

public sealed class CheckDmpExistsQueryValidator : AbstractValidator<CheckDmpExistsQuery>
{
    public CheckDmpExistsQueryValidator()
    {
        RuleFor(q => q.Ins)
            .NotEmpty()
            .Matches(@"^\d{15}$")
            .When(q => q.InsOid == InsOidValues.Nir)
            .WithMessage("Le NIR doit comporter exactement 15 chiffres.");

        RuleFor(q => q.InsOid)
            .Must(oid => oid == InsOidValues.Nir || oid == InsOidValues.Nia)
            .WithMessage($"L'OID INS doit être {InsOidValues.Nir} (NIR) ou {InsOidValues.Nia} (NIA).");
    }
}

internal sealed class CheckDmpExistsQueryHandler
    : IRequestHandler<CheckDmpExistsQuery, DmpExistenceDto>
{
    private readonly IDmpExistenceRepository _repo;

    public CheckDmpExistsQueryHandler(IDmpExistenceRepository repo) => _repo = repo;

    public async Task<DmpExistenceDto> Handle(CheckDmpExistsQuery request, CancellationToken ct)
    {
        var ins = request.InsOid == InsOidValues.Nir
            ? Ins.CreateNir(request.Ins)
            : Ins.CreateNia(request.Ins);

        var result = await _repo.CheckExistenceAsync(ins, ct);
        return Map(result);
    }

    private static DmpExistenceDto Map(DmpExistenceResult r) => new(
        PatientIns:                    r.PatientIns.Value,
        Exists:                        r.Exists,
        QueryResponseCode:             r.QueryResponseCode,
        AckTypeCode:                   r.AckTypeCode,
        IsAuthorizationValid:          r.IsAuthorizationValid,
        IsAttachedToTreatingPhysician: r.IsAttachedToTreatingPhysician,
        Patient:                       r.Patient is null ? null : MapPatient(r.Patient),
        ErrorMessage:                  r.ErrorMessage);

    private static DmpPatientDto MapPatient(DmpPatientInfo p) => new(
        InsC:               p.InsC,
        InsNir:             p.InsNir,
        Status:             p.Status,
        Prefix:             p.Prefix,
        GivenName:          p.GivenName,
        FamilyName:         p.FamilyName,
        Email:              p.Email,
        Phone:              p.Phone,
        GenderCode:         p.GenderCode,
        BirthDate:          p.BirthDate,
        HasInternetAccount: p.HasInternetAccount,
        IsAttachedToEns:    p.IsAttachedToEns);
}
