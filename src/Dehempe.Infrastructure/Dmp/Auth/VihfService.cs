using System.Globalization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using Dehempe.Application.Common.Interfaces;
using Dehempe.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Dehempe.Infrastructure.Dmp.Auth;

/// <summary>
/// Construit et signe l'assertion SAML 2.0 VIHF v3.0 conformément à la spec ANS
/// (CI-SIS Volet DMP, section VIHF). L'assertion est ensuite injectée comme
/// header WS-Security dans chaque appel SOAP DMP.
///
/// Référence concrète : <c>docs/package dmp/DMP_LPS_Exemple de messages_1.0.1_1 -
/// v02.05.00/TD0.2 - Test d'existence_requête.xml</c>.
/// </summary>
internal sealed class VihfService : IVihfService
{
    private const string SamlNs = "urn:oasis:names:tc:SAML:2.0:assertion";
    private const string XsiNs  = "http://www.w3.org/2001/XMLSchema-instance";
    private const string XsdNs  = "http://www.w3.org/2001/XMLSchema";
    private const string Hl7Ns  = "urn:hl7-org:v3";

    // Code-system OIDs ANS
    private const string OidRoleNomenclature        = "1.2.250.1.71.1.2.7";  // G15 — professions de santé
    private const string OidSpecialityNomenclature  = "1.2.250.1.71.4.2.5";  // R01 — spécialités
    private const string OidPurposeOfUse            = "1.2.250.1.213.1.1.4.248";
    private const string OidIssuerFormatX509       = "urn:oasis:names:tc:SAML:1.1:nameid-format:X509SubjectName";
    private const string OidAuthnContextSmartcard  = "urn:oasis:names:tc:SAML:2.0:ac:classes:SmartcardPKI";

    private readonly ICpsAuthService _cpsAuth;
    private readonly ILogger<VihfService> _logger;

    public VihfService(ICpsAuthService cpsAuth, ILogger<VihfService> logger)
    {
        _cpsAuth = cpsAuth;
        _logger  = logger;
    }

    public async Task<string> BuildVihfAssertionAsync(VihfContext context, CancellationToken ct = default)
    {
        var cert = await _cpsAuth.GetCertificateAsync(ct);
        var key  = await _cpsAuth.GetSigningKeyAsync(ct);

        var assertionId = $"_vihf-{Guid.NewGuid():N}";
        var issueInstant = DateTimeOffset.UtcNow;
        var notOnOrAfter = issueInstant.AddHours(8);

        var doc = BuildAssertion(assertionId, issueInstant, notOnOrAfter, context, cert);

        try
        {
            SignAssertion(doc, assertionId, cert, key);
        }
        catch (CryptographicException ex)
        {
            throw new DmpAuthException($"Échec de la signature de l'assertion VIHF : {ex.Message}", ex);
        }

        _logger.LogDebug("VIHF généré pour le praticien {Id} ({Role})",
            context.PractitionerIdentifier, context.PractitionerRole);
        return doc.OuterXml;
    }

    // ── XML construction ──────────────────────────────────────────────────────

    private static XmlDocument BuildAssertion(
        string assertionId,
        DateTimeOffset issueInstant,
        DateTimeOffset notOnOrAfter,
        VihfContext ctx,
        X509Certificate2 cert)
    {
        var doc = new XmlDocument { PreserveWhitespace = false };

        var assertion = doc.CreateElement("saml2", "Assertion", SamlNs);
        doc.AppendChild(assertion);
        assertion.SetAttribute("xmlns:xsi", XsiNs);
        assertion.SetAttribute("ID", assertionId);
        assertion.SetAttribute("IssueInstant", Iso8601(issueInstant));
        assertion.SetAttribute("Version", "2.0");

        // ── Issuer ─────────────────────────────────────────────────────────────
        var issuer = doc.CreateElement("saml2", "Issuer", SamlNs);
        issuer.SetAttribute("Format", OidIssuerFormatX509);
        issuer.InnerText = cert.Subject;
        assertion.AppendChild(issuer);

        // ── Subject : RPPS du praticien ────────────────────────────────────────
        var subject = doc.CreateElement("saml2", "Subject", SamlNs);
        var nameId  = doc.CreateElement("saml2", "NameID", SamlNs);
        nameId.InnerText = ctx.PractitionerIdentifier;
        subject.AppendChild(nameId);
        assertion.AppendChild(subject);

        // ── AuthnStatement : exigée par l'ANS ──────────────────────────────────
        var authn = doc.CreateElement("saml2", "AuthnStatement", SamlNs);
        authn.SetAttribute("AuthnInstant", Iso8601(issueInstant));
        var ctxEl = doc.CreateElement("saml2", "AuthnContext", SamlNs);
        var ctxRef = doc.CreateElement("saml2", "AuthnContextClassRef", SamlNs);
        ctxRef.InnerText = OidAuthnContextSmartcard;
        ctxEl.AppendChild(ctxRef);
        authn.AppendChild(ctxEl);
        assertion.AppendChild(authn);

        // ── AttributeStatement : tous les champs VIHF v3.0 ─────────────────────
        var stmt = doc.CreateElement("saml2", "AttributeStatement", SamlNs);

        AddStringAttr(doc, stmt, "Identifiant_Structure",     ctx.OrganizationId);
        AddStringAttr(doc, stmt, "Secteur_Activite",          ctx.OrganizationSector);
        AddRoleAttr(  doc, stmt,                              ctx.PractitionerRole, ctx.PractitionerRoleLabel,
                                                              ctx.PractitionerSpecialityCode, ctx.PractitionerSpecialityLabel);
        AddStringAttr(doc, stmt, "VIHF_Version",              "3.0");
        AddStringAttr(doc, stmt, "Authentification_mode",     "DIRECTE");
        AddStringAttr(doc, stmt, "urn:oasis:names:tc:xacml:2.0:resource:resource-id",
                                                              $"{ctx.PatientIns}^^^&{ctx.PatientInsOid}&ISO^NH");
        AddStringAttr(doc, stmt, "Ressource_URN",             "urn:dmp");
        AddPurposeOfUseAttr(doc, stmt,                        ctx.AccessContext);
        AddStringAttr(doc, stmt, "LPS_ID",                    ctx.LpsId);
        AddStringAttr(doc, stmt, "LPS_Nom",                   ctx.LpsName);
        AddStringAttr(doc, stmt, "LPS_Version",               ctx.LpsVersion);
        AddStringAttr(doc, stmt, "LPS_ID_HOMOLOGATION_DMP",   ctx.LpsAuthorizationId);

        assertion.AppendChild(stmt);
        return doc;
    }

    // ── Helpers pour bâtir les attributs ──────────────────────────────────────

    private static void AddStringAttr(XmlDocument doc, XmlElement parent, string name, string value)
    {
        var attr = doc.CreateElement("saml2", "Attribute", SamlNs);
        attr.SetAttribute("Name", name);

        var val = doc.CreateElement("saml2", "AttributeValue", SamlNs);
        val.SetAttribute("xmlns:xs", XsdNs);
        val.SetAttribute("type", XsiNs, "xs:string");
        val.InnerText = value ?? string.Empty;

        attr.AppendChild(val);
        parent.AppendChild(attr);
    }

    private static void AddRoleAttr(
        XmlDocument doc, XmlElement parent,
        string code, string displayName,
        string? specialityCode, string? specialityLabel)
    {
        var attr = doc.CreateElement("saml2", "Attribute", SamlNs);
        attr.SetAttribute("Name", "urn:oasis:names:tc:xacml:2.0:subject:role");

        // Premier AttributeValue : profession (G15)
        var val1 = doc.CreateElement("saml2", "AttributeValue", SamlNs);
        var role1 = doc.CreateElement("Role", Hl7Ns);
        role1.SetAttribute("code",           code);
        role1.SetAttribute("codeSystem",     OidRoleNomenclature);
        role1.SetAttribute("codeSystemName", "G15");
        role1.SetAttribute("displayName",    displayName);
        role1.SetAttribute("type", XsiNs, "CE");
        val1.AppendChild(role1);
        attr.AppendChild(val1);

        // Second AttributeValue : spécialité (R01) — obligatoire pour Médecins et Pharmaciens
        if (!string.IsNullOrWhiteSpace(specialityCode))
        {
            var val2 = doc.CreateElement("saml2", "AttributeValue", SamlNs);
            var role2 = doc.CreateElement("Role", Hl7Ns);
            role2.SetAttribute("code",           specialityCode);
            role2.SetAttribute("codeSystem",     OidSpecialityNomenclature);
            role2.SetAttribute("codeSystemName", "R01");
            role2.SetAttribute("displayName",    specialityLabel ?? specialityCode);
            role2.SetAttribute("type", XsiNs, "CE");
            val2.AppendChild(role2);
            attr.AppendChild(val2);
        }

        parent.AppendChild(attr);
    }

    private static void AddPurposeOfUseAttr(XmlDocument doc, XmlElement parent, AccessContext access)
    {
        var (code, label) = access switch
        {
            AccessContext.Urgence     => ("urgence",      "Accès en urgence"),
            AccessContext.BrisDeGlace => ("brisDeGlace",  "Bris de glace"),
            _                         => ("normal",       "Accès normal"),
        };

        var attr = doc.CreateElement("saml2", "Attribute", SamlNs);
        attr.SetAttribute("Name", "urn:oasis:names:tc:xspa:1.0:subject:purposeofuse");

        var val = doc.CreateElement("saml2", "AttributeValue", SamlNs);
        var pou = doc.CreateElement("PurposeOfUse", Hl7Ns);
        pou.SetAttribute("code",           code);
        pou.SetAttribute("codeSystem",     OidPurposeOfUse);
        pou.SetAttribute("codeSystemName", "mode acces VIHF 2.0");
        pou.SetAttribute("displayName",    label);
        pou.SetAttribute("type", XsiNs, "CE");

        val.AppendChild(pou);
        attr.AppendChild(val);
        parent.AppendChild(attr);
    }

    private static string Iso8601(DateTimeOffset t)
        => t.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);

    // ── XML-Signature de l'assertion ──────────────────────────────────────────

    private static void SignAssertion(XmlDocument doc, string assertionId, X509Certificate2 cert, RSA signingKey)
    {
        var signedXml = new SignedXml(doc) { SigningKey = signingKey };
        signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;
        signedXml.SignedInfo.SignatureMethod        = SignedXml.XmlDsigRSASHA256Url;

        var reference = new Reference($"#{assertionId}");
        reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
        reference.AddTransform(new XmlDsigExcC14NTransform());
        reference.DigestMethod = SignedXml.XmlDsigSHA256Url;
        signedXml.AddReference(reference);

        var keyInfo = new KeyInfo();
        keyInfo.AddClause(new KeyInfoX509Data(cert));
        signedXml.KeyInfo = keyInfo;

        signedXml.ComputeSignature();

        var sigElement = signedXml.GetXml();
        doc.DocumentElement!.AppendChild(doc.ImportNode(sigElement, deep: true));
    }
}
