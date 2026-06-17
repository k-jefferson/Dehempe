using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using Dehempe.Application.Common.Interfaces;
using Dehempe.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dehempe.Infrastructure.Dmp.Soap;

/// <summary>
/// Base pour les clients SOAP XDS.b.
/// Gère l'envoi des enveloppes SOAP avec en-tête WS-Security (VIHF + certificat mTLS).
/// </summary>
internal abstract class XdsSoapClientBase
{
    private readonly HttpClient _http;
    private readonly IVihfService _vihf;
    private readonly ICpsAuthService _cpsAuth;
    protected readonly ILogger Logger;

    protected XdsSoapClientBase(
        HttpClient http,
        IVihfService vihf,
        ICpsAuthService cpsAuth,
        ILogger logger)
    {
        _http  = http;
        _vihf  = vihf;
        _cpsAuth = cpsAuth;
        Logger = logger;
    }

    protected async Task<XmlDocument> SendSoapAsync(
        string endpoint,
        string soapAction,
        XmlElement body,
        VihfContext vihfCtx,
        CancellationToken ct)
    {
        var vihfAssertion = await _vihf.BuildVihfAssertionAsync(vihfCtx, ct);
        var envelope = BuildEnvelope(soapAction, body, vihfAssertion);

        var content = new StringContent(envelope, Encoding.UTF8, "application/soap+xml");
        content.Headers.Add("SOAPAction", $"\"{soapAction}\"");

        Logger.LogDebug("Envoi SOAP {Action} vers {Endpoint}", soapAction, endpoint);

        HttpResponseMessage response;
        try
        {
            response = await _http.PostAsync(endpoint, content, ct);
        }
        catch (HttpRequestException ex) when (ex.InnerException is System.Security.Authentication.AuthenticationException)
        {
            var platformHint = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
                System.Runtime.InteropServices.OSPlatform.OSX)
                ? "Sur macOS, .NET ne peut pas attacher un cert PKCS#11 au handshake mTLS : " +
                  "AppleCertificatePal.CopyWithPrivateKey impose un export de clé privée, " +
                  "ce que le token CPS refuse par design. Solutions : déployer sur Windows/Linux, " +
                  "passer par un proxy mTLS local (stunnel + PKCS#11), ou configurer un .p12 de test."
                : "Vérifie que Cps:Pkcs11LibraryPath + Cps:Pkcs11Pin sont renseignés et que la carte est insérée.";

            throw new DmpAuthException(
                $"Échec du handshake TLS avec le DMP ({endpoint}). {platformHint}",
                ex);
        }
        catch (HttpRequestException ex)
        {
            throw new DmpException(
                $"Erreur réseau vers le DMP ({endpoint}) : {ex.Message}", ex, "NETWORK_ERROR");
        }

        using (response)
        {
            var responseXml = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
                throw new DmpException(
                    $"Erreur HTTP {(int)response.StatusCode} lors de l'appel DMP ({soapAction}). Body: {responseXml}",
                    response.StatusCode.ToString());

            var doc = new XmlDocument();
            doc.LoadXml(responseXml);
            CheckRegistryError(doc);
            return doc;
        }
    }

    private static string BuildEnvelope(string soapAction, XmlElement body, string vihfAssertion)
    {
        return $@"<s:Envelope xmlns:s=""{XdsConstants.SoapNs}"" xmlns:wsa=""{XdsConstants.WsaNs}"">
  <s:Header>
    <wsa:Action s:mustUnderstand=""1"">{soapAction}</wsa:Action>
    <wsse:Security xmlns:wsse=""{XdsConstants.WssNs}"" s:mustUnderstand=""1"">
      {vihfAssertion}
    </wsse:Security>
  </s:Header>
  <s:Body>
    {body.OuterXml}
  </s:Body>
</s:Envelope>";
    }

    private static void CheckRegistryError(XmlDocument doc)
    {
        var ns = new XmlNamespaceManager(doc.NameTable);
        ns.AddNamespace("rs", XdsConstants.EbRs3Ns);
        ns.AddNamespace("s", XdsConstants.SoapNs);

        var fault = doc.SelectSingleNode("//s:Body/s:Fault", ns);
        if (fault is not null)
        {
            var code  = fault.SelectSingleNode("s:Code/s:Value", ns)?.InnerText ?? "Unknown";
            var reason = fault.SelectSingleNode("s:Reason/s:Text", ns)?.InnerText ?? string.Empty;
            throw new DmpException($"SOAP Fault [{code}]: {reason}", code);
        }

        var status = doc.SelectSingleNode("//rs:RegistryResponse/@status", ns)?.Value;
        if (status is not null && !status.Contains("Success", StringComparison.OrdinalIgnoreCase))
        {
            var errorCode = doc.SelectSingleNode("//rs:RegistryError/@errorCode", ns)?.Value;
            var errorMsg  = doc.SelectSingleNode("//rs:RegistryError/@codeContext", ns)?.Value
                         ?? "Erreur DMP inconnue.";
            throw new DmpException(errorMsg, errorCode);
        }
    }

    protected static XmlElement CreateElement(XmlDocument doc, string ns, string localName)
        => doc.CreateElement(localName, ns);

    protected static XmlElement CreateElement(XmlDocument doc, string prefix, string localName, string ns)
        => doc.CreateElement(prefix, localName, ns);
}
