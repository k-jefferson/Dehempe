namespace Dehempe.Domain.Exceptions;

public class DmpException : Exception
{
    public string? ErrorCode { get; }

    /// <summary>URL DMP appelée (utile pour débogger un mauvais routage).</summary>
    public string? Endpoint    { get; set; }

    /// <summary>SOAPAction transmise dans le header HTTP.</summary>
    public string? SoapAction  { get; set; }

    /// <summary>Enveloppe SOAP envoyée au DMP (incluant le header VIHF signé).</summary>
    public string? RequestBody { get; set; }

    /// <summary>Réponse brute du DMP, si reçue (SOAP Fault, HTML d'erreur, etc.).</summary>
    public string? ResponseBody { get; set; }

    public DmpException(string message, string? errorCode = null)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    public DmpException(string message, Exception inner, string? errorCode = null)
        : base(message, inner)
    {
        ErrorCode = errorCode;
    }
}

public class DmpPatientNotFoundException : DmpException
{
    public DmpPatientNotFoundException(string ins)
        : base($"Patient introuvable dans le DMP pour l'INS : {ins}", "XDSUnknownPatientId") { }
}

public class DmpDocumentNotFoundException : DmpException
{
    public DmpDocumentNotFoundException(string documentUniqueId)
        : base($"Document introuvable dans le DMP : {documentUniqueId}", "XDSDocumentUniqueIdError") { }
}

public class DmpAuthException : DmpException
{
    public DmpAuthException(string message, Exception? inner = null)
        : base(message, inner ?? new Exception(message), "XDSRegistryNotAvailable") { }
}

/// <summary>
/// Émise quand le PIN de la carte CPS est requis pour effectuer l'opération en cours
/// (login PKCS#11) et qu'aucun PIN n'a été fourni par le frontend.
/// Le frontend doit demander le PIN à l'utilisateur et rejouer la requête avec le header
/// <c>X-Cps-Pin</c>.
/// </summary>
public class DmpPinRequiredException : DmpException
{
    public DmpPinRequiredException()
        : base("Le PIN de la carte CPS est requis. Le frontend doit le demander à l'utilisateur " +
               "et le transmettre dans le header HTTP X-Cps-Pin.",
               "CpsPinRequired") { }
}
