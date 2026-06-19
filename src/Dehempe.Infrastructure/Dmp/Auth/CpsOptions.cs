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
    /// Identifiant de structure d'exercice (Struct_IdNat), valeur BRUTE sans préfixe OID
    /// (ex: "499700735741008"). <b>Fallback uniquement</b> : avec PKCS#11 (cas normal), la
    /// structure est lue sur la carte depuis les objets CPS_ACTIVITY_xx_PS — laisser vide.
    /// En authentification directe, le DMP exige le Struct_IdNat de la carte, pas une valeur
    /// de config. Voir CLAUDE.md § « Structure d'exercice & secteur ».
    /// </summary>
    public string OrganizationId { get; set; } = string.Empty;

    /// <summary>
    /// Override optionnel du chemin vers la librairie PKCS#11 du middleware CPS.
    /// Laisser vide pour auto-détection (Pkcs11CpsKeyStore sonde les emplacements connus
    /// par plateforme — <c>/usr/local/lib/libcps3_pkcs11_osx.dylib</c> sur macOS,
    /// <c>C:\Windows\System32\cps3_pkcs11_w64.dll</c> sur Windows, etc.).
    /// </summary>
    public string? Pkcs11LibraryPath { get; set; }

    /// <summary>
    /// Code PIN de la carte CPS.
    /// <para>
    /// <b>À ne pas renseigner en production</b> : le PIN est censé être saisi par le porteur
    /// au moment où le certificat CPS est utilisé pour la négociation mTLS, pas stocké en config.
    /// Champ conservé uniquement comme fallback de dev pour les scénarios de signature isolée.
    /// </para>
    /// </summary>
    public string? Pkcs11Pin { get; set; }

    /// <summary>
    /// Si <c>true</c>, l'API affiche un <b>dialog natif</b> sur le poste (osascript sur macOS,
    /// WinForms sur Windows) pour demander le PIN quand le header <c>X-Cps-Pin</c> est absent.
    /// Pratique pour tester via Swagger sur le poste du praticien, sans frontend.
    /// <para>
    /// En production avec un frontend, laisser <c>false</c> : le PIN vient du header
    /// <c>X-Cps-Pin</c> et l'API répond <c>401 CpsPinRequired</c> sinon (le frontend prompte).
    /// </para>
    /// </summary>
    public bool InteractivePinPrompt { get; set; }
}
