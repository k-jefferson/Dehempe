using System.Net.Http.Headers;
using System.Text;
using Dehempe.Domain.Exceptions;

namespace Dehempe.Infrastructure.Dmp.Soap;

/// <summary>
/// Parseur de réponses MTOM/XOP (<c>multipart/related</c>) renvoyées par le DMP pour l'ITI-43.
///
/// Une réponse MTOM contient une partie racine (l'enveloppe SOAP en <c>application/xop+xml</c>)
/// et une ou plusieurs pièces jointes binaires (le contenu des documents), référencées depuis
/// le SOAP par des éléments <c>&lt;xop:Include href="cid:..."/&gt;</c>.
///
/// Le parsing travaille au **niveau octet** : les pièces jointes (<c>Content-Transfer-Encoding:
/// binary</c>) ne doivent pas être décodées en texte sous peine de corruption.
/// </summary>
internal static class MtomParser
{
    /// <summary>Résultat : enveloppe SOAP racine (texte) + pièces jointes par Content-ID normalisé.</summary>
    public sealed record Result(string RootXml, IReadOnlyDictionary<string, byte[]> Attachments);

    private static readonly byte[] CrlfCrlf = { 13, 10, 13, 10 };

    /// <summary>
    /// Parse un corps <c>multipart/related</c> à partir des octets bruts et de l'en-tête Content-Type
    /// (qui porte <c>boundary</c> et éventuellement <c>start</c> = Content-ID de la partie racine).
    /// </summary>
    public static Result Parse(byte[] data, MediaTypeHeaderValue contentType)
    {
        string? boundary = null, startCid = null;
        foreach (var p in contentType.Parameters)
        {
            var val = p.Value?.Trim('"');
            if (p.Name.Equals("boundary", StringComparison.OrdinalIgnoreCase)) boundary = val;
            else if (p.Name.Equals("start", StringComparison.OrdinalIgnoreCase)) startCid = val;
        }
        if (string.IsNullOrEmpty(boundary))
            throw new DmpException("Réponse MTOM du DMP sans 'boundary'.", "MTOM_PARSE");

        var delimiter = Encoding.ASCII.GetBytes("--" + boundary);
        var positions = FindAll(data, delimiter);

        var attachments = new Dictionary<string, byte[]>(StringComparer.Ordinal);
        var rootWanted  = startCid is null ? null : NormalizeCid(startCid);
        string? rootXml = null;
        string? firstXmlFallback = null;

        for (var i = 0; i + 1 < positions.Count; i++)
        {
            var partStart = positions[i] + delimiter.Length;
            var partEnd   = positions[i + 1];
            if (partEnd <= partStart) continue;

            // Sépare en-têtes MIME / corps sur le premier CRLFCRLF.
            var sep = IndexOf(data, CrlfCrlf, partStart, partEnd);
            if (sep < 0) continue;

            var headers   = Encoding.ASCII.GetString(data, partStart, sep - partStart);
            var bodyStart = sep + CrlfCrlf.Length;
            var bodyEnd   = partEnd;
            // Retire le CRLF qui précède le délimiteur suivant.
            if (bodyEnd - 2 >= bodyStart && data[bodyEnd - 2] == 13 && data[bodyEnd - 1] == 10)
                bodyEnd -= 2;
            if (bodyEnd < bodyStart) continue;

            string? cid = null, partContentType = null;
            foreach (var line in headers.Split('\n'))
            {
                var h = line.Trim();
                if (h.StartsWith("Content-ID:", StringComparison.OrdinalIgnoreCase))
                    cid = h["Content-ID:".Length..].Trim();
                else if (h.StartsWith("Content-Type:", StringComparison.OrdinalIgnoreCase))
                    partContentType = h["Content-Type:".Length..].Trim();
            }

            var body    = new byte[bodyEnd - bodyStart];
            Array.Copy(data, bodyStart, body, 0, body.Length);
            var cidNorm = cid is null ? null : NormalizeCid(cid);

            var isXmlPart = partContentType is not null
                            && (partContentType.Contains("xop+xml", StringComparison.OrdinalIgnoreCase)
                                || partContentType.Contains("soap", StringComparison.OrdinalIgnoreCase)
                                || partContentType.Contains("/xml", StringComparison.OrdinalIgnoreCase));

            var isRoot = rootWanted is not null ? cidNorm == rootWanted : isXmlPart;

            if (isRoot && rootXml is null)
                rootXml = DecodeText(body);
            else if (cidNorm is not null)
                attachments[cidNorm] = body;

            if (firstXmlFallback is null && isXmlPart)
                firstXmlFallback = DecodeText(body);
        }

        rootXml ??= firstXmlFallback
            ?? throw new DmpException("Partie SOAP introuvable dans la réponse MTOM du DMP.", "MTOM_PARSE");

        return new Result(rootXml, attachments);
    }

    /// <summary>
    /// Normalise un Content-ID / référence <c>cid:</c> pour comparaison : retire le préfixe
    /// <c>cid:</c>, les chevrons <c>&lt; &gt;</c>, et URL-décode (le href XOP encode le <c>@</c>
    /// en <c>%40</c> alors que l'en-tête Content-ID ne l'encode pas).
    /// </summary>
    public static string NormalizeCid(string raw)
    {
        var s = raw.Trim();
        if (s.StartsWith("cid:", StringComparison.OrdinalIgnoreCase)) s = s[4..];
        s = s.Trim();
        if (s.StartsWith('<')) s = s[1..];
        if (s.EndsWith('>'))   s = s[..^1];
        try { s = Uri.UnescapeDataString(s); } catch { /* garde la valeur brute */ }
        return s.Trim();
    }

    /// <summary>Décode des octets en texte UTF-8 en retirant un éventuel BOM.</summary>
    private static string DecodeText(byte[] bytes)
    {
        if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
            return Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);
        return Encoding.UTF8.GetString(bytes);
    }

    /// <summary>Toutes les positions de <paramref name="pattern"/> dans <paramref name="data"/>.</summary>
    private static List<int> FindAll(byte[] data, byte[] pattern)
    {
        var positions = new List<int>();
        var idx = 0;
        while ((idx = IndexOf(data, pattern, idx, data.Length)) >= 0)
        {
            positions.Add(idx);
            idx += pattern.Length;
        }
        return positions;
    }

    /// <summary>Index de <paramref name="pattern"/> dans <c>data[start..end)</c>, ou -1.</summary>
    private static int IndexOf(byte[] data, byte[] pattern, int start, int end)
    {
        var last = Math.Min(end, data.Length) - pattern.Length;
        for (var i = start; i <= last; i++)
        {
            var match = true;
            for (var j = 0; j < pattern.Length; j++)
            {
                if (data[i + j] != pattern[j]) { match = false; break; }
            }
            if (match) return i;
        }
        return -1;
    }
}
