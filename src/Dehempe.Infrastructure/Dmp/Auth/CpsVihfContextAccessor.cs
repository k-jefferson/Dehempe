using Dehempe.Application.Common.Interfaces;
using Dehempe.Domain.ValueObjects;
using Dehempe.Infrastructure.Dmp.Soap;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dehempe.Infrastructure.Dmp.Auth;

/// <summary>
/// Construit le VihfContext en lisant l'identité du praticien directement
/// dans le certificat CPS branché à la machine, et l'INS du patient depuis la route HTTP.
/// </summary>
internal sealed class CpsVihfContextAccessor : IVihfContextAccessor
{
    private readonly ICpsAuthService _cpsAuth;
    private readonly CpsOptions _options;
    private readonly IHttpContextAccessor _http;
    private readonly ILogger<CpsVihfContextAccessor> _logger;

    // Cache : le certificat ne change pas pendant la durée de vie du service
    private CpsPractitionerIdentity? _identity;

    public CpsVihfContextAccessor(
        ICpsAuthService cpsAuth,
        IOptions<CpsOptions> options,
        IHttpContextAccessor http,
        ILogger<CpsVihfContextAccessor> logger)
    {
        _cpsAuth = cpsAuth;
        _options = options.Value;
        _http    = http;
        _logger  = logger;
    }

    public VihfContext GetContext(Ins patientIns)
    {
        var identity = GetIdentity();
        var ins      = ResolvePatientIns(patientIns);

        return new VihfContext(
            PractitionerRole:       identity.RoleCode,
            PractitionerIdentifier: identity.Identifier,
            PractitionerName:       identity.Name,
            OrganizationId:         _options.OrganizationId,
            OrganizationName:       identity.OrgName,
            PatientIns:             ins.Value,
            PatientInsOid:          ins.Oid.ToOidString(),
            AccessContext:          AccessContext.Normal
        );
    }

    private CpsPractitionerIdentity GetIdentity()
    {
        if (_identity is not null) return _identity;

        // GetAwaiter().GetResult() est acceptable ici : appelé une seule fois,
        // le cert est en cache dans CpsAuthService après le premier appel.
        var cert = _cpsAuth.GetCertificateAsync().GetAwaiter().GetResult();
        _identity = CpsCertificateParser.Parse(cert);

        _logger.LogInformation(
            "Identité CPS chargée — Praticien : {Name} ({Id}), Rôle : {Role}, Structure : {Org}",
            _identity.Name, _identity.Identifier, _identity.RoleCode, _identity.OrgName);

        return _identity;
    }

    private Ins ResolvePatientIns(Ins? patientIns)
    {
        if (patientIns is not null) return patientIns;

        // Lit l'INS depuis la route HTTP : /api/patients/{ins}/...
        var routeIns = _http.HttpContext?.Request.RouteValues["ins"]?.ToString();
        if (string.IsNullOrWhiteSpace(routeIns))
            throw new InvalidOperationException(
                "INS du patient introuvable : ni passé en paramètre ni présent dans la route HTTP.");

        // Détermine NIR ou NIA selon la longueur (NIR = 15 chiffres)
        return routeIns.Length == 15 && routeIns.All(char.IsDigit)
            ? Ins.CreateNir(routeIns)
            : Ins.CreateNia(routeIns);
    }
}
