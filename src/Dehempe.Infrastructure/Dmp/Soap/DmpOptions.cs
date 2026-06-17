namespace Dehempe.Infrastructure.Dmp.Soap;

public sealed class DmpOptions
{
    public const string SectionName = "Dmp";

    /// <summary>URL du service ITI-18 (Registry Stored Query).</summary>
    public string RegistryEndpoint { get; set; } = "https://dmp.asipsante.fr/services/RegistryService";

    /// <summary>URL du service ITI-43 / ITI-41 (Repository).</summary>
    public string RepositoryEndpoint { get; set; } = "https://dmp.asipsante.fr/services/RepositoryService";

    /// <summary>OID du repository XDS cible.</summary>
    public string RepositoryUniqueId { get; set; } = string.Empty;

    /// <summary>HomeCommunityId du réseau DMP.</summary>
    public string HomeCommunityId { get; set; } = "2.16.840.1.113883.2.8.3.7";

    /// <summary>Délai d'expiration des requêtes SOAP en secondes.</summary>
    public int TimeoutSeconds { get; set; } = 30;
}
