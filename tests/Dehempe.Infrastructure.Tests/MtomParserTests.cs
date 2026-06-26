using System.Net.Http.Headers;
using System.Text;
using Dehempe.Infrastructure.Dmp.Soap;
using Xunit;

namespace Dehempe.Infrastructure.Tests;

/// <summary>
/// Vérifie le parsing MTOM/XOP des réponses ITI-43 du DMP (cf. XdsRepositoryClient).
/// Le format reproduit celui émis par le DMP (Apache CXF) : boundary, paramètre <c>start</c>,
/// href XOP URL-encodé (<c>%40</c>), pièce jointe binaire (Content-Transfer-Encoding: binary).
/// </summary>
public class MtomParserTests
{
    private const string Boundary = "uuid:be23ca67-74fc-412d-b7ba-1d3d52ee6dc1";
    private const string RootCid = "root.message@cxf.apache.org";
    private const string DocCid = "b857abb7-a2c3-4db8-bd69-d5ff76826e72-2@urn:ihe:iti:xds-b:2007";

    /// <summary>Le href XOP encode le '@' en %40 et ':' en %3A — la résolution doit les décoder.</summary>
    private const string DocHrefEncoded =
        "cid:b857abb7-a2c3-4db8-bd69-d5ff76826e72-2@urn%3Aihe%3Aiti%3Axds-b%3A2007";

    private static MediaTypeHeaderValue MultipartContentType() =>
        MediaTypeHeaderValue.Parse(
            $"multipart/related; type=\"application/xop+xml\"; boundary=\"{Boundary}\"; " +
            $"start=\"<{RootCid}>\"; start-info=\"application/soap+xml\"");

    private static byte[] BuildMtom(string soapEnvelope, byte[] documentBytes)
    {
        // Construit un multipart/related au niveau octet (les sauts de ligne MIME sont des CRLF).
        const string crlf = "\r\n";
        var prefix = new StringBuilder()
            .Append("--").Append(Boundary).Append(crlf)
            .Append("Content-Type: application/xop+xml; charset=UTF-8; type=\"application/soap+xml\"").Append(crlf)
            .Append("Content-Transfer-Encoding: binary").Append(crlf)
            .Append("Content-ID: <").Append(RootCid).Append('>').Append(crlf)
            .Append(crlf)
            .Append(soapEnvelope).Append(crlf)
            .Append("--").Append(Boundary).Append(crlf)
            .Append("Content-Type: application/octet-stream").Append(crlf)
            .Append("Content-Transfer-Encoding: binary").Append(crlf)
            .Append("Content-ID: <").Append(DocCid).Append('>').Append(crlf)
            .Append(crlf)
            .ToString();
        var suffix = crlf + "--" + Boundary + "--" + crlf;

        var ms = new MemoryStream();
        ms.Write(Encoding.UTF8.GetBytes(prefix));
        ms.Write(documentBytes);
        ms.Write(Encoding.UTF8.GetBytes(suffix));
        return ms.ToArray();
    }

    [Fact]
    public void Parse_extrait_l_enveloppe_racine_et_la_piece_jointe()
    {
        var soap =
            "<soap:Envelope xmlns:soap=\"http://www.w3.org/2003/05/soap-envelope\">" +
            "<soap:Body><ns4:RetrieveDocumentSetResponse xmlns:ns4=\"urn:ihe:iti:xds-b:2007\">" +
            "<ns4:DocumentResponse><ns4:mimeType>text/xml</ns4:mimeType>" +
            "<ns4:Document><xop:Include href=\"" + DocHrefEncoded +
            "\" xmlns:xop=\"http://www.w3.org/2004/08/xop/include\"/></ns4:Document>" +
            "</ns4:DocumentResponse></ns4:RetrieveDocumentSetResponse></soap:Body></soap:Envelope>";
        var docContent = Encoding.UTF8.GetBytes("<ClinicalDocument>contenu CDA</ClinicalDocument>");

        var result = MtomParser.Parse(BuildMtom(soap, docContent), MultipartContentType());

        Assert.Contains("RetrieveDocumentSetResponse", result.RootXml);
        Assert.Single(result.Attachments);

        // Le cid résolu depuis le href (URL-décodé) doit pointer sur la pièce jointe.
        var cid = MtomParser.NormalizeCid(DocHrefEncoded);
        Assert.True(result.Attachments.ContainsKey(cid));
        Assert.Equal(docContent, result.Attachments[cid]);
    }

    [Fact]
    public void Parse_preserve_les_octets_binaires_sans_corruption()
    {
        // Octets non-UTF-8 valides (ex. en-tête PDF + octets hauts) : ne doivent PAS être altérés.
        var binary = new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D, 0x31, 0x2E, 0x34, 0x00, 0xFF, 0xFE, 0x80, 0x0A };
        var soap =
            "<soap:Envelope xmlns:soap=\"http://www.w3.org/2003/05/soap-envelope\">" +
            "<soap:Body><Document><xop:Include href=\"" + DocHrefEncoded +
            "\" xmlns:xop=\"http://www.w3.org/2004/08/xop/include\"/></Document></soap:Body></soap:Envelope>";

        var result = MtomParser.Parse(BuildMtom(soap, binary), MultipartContentType());

        var cid = MtomParser.NormalizeCid(DocHrefEncoded);
        Assert.Equal(binary, result.Attachments[cid]);
    }

    [Theory]
    [InlineData("cid:doc%40host", "doc@host")]
    [InlineData("<doc@host>", "doc@host")]
    [InlineData("doc@host", "doc@host")]
    [InlineData("cid:b857%3Aabc%40urn", "b857:abc@urn")]
    public void NormalizeCid_uniformise_les_formes_href_et_content_id(string raw, string expected)
    {
        Assert.Equal(expected, MtomParser.NormalizeCid(raw));
    }
}
