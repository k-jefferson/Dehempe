namespace Dehempe.Infrastructure.Dmp.Soap;

public sealed class DmpOptions
{
    public const string SectionName = "Dmp";

    /// <summary>URL du service ITI-18 (Registry Stored Query).</summary>
    public string RegistryEndpoint { get; set; } = "https://lps.dev1.dmp.gouv.fr/si-dmp-server/v2/services";

    /// <summary>URL du service ITI-43 / ITI-41 (Repository).</summary>
    public string RepositoryEndpoint { get; set; } = "https://lps.dev1.dmp.gouv.fr/si-dmp-server/v2/services";

    /// <summary>OID du repository XDS cible.</summary>
    public string RepositoryUniqueId { get; set; } = string.Empty;

    /// <summary>HomeCommunityId du réseau DMP.</summary>
    public string HomeCommunityId { get; set; } = "2.16.840.1.113883.2.8.3.7";

    /// <summary>URL du service GDP (TD 0.x — Gestion Dossier Patient Partagé).</summary>
    public string GdpEndpoint { get; set; } = "https://lps.dev1.dmp.gouv.fr/si-dmp-server/v2/services";

    /// <summary>OID racine identifiant le logiciel LPS (champ <c>sender/device/id</c> HL7 V3).</summary>
    public string LpsDeviceOid { get; set; } = "1.3.6.1.4.1.48364";

    /// <summary>Nom du logiciel LPS publié dans les messages HL7 V3.</summary>
    public string LpsSoftwareName { get; set; } = "Déhempé";

    /// <summary>Identifiant ANS du LPS (attribut <c>LPS_ID</c> du VIHF). Ex: "01.01.01.01".</summary>
    public string LpsId { get; set; } = "01.01.01.01";

    /// <summary>Version applicative du LPS (attribut <c>LPS_Version</c> du VIHF).</summary>
    public string LpsVersion { get; set; } = "1.0";

    /// <summary>Numéro d'homologation DMP du LPS (attribut <c>LPS_ID_HOMOLOGATION_DMP</c>).</summary>
    public string LpsAuthorizationId { get; set; } = "NumAutorisation";

    /// <summary>Secteur d'activité du PS au format ANS « SAxx^1.2.250.1.71.4.2.4 ». Ex: SA07 = libéral.</summary>
    public string OrganizationSector { get; set; } = "SA07^1.2.250.1.71.4.2.4";

    /// <summary>Délai d'expiration des requêtes SOAP en secondes.</summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// URL d'un proxy mTLS local (stunnel + PKCS#11) qui termine le mTLS côté carte CPS.
    /// Quand renseigné, toutes les requêtes DMP sont redirigées vers ce proxy en HTTP simple,
    /// avec le Host header de l'URL DMP d'origine préservé pour la routage côté DMP.
    /// Format : <c>http://127.0.0.1:5443</c>. Vide = pas de tunnel (mTLS direct natif).
    /// </summary>
    public string? TunnelEndpoint { get; set; }
}
