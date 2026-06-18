using Dehempe.Application.Common.Interfaces;
using Dehempe.Domain.ValueObjects;
using Dehempe.Infrastructure.Dmp.Soap;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dehempe.Infrastructure.Dmp.Auth;

/// <summary>
/// Construit le VihfContext en assemblant :
/// - l'identité praticien depuis le certificat CPS branché (RPPS, rôle, nom)
/// - l'INS du patient depuis la route HTTP <c>/api/patients/{ins}/...</c>
/// - la conf LPS / structure depuis <see cref="DmpOptions"/> et <see cref="CpsOptions"/>
/// </summary>
internal sealed class CpsVihfContextAccessor : IVihfContextAccessor
{
    /// <summary>
    /// OID utilisé par le DMP pour l'INS dans les messages HL7 V3 (cf. exemple TD 0.2).
    /// Cohabite avec les OIDs « source » <see cref="InsOidValues.Nir"/> (1.4.8) et
    /// <see cref="InsOidValues.Nia"/> (1.4.9) ; le DMP attend ce 1.4.10 dans la
    /// resource-id du VIHF et dans le patientIdentifier de la requête.
    /// </summary>
    private const string DmpInsOid = "1.2.250.1.213.1.4.10";

    private readonly ICpsAuthService _cpsAuth;
    private readonly CpsOptions _cpsOptions;
    private readonly DmpOptions _dmpOptions;
    private readonly IHttpContextAccessor _http;
    private readonly ILogger<CpsVihfContextAccessor> _logger;

    private CpsPractitionerIdentity? _identity;

    public CpsVihfContextAccessor(
        ICpsAuthService cpsAuth,
        IOptions<CpsOptions> cpsOptions,
        IOptions<DmpOptions> dmpOptions,
        IHttpContextAccessor http,
        ILogger<CpsVihfContextAccessor> logger)
    {
        _cpsAuth    = cpsAuth;
        _cpsOptions = cpsOptions.Value;
        _dmpOptions = dmpOptions.Value;
        _http       = http;
        _logger     = logger;
    }

    public VihfContext GetContext(Ins patientIns)
    {
        var identity = GetIdentity();
        var ins      = ResolvePatientIns(patientIns);

        return new VihfContext(
            PractitionerRole:       identity.RoleCode,
            PractitionerRoleLabel:  identity.RoleLabel,
            PractitionerIdentifier: identity.Identifier,
            PractitionerName:       identity.Name,
            OrganizationId:         _cpsOptions.OrganizationId,
            OrganizationName:       identity.OrgName,
            OrganizationSector:     _dmpOptions.OrganizationSector,
            PatientIns:             ins.Value,
            PatientInsOid:          DmpInsOid,
            LpsId:                  _dmpOptions.LpsId,
            LpsName:                _dmpOptions.LpsSoftwareName,
            LpsVersion:             _dmpOptions.LpsVersion,
            LpsAuthorizationId:     _dmpOptions.LpsAuthorizationId,
            AccessContext:          AccessContext.Normal
        );
    }

    private CpsPractitionerIdentity GetIdentity()
    {
        if (_identity is not null) return _identity;

        var cert = _cpsAuth.GetCertificateAsync().GetAwaiter().GetResult();
        _identity = CpsCertificateParser.Parse(cert);

        _logger.LogInformation(
            "Identité CPS chargée — Praticien : {Name} ({Id}), Rôle : {RoleCode}/{RoleLabel}, Structure : {Org}",
            _identity.Name, _identity.Identifier, _identity.RoleCode, _identity.RoleLabel, _identity.OrgName);

        return _identity;
    }

    private Ins ResolvePatientIns(Ins? patientIns)
    {
        if (patientIns is not null) return patientIns;

        var routeIns = _http.HttpContext?.Request.RouteValues["ins"]?.ToString();
        if (string.IsNullOrWhiteSpace(routeIns))
            throw new InvalidOperationException(
                "INS du patient introuvable : ni passé en paramètre ni présent dans la route HTTP.");

        return routeIns.Length == 15 && routeIns.All(char.IsDigit)
            ? Ins.CreateNir(routeIns)
            : Ins.CreateNia(routeIns);
    }
}
