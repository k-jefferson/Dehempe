using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Dehempe.Application.Common.Interfaces;
using Dehempe.Application.Cps.DTOs;
using Dehempe.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using PCSC;
using PCSC.Exceptions;

namespace Dehempe.Infrastructure.Dmp.Card;

/// <summary>
/// Reads CPS card data from the personal certificate published by the CPS
/// middleware (CryptoTokenKit on macOS, standard cert store on Windows).
/// </summary>
internal sealed class CpsCardReaderService : ICpsCardReaderService
{
    // X.500 OIDs used by CPS3 personal certs
    private const string OidCommonName = "2.5.4.3";   // CN  = RPPS (12 digits)
    private const string OidSurname    = "2.5.4.4";   // SN  = Nom
    private const string OidGivenName  = "2.5.4.42";  // GN  = Prénom
    private const string OidTitle      = "2.5.4.12";  // title = Profession

    private readonly ILogger<CpsCardReaderService> _logger;

    public CpsCardReaderService(ILogger<CpsCardReaderService> logger) => _logger = logger;

    public async Task<CpsCardDto> ReadCardAsync(CancellationToken ct = default)
    {
        await Task.Run(EnsureReaderHasCard, ct);

        ICpsCertificateProvider provider = RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
            ? new MacOsCtkTokenCertificateProvider(_logger)
            : new KeychainCertificateProvider(_logger);

        var found = await Task.Run(provider.FindCpsCertificates, ct);
        if (found.Count == 0)
            throw new DmpException(
                "Aucun certificat CPS trouvé. Vérifiez que le middleware CPS est lancé et que la carte est insérée.",
                "CPS_CERT_NOT_FOUND");

        var personal = found
            .Where(x => IsPersonalCpsCertificate(x.Cert))
            .OrderByDescending(x => x.Cert.NotAfter)
            .ToList();

        if (personal.Count == 0)
        {
            _logger.LogWarning(
                "Aucun certificat personnel CPS dans les {N} candidats — utilisation du premier disponible.",
                found.Count);
            personal = found.OrderByDescending(x => x.Cert.NotAfter).ToList();
        }

        var (cert, cardId) = personal[0];
        return Map(cert, cardId);
    }

    // ── Reader presence check ──────────────────────────────────────────────

    private void EnsureReaderHasCard()
    {
        try
        {
            using var ctx = ContextFactory.Instance.Establish(SCardScope.System);
            var readers   = ctx.GetReaders();

            if (readers.Length == 0)
                throw new DmpException("Aucun lecteur de carte PC/SC détecté.", "NO_READER");

            foreach (var reader in readers)
            {
                try
                {
                    using var card = ctx.ConnectReader(reader, SCardShareMode.Shared, SCardProtocol.Any);
                    _logger.LogInformation("Carte détectée dans « {Reader} ».", reader);
                    return;
                }
                catch (NoSmartcardException) { /* no card */ }
            }

            throw new DmpException(
                "Aucune carte CPS insérée dans les lecteurs PC/SC disponibles.",
                "NO_CARD");
        }
        catch (PCSCException ex)
        {
            throw new DmpException($"Erreur PC/SC : {ex.Message} (code {ex.SCardError})", "PCSC_ERROR");
        }
    }

    // ── Personal-cert filter ──────────────────────────────────────────────

    private static bool IsPersonalCpsCertificate(X509Certificate2 cert)
    {
        var attrs = ParseSubject(cert);
        return attrs.ContainsKey(OidSurname) && attrs.ContainsKey(OidGivenName);
    }

    // ── Mapping ────────────────────────────────────────────────────────────

    private CpsCardDto Map(X509Certificate2 cert, string cardId)
    {
        var attrs = ParseSubject(cert);

        string Get(string oid) => attrs.TryGetValue(oid, out var v) ? v : string.Empty;

        var nom         = Get(OidSurname);
        var prenom      = Get(OidGivenName);
        var identifiant = Get(OidCommonName);
        var profession  = Get(OidTitle);

        var emission   = DateOnly.FromDateTime(cert.NotBefore.ToUniversalTime());
        var expiration = DateOnly.FromDateTime(cert.NotAfter.ToUniversalTime());

        _logger.LogInformation(
            "Carte CPS — {Nom}/{Prenom} (id {Id}, {Pro}) — carte {Card} ({Iss} → {Exp})",
            nom, prenom, identifiant, profession, cardId, emission, expiration);

        return new CpsCardDto(
            Porteur: new CpsPorteurDto(nom, prenom, identifiant, profession),
            Carte:   new CpsCarteDto(cardId, emission, expiration));
    }

    /// <summary>
    /// Walks the cert's Subject DN as raw DER (ASN.1) and returns a flat
    /// dictionary of OID → value. Handles multi-AVA RDNs that .NET's
    /// <c>GetSingleElementType()</c> rejects.
    /// </summary>
    private static Dictionary<string, string> ParseSubject(X509Certificate2 cert)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);

        var reader = new System.Formats.Asn1.AsnReader(
            cert.SubjectName.RawData, System.Formats.Asn1.AsnEncodingRules.DER);

        var seq = reader.ReadSequence(); // SEQUENCE of RDNs
        while (seq.HasData)
        {
            // RDN ::= SET OF AttributeTypeAndValue
            var set = seq.ReadSetOf();
            while (set.HasData)
            {
                // AttributeTypeAndValue ::= SEQUENCE { type OID, value ANY }
                var ava = set.ReadSequence();
                var oid = ava.ReadObjectIdentifier();
                var value = ava.ReadCharacterString(ResolveStringTag(ava));
                if (!result.ContainsKey(oid)) result[oid] = value;
            }
        }

        return result;
    }

    private static System.Formats.Asn1.UniversalTagNumber ResolveStringTag(System.Formats.Asn1.AsnReader ava)
    {
        // Peek the next tag to choose the right ReadCharacterString variant
        var tag = ava.PeekTag();
        return tag.TagValue switch
        {
            (int)System.Formats.Asn1.UniversalTagNumber.UTF8String      => System.Formats.Asn1.UniversalTagNumber.UTF8String,
            (int)System.Formats.Asn1.UniversalTagNumber.PrintableString => System.Formats.Asn1.UniversalTagNumber.PrintableString,
            (int)System.Formats.Asn1.UniversalTagNumber.IA5String       => System.Formats.Asn1.UniversalTagNumber.IA5String,
            (int)System.Formats.Asn1.UniversalTagNumber.BMPString       => System.Formats.Asn1.UniversalTagNumber.BMPString,
            (int)System.Formats.Asn1.UniversalTagNumber.T61String       => System.Formats.Asn1.UniversalTagNumber.T61String,
            _                                                            => System.Formats.Asn1.UniversalTagNumber.UTF8String,
        };
    }
}
