using Dehempe.Application.Common.Interfaces;
using Dehempe.Domain.ValueObjects;
using Dehempe.Infrastructure.Dmp.Auth.Pkcs11;
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

    /// <summary>OID de la nomenclature « secteur d'activité » (TRE_R03). Suffixe du champ Secteur_Activite.</summary>
    private const string SectorOid = "1.2.250.1.71.4.2.4";

    private readonly ICpsAuthService _cpsAuth;
    private readonly Pkcs11CpsKeyStore _pkcs11;
    private readonly CpsOptions _cpsOptions;
    private readonly DmpOptions _dmpOptions;
    private readonly IHttpContextAccessor _http;
    private readonly ILogger<CpsVihfContextAccessor> _logger;

    private CpsPractitionerIdentity? _identity;
    private string? _cachedSpecialityCode;
    private (string structureId, string sector)? _cachedStructure;

    public CpsVihfContextAccessor(
        ICpsAuthService cpsAuth,
        Pkcs11CpsKeyStore pkcs11,
        IOptions<CpsOptions> cpsOptions,
        IOptions<DmpOptions> dmpOptions,
        IHttpContextAccessor http,
        ILogger<CpsVihfContextAccessor> logger)
    {
        _cpsAuth    = cpsAuth;
        _pkcs11     = pkcs11;
        _cpsOptions = cpsOptions.Value;
        _dmpOptions = dmpOptions.Value;
        _http       = http;
        _logger     = logger;
    }

    public VihfContext GetContext(Ins patientIns)
    {
        var identity = GetIdentity();
        var ins      = ResolvePatientIns(patientIns);

        var specialityCode          = GetSpecialityCode();
        var (structureId, sector)   = ResolveStructure();

        return new VihfContext(
            PractitionerRole:             identity.RoleCode,
            PractitionerRoleLabel:        identity.RoleLabel,
            PractitionerSpecialityCode:   specialityCode,
            PractitionerSpecialityLabel:  specialityCode,
            PractitionerIdentifier:       identity.Identifier,
            PractitionerName:       identity.Name,
            OrganizationId:         structureId,
            OrganizationName:       identity.OrgName,
            OrganizationSector:     sector,
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

        // L'identité praticien (RPPS, nom, prénom, profession) est encodée à l'identique
        // dans le DN du cert d'auth et du cert de signature ; on lit l'auth (toujours
        // chargé en premier et utilisé aussi par le wiring mTLS).
        var cert = _cpsAuth.GetAuthenticationCertificateAsync().GetAwaiter().GetResult();
        _identity = CpsCertificateParser.Parse(cert);

        _logger.LogInformation(
            "Identité CPS chargée — Praticien : {Name} ({Id}), Rôle : {RoleCode}/{RoleLabel}, Structure : {Org}",
            _identity.Name, _identity.Identifier, _identity.RoleCode, _identity.RoleLabel, _identity.OrgName);

        return _identity;
    }

    private string GetSpecialityCode()
    {
        if (_cachedSpecialityCode is not null) return _cachedSpecialityCode;
        _cachedSpecialityCode = _pkcs11.ReadSpecialityCode();
        return _cachedSpecialityCode;
    }

    /// <summary>
    /// Détermine le couple (Identifiant_Structure, Secteur_Activite) du VIHF.
    ///
    /// En authentification directe par CPS, ces deux valeurs DOIVENT être lues sur la carte
    /// et provenir du MÊME exercice, sinon le DMP rejette (« Structure introuvable ou Inactive »
    /// / « PS et Structure non liés »). On lit les <c>CPS_ACTIVITY_xx_PS</c>, on sélectionne
    /// l'exercice dont le secteur correspond à <see cref="DmpOptions.OrganizationSector"/>
    /// (sélecteur, ex: SA07 = libéral), et on dérive structure + secteur de cet exercice.
    ///
    /// Sans PKCS#11 (.p12 dev), on retombe sur les valeurs de config.
    /// </summary>
    private (string structureId, string sector) ResolveStructure()
    {
        if (_cachedStructure is not null) return _cachedStructure.Value;

        var activities = _pkcs11.ReadActivities();
        if (activities.Count > 0)
        {
            var preferred = ExtractSectorCode(_dmpOptions.OrganizationSector);
            var chosen = activities.FirstOrDefault(a =>
                             string.Equals(a.SectorCode, preferred, StringComparison.OrdinalIgnoreCase))
                         ?? activities[0];

            if (!string.Equals(chosen.SectorCode, preferred, StringComparison.OrdinalIgnoreCase))
                _logger.LogWarning(
                    "Aucun exercice CPS pour le secteur {Preferred} — fallback sur l'exercice {Index} (secteur {Sector}, structure {Struct}).",
                    preferred, chosen.Index, chosen.SectorCode, chosen.StructureId);

            var sector = $"{chosen.SectorCode}^{SectorOid}";
            _logger.LogInformation(
                "Structure VIHF lue sur la carte : {Struct} '{Name}' (secteur {Sector})",
                chosen.StructureId, chosen.StructureName, sector);
            _cachedStructure = (chosen.StructureId, sector);
            return _cachedStructure.Value;
        }

        _logger.LogWarning(
            "Aucun exercice CPS lisible (PKCS#11 indisponible ?) — fallback sur la config Cps:OrganizationId / Dmp:OrganizationSector.");
        _cachedStructure = (_cpsOptions.OrganizationId, _dmpOptions.OrganizationSector);
        return _cachedStructure.Value;
    }

    /// <summary>Extrait le code secteur (« SA07 ») d'une valeur « SA07^1.2.250.1.71.4.2.4 » ou « SA07 ».</summary>
    private static string ExtractSectorCode(string configured)
    {
        if (string.IsNullOrWhiteSpace(configured)) return "SA07";
        var caret = configured.IndexOf('^');
        return (caret >= 0 ? configured[..caret] : configured).Trim();
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
