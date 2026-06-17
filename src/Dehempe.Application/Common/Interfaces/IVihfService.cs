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
    /// <summary>Rôle RPPS du professionnel de santé.</summary>
    string PractitionerRole,

    /// <summary>Identifiant RPPS ou ADELI du praticien.</summary>
    string PractitionerIdentifier,

    /// <summary>Nom du praticien (NomUsage).</summary>
    string PractitionerName,

    /// <summary>Structure d'exercice (OID de l'établissement).</summary>
    string OrganizationId,

    /// <summary>Nom de la structure d'exercice.</summary>
    string OrganizationName,

    /// <summary>INS du patient concerné par l'accès.</summary>
    string PatientIns,

    /// <summary>OID de l'INS (NIR ou NIA).</summary>
    string PatientInsOid,

    /// <summary>Contexte d'accès : NORMAL, URGENCE, BRIS_DE_GLACE.</summary>
    AccessContext AccessContext = AccessContext.Normal
);

public enum AccessContext
{
    Normal,
    Urgence,
    BrisDeGlace
}
