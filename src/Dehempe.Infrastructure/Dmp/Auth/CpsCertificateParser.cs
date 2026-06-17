using System.Security.Cryptography.X509Certificates;

namespace Dehempe.Infrastructure.Dmp.Auth;

/// <summary>
/// Extrait l'identité du praticien depuis le Subject DN d'un certificat CPS3.
///
/// Format Subject CPS3 (ANS / CI-SIS) :
///   CN=DUPONT PIERRE, SERIALNUMBER=8001234567, OU=MEDECIN, O=CABINET DR DUPONT, C=FR
///
/// RPPS = 11 chiffres  |  ADELI = 9 chiffres
/// </summary>
public sealed class CpsPractitionerIdentity
{
    public string Identifier { get; init; } = string.Empty;  // RPPS ou ADELI
    public string Name       { get; init; } = string.Empty;  // "DUPONT PIERRE"
    public string RoleCode   { get; init; } = string.Empty;  // "MEDECIN"
    public string OrgName    { get; init; } = string.Empty;  // nom de la structure
}

internal static class CpsCertificateParser
{
    // OIDs standard X.500
    private const string OidCn           = "2.5.4.3";
    private const string OidSerialNumber = "2.5.4.5";
    private const string OidOrg          = "2.5.4.10";
    private const string OidOu           = "2.5.4.11";

    public static CpsPractitionerIdentity Parse(X509Certificate2 cert)
    {
        var rdns = cert.SubjectName
            .EnumerateRelativeDistinguishedNames()
            .ToList();

        var cn     = GetValue(rdns, OidCn);
        var serial = GetValue(rdns, OidSerialNumber);
        var org    = GetValue(rdns, OidOrg);
        var ous    = rdns
            .Where(r => r.GetSingleElementType().Value == OidOu)
            .Select(r => r.GetSingleElementValue() ?? string.Empty)
            .ToList();

        // RPPS en priorité depuis SERIALNUMBER, sinon cherche dans CN ou OU
        var identifier = ExtractIdentifier(serial, cn, ous);

        // Le rôle est le premier OU qui ressemble à un code de rôle (lettres uniquement)
        var role = ous.FirstOrDefault(IsRoleCode) ?? string.Empty;

        return new CpsPractitionerIdentity
        {
            Identifier = identifier,
            Name       = FormatName(cn),
            RoleCode   = role,
            OrgName    = org ?? string.Empty
        };
    }

    private static string GetValue(
        IEnumerable<X500RelativeDistinguishedName> rdns, string oid)
        => rdns
            .FirstOrDefault(r => r.GetSingleElementType().Value == oid)
            ?.GetSingleElementValue()
           ?? string.Empty;

    private static string ExtractIdentifier(string serial, string cn, IList<string> ous)
    {
        // SERIALNUMBER = "8001234567" ou "RPPS|8001234567"
        if (!string.IsNullOrWhiteSpace(serial))
        {
            var digits = ExtractDigits(serial);
            if (digits.Length is 11 or 9) return digits;
        }

        // Certains émetteurs mettent le RPPS dans le CN après le nom : "DUPONT PIERRE 8001234567"
        var cnDigits = ExtractDigits(cn);
        if (cnDigits.Length is 11 or 9) return cnDigits;

        // Certains émetteurs mettent l'identifiant dans un OU numérique
        foreach (var ou in ous)
        {
            var ouDigits = ExtractDigits(ou);
            if (ouDigits.Length is 11 or 9) return ouDigits;
        }

        return serial; // Retour brut si rien ne correspond
    }

    private static string ExtractDigits(string value)
        => new(value.Where(char.IsDigit).ToArray());

    // Un code rôle CPS3 est composé de lettres (ex: MEDECIN, PHARMACIEN, INFIRMIER)
    private static bool IsRoleCode(string ou)
        => !string.IsNullOrWhiteSpace(ou) && ou.All(c => char.IsLetter(c) || c == ' ');

    // Normalise "DUPONT PIERRE" en "DUPONT PIERRE" (déjà en majuscules dans les certs CPS)
    private static string FormatName(string cn)
    {
        // Retire un éventuel identifiant numérique en fin de CN
        var parts = cn.Split(' ');
        return string.Join(' ', parts.Where(p => !p.All(char.IsDigit))).Trim();
    }
}
