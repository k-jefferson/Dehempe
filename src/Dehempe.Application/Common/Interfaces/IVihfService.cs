namespace Dehempe.Application.Common.Interfaces;

/// <summary>
/// Construit le VIHF (Vecteur d'Identification et d'Habilitation)
/// requis dans l'en-tête WS-Security de chaque appel SOAP DMP.
/// Spec ANS : CI-SIS Volet DMP — section VIHF.
/// </summary>
public interface IVihfService
{
    /// <summary>
    /// Génère l'assertion SAML 2.0 VIHF signée avec le certificat CPS.
    /// </summary>
    Task<string> BuildVihfAssertionAsync(VihfContext context, CancellationToken ct = default);
}

public record VihfContext(
    /// <summary>Code rôle ANS du professionnel (nomenclature 1.2.250.1.71.1.2.7). Ex: "10" pour Médecin.</summary>
    string PractitionerRole,

    /// <summary>Libellé du rôle (displayName). Ex: "Médecin".</summary>
    string PractitionerRoleLabel,

    /// <summary>Identifiant RPPS ou ADELI du praticien.</summary>
    string PractitionerIdentifier,

    /// <summary>Nom du praticien (NomUsage).</summary>
    string PractitionerName,

    /// <summary>Structure d'exercice (OID de l'établissement).</summary>
    string OrganizationId,

    /// <summary>Nom de la structure d'exercice.</summary>
    string OrganizationName,

    /// <summary>Secteur d'activité au format ANS « SAXX^1.2.250.1.71.4.2.4 ».</summary>
    string OrganizationSector,

    /// <summary>INS du patient concerné par l'accès.</summary>
    string PatientIns,

    /// <summary>OID de l'INS (DMP utilise 1.2.250.1.213.1.4.10 pour le NIR).</summary>
    string PatientInsOid,

    /// <summary>Identifiant ANS du LPS. Ex: "01.01.01.01".</summary>
    string LpsId,

    /// <summary>Nom du LPS. Ex: "Déhempé".</summary>
    string LpsName,

    /// <summary>Version du LPS. Ex: "1.0".</summary>
    string LpsVersion,

    /// <summary>Numéro d'homologation DMP du LPS.</summary>
    string LpsAuthorizationId,

    /// <summary>Contexte d'accès : NORMAL, URGENCE, BRIS_DE_GLACE.</summary>
    AccessContext AccessContext = AccessContext.Normal
);

public enum AccessContext
{
    Normal,
    Urgence,
    BrisDeGlace
}
