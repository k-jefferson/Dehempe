namespace Dehempe.Infrastructure.Dmp.Auth;

public sealed class CpsOptions
{
    public const string SectionName = "Cps";

    /// <summary>
    /// Chemin vers le fichier .p12 / .pfx contenant le certificat CPS et sa clé privée.
    /// Laisser vide si le certificat est dans le magasin Windows/macOS.
    /// </summary>
    public string? CertificatePath { get; set; }

    /// <summary>Mot de passe du fichier .p12 (si CertificatePath est renseigné).</summary>
    public string? CertificatePassword { get; set; }

    /// <summary>
    /// Empreinte (thumbprint SHA-1) pour chercher le certificat dans le magasin système.
    /// Utilisé si CertificatePath est vide.
    /// </summary>
    public string? CertificateThumbprint { get; set; }

    /// <summary>
    /// Emplacement du magasin de certificats (CurrentUser ou LocalMachine).
    /// Par défaut CurrentUser.
    /// </summary>
    public string StoreLocation { get; set; } = "CurrentUser";

    /// <summary>
    /// OID de la structure d'exercice au format ANS : "1.2.250.1.71.4.2.2/&lt;FINESS&gt;".
    /// Non présent dans tous les certificats CPS — à renseigner si absent du Subject.
    /// </summary>
    public string OrganizationId { get; set; } = string.Empty;
}
