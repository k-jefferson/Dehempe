using System.Formats.Asn1;
using System.Security.Cryptography.X509Certificates;

namespace Dehempe.Infrastructure.Dmp.Auth;

/// <summary>
/// Identité du praticien extraite du certificat CPS.
/// </summary>
public sealed class CpsPractitionerIdentity
{
    public string Identifier { get; init; } = string.Empty;  // RPPS (12 chiffres) ou ADELI (9)
    public string Name       { get; init; } = string.Empty;  // "DOC0073574 KIT"
    public string RoleCode   { get; init; } = string.Empty;  // "10" (Médecin) ou "MEDECIN" selon format
    public string RoleLabel  { get; init; } = string.Empty;  // "Médecin" — displayName HL7
    public string OrgName    { get; init; } = string.Empty;  // nom de la structure (si présent dans le cert)
}

/// <summary>
/// Extrait l'identité du praticien depuis le Subject DN d'un certificat CPS.
///
/// Deux formats CPS3 cohabitent en production :
///
/// <list type="bullet">
/// <item>
///   Classique (anciens certs) :
///   <c>CN=DUPONT PIERRE, SERIALNUMBER=8001234567, OU=MEDECIN, O=CABINET DR DUPONT, C=FR</c>
/// </item>
/// <item>
///   Récent / multi-AVA :
///   <c>C=FR/title=Médecin/GN=KIT/SN=DOC0073574/CN=899700735741</c>
/// </item>
/// </list>
///
/// Le parser lit le DN directement en DER via <see cref="AsnReader"/> pour gérer
/// les RDN multi-AVA que <see cref="X500RelativeDistinguishedName.GetSingleElementType"/> refuse.
/// </summary>
internal static class CpsCertificateParser
{
    private const string OidCn           = "2.5.4.3";   // CN
    private const string OidSurname      = "2.5.4.4";   // SN
    private const string OidSerialNumber = "2.5.4.5";   // SERIALNUMBER (ancien format)
    private const string OidOrg          = "2.5.4.10";  // O
    private const string OidOu           = "2.5.4.11";  // OU
    private const string OidTitle        = "2.5.4.12";  // title (nouveau format)
    private const string OidGivenName    = "2.5.4.42";  // GN

    // Mapping libellé → code rôle ANS (nomenclature 1.2.250.1.71.1.2.7)
    private static readonly Dictionary<string, string> ProfessionToRoleCode =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Médecin"]                  = "10",
            ["Medecin"]                  = "10",
            ["MEDECIN"]                  = "10",
            ["Pharmacien"]               = "21",
            ["PHARMACIEN"]               = "21",
            ["Chirurgien-dentiste"]      = "26",
            ["CHIRURGIEN-DENTISTE"]      = "26",
            ["Sage-femme"]               = "28",
            ["SAGE-FEMME"]               = "28",
            ["Infirmier"]                = "40",
            ["Infirmière"]               = "40",
            ["INFIRMIER"]                = "40",
            ["Masseur-kinésithérapeute"] = "50",
            ["MASSEUR-KINESITHERAPEUTE"] = "50",
            ["Orthoptiste"]              = "69",
            ["Orthophoniste"]            = "70",
            ["Biologiste"]               = "91",
            ["BIOLOGISTE"]                = "91",
        };

    public static CpsPractitionerIdentity Parse(X509Certificate2 cert)
    {
        var attrs = ReadSubject(cert);

        var cn        = attrs.GetValueOrDefault(OidCn,           string.Empty);
        var sn        = attrs.GetValueOrDefault(OidSurname,      string.Empty);
        var gn        = attrs.GetValueOrDefault(OidGivenName,    string.Empty);
        var serial    = attrs.GetValueOrDefault(OidSerialNumber, string.Empty);
        var orgName   = attrs.GetValueOrDefault(OidOrg,          string.Empty);
        var title     = attrs.GetValueOrDefault(OidTitle,        string.Empty);
        var ouRole    = attrs.GetValueOrDefault(OidOu,           string.Empty);

        return new CpsPractitionerIdentity
        {
            Identifier = ResolveIdentifier(cn, serial, ouRole),
            Name       = ResolveName(sn, gn, cn),
            RoleCode   = ResolveRoleCode(title, ouRole),
            RoleLabel  = ResolveRoleLabel(title, ouRole),
            OrgName    = orgName
        };
    }

    private static string ResolveRoleLabel(string title, string ouRole)
    {
        if (!string.IsNullOrWhiteSpace(title)) return title;
        if (!string.IsNullOrWhiteSpace(ouRole) && ouRole.All(c => char.IsLetter(c) || c == ' ' || c == '-'))
            return ouRole;
        return string.Empty;
    }

    // ─── Walk Subject DN as DER ──────────────────────────────────────────────

    private static Dictionary<string, string> ReadSubject(X509Certificate2 cert)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        try
        {
            var reader = new AsnReader(cert.SubjectName.RawData, AsnEncodingRules.DER);
            var seq    = reader.ReadSequence();
            while (seq.HasData)
            {
                var set = seq.ReadSetOf();
                while (set.HasData)
                {
                    var ava   = set.ReadSequence();
                    var oid   = ava.ReadObjectIdentifier();
                    var value = ReadAsnString(ava);
                    if (!result.ContainsKey(oid)) result[oid] = value;
                }
            }
        }
        catch { /* return what we got */ }
        return result;
    }

    private static string ReadAsnString(AsnReader reader)
    {
        var tag = reader.PeekTag();
        var universal = tag.TagValue switch
        {
            (int)UniversalTagNumber.UTF8String      => UniversalTagNumber.UTF8String,
            (int)UniversalTagNumber.PrintableString => UniversalTagNumber.PrintableString,
            (int)UniversalTagNumber.IA5String       => UniversalTagNumber.IA5String,
            (int)UniversalTagNumber.BMPString       => UniversalTagNumber.BMPString,
            (int)UniversalTagNumber.T61String       => UniversalTagNumber.T61String,
            _                                       => UniversalTagNumber.UTF8String,
        };
        return reader.ReadCharacterString(universal);
    }

    // ─── Field resolution (handles both DN formats) ──────────────────────────

    private static string ResolveIdentifier(string cn, string serial, string ou)
    {
        // Nouveau format : CN = RPPS (12 chiffres)
        var cnDigits = ExtractDigits(cn);
        if (cnDigits.Length is 12 or 11 or 9) return cnDigits;

        // Ancien format : SERIALNUMBER = "RPPS|123..." ou juste les chiffres
        var serialDigits = ExtractDigits(serial);
        if (serialDigits.Length is 11 or 9) return serialDigits;

        var ouDigits = ExtractDigits(ou);
        if (ouDigits.Length is 11 or 9) return ouDigits;

        return string.IsNullOrEmpty(serial) ? cn : serial;
    }

    private static string ResolveName(string sn, string gn, string cn)
    {
        if (!string.IsNullOrWhiteSpace(sn) || !string.IsNullOrWhiteSpace(gn))
            return $"{sn} {gn}".Trim();

        // Ancien format : tout dans CN
        var parts = cn.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return string.Join(' ', parts.Where(p => !p.All(char.IsDigit))).Trim();
    }

    private static string ResolveRoleCode(string title, string ouRole)
    {
        if (!string.IsNullOrWhiteSpace(title))
            return ProfessionToRoleCode.TryGetValue(title, out var code) ? code : title;

        if (!string.IsNullOrWhiteSpace(ouRole) && ouRole.All(c => char.IsLetter(c) || c == ' ' || c == '-'))
            return ProfessionToRoleCode.TryGetValue(ouRole, out var code) ? code : ouRole;

        return string.Empty;
    }

    private static string ExtractDigits(string value)
        => string.IsNullOrEmpty(value) ? string.Empty : new(value.Where(char.IsDigit).ToArray());
}
