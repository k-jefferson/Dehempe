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

    /// <summary>Délai d'expiration des requêtes SOAP en secondes.</summary>
    public int TimeoutSeconds { get; set; } = 30;
}
