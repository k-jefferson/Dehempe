using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Security;
using System.Text;
using System.Xml;
using Dehempe.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Dehempe.Infrastructure.Dmp.Auth;

/// <summary>
/// Construit et signe l'assertion SAML 2.0 VIHF conformément au CI-SIS de l'ANS.
/// Référence : ANS — Cadre d'Interopérabilité des Systèmes d'Information de Santé,
/// Volet DMP — Section VIHF v2.x.
/// </summary>
internal sealed class VihfService : IVihfService
{
    private readonly ICpsAuthService _cpsAuth;
    private readonly ILogger<VihfService> _logger;

    private const string SamlAssertionNs = "urn:oasis:names:tc:SAML:2.0:assertion";
    private const string XsiNs = "http://www.w3.org/2001/XMLSchema-instance";

    public VihfService(ICpsAuthService cpsAuth, ILogger<VihfService> logger)
    {
        _cpsAuth = cpsAuth;
        _logger = logger;
    }

    public async Task<string> BuildVihfAssertionAsync(VihfContext context, CancellationToken ct = default)
    {
        var cert = await _cpsAuth.GetCertificateAsync(ct);
        var key  = await _cpsAuth.GetSigningKeyAsync(ct);
        var assertionId = $"_vihf-{Guid.NewGuid():N}";
        var now = DateTimeOffset.UtcNow;
        var notOnOrAfter = now.AddHours(8);

        var doc = BuildXmlAssertion(assertionId, now, notOnOrAfter, context, cert);

        try
        {
            SignAssertion(doc, assertionId, cert, key);
        }
        catch (System.Security.Cryptography.CryptographicException ex)
        {
            throw new Dehempe.Domain.Exceptions.DmpAuthException(
                $"Échec de la signature de l'assertion VIHF : {ex.Message}", ex);
        }

        _logger.LogDebug("VIHF généré pour le praticien {Id}", context.PractitionerIdentifier);
        return doc.OuterXml;
    }

    private static XmlDocument BuildXmlAssertion(
        string assertionId,
        DateTimeOffset issueInstant,
        DateTimeOffset notOnOrAfter,
        VihfContext ctx,
        X509Certificate2 cert)
    {
        var sb = new StringBuilder();
        sb.Append($@"<saml:Assertion
            xmlns:saml=""{SamlAssertionNs}""
            xmlns:xsi=""{XsiNs}""
            ID=""{assertionId}""
            IssueInstant=""{issueInstant:yyyy-MM-ddTHH:mm:ssZ}""
            Version=""2.0"">
          <saml:Issuer Format=""urn:oasis:names:tc:SAML:1.1:nameid-format:X509SubjectName"">{cert.Subject}</saml:Issuer>
          <saml:Subject>
            <saml:NameID Format=""urn:oasis:names:tc:SAML:1.1:nameid-format:X509SubjectName"">{ctx.PractitionerIdentifier}</saml:NameID>
          </saml:Subject>
          <saml:Conditions NotBefore=""{issueInstant:yyyy-MM-ddTHH:mm:ssZ}"" NotOnOrAfter=""{notOnOrAfter:yyyy-MM-ddTHH:mm:ssZ}"">
          </saml:Conditions>
          <saml:AttributeStatement>
            {BuildAttribute("urn:oasis:names:tc:xspa:1.0:subject:subject-id", ctx.PractitionerName)}
            {BuildAttribute("urn:oasis:names:tc:xspa:1.0:subject:organization", ctx.OrganizationName)}
            {BuildAttribute("urn:oasis:names:tc:xspa:1.0:subject:organization-id", ctx.OrganizationId)}
            {BuildAttribute("urn:oasis:names:tc:xspa:1.0:subject:hl7:permission", ctx.PractitionerRole)}
            {BuildAttribute("urn:oasis:names:tc:xacml:2.0:subject:role", ctx.PractitionerRole)}
            {BuildAttribute("urn:oasis:names:tc:xspa:1.0:subject:purposeofuse", ctx.AccessContext.ToString().ToUpper())}
            {BuildAttribute("urn:oasis:names:tc:xacml:2.0:resource:resource-id",
                $"{ctx.PatientIns}^^^&{ctx.PatientInsOid}&ISO")}
          </saml:AttributeStatement>
        </saml:Assertion>");

        var doc = new XmlDocument { PreserveWhitespace = false };
        doc.LoadXml(sb.ToString());
        return doc;
    }

    private static string BuildAttribute(string name, string value)
        => $@"<saml:Attribute Name=""{name}"">
               <saml:AttributeValue>{SecurityElement.Escape(value)}</saml:AttributeValue>
             </saml:Attribute>";

    private static void SignAssertion(XmlDocument doc, string assertionId, X509Certificate2 cert, System.Security.Cryptography.RSA signingKey)
    {
        var signedXml = new SignedXml(doc)
        {
            SigningKey = signingKey
        };

        signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;
        signedXml.SignedInfo.SignatureMethod = SignedXml.XmlDsigRSASHA256Url;

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
