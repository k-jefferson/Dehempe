namespace Dehempe.Domain.Exceptions;

public class DmpException : Exception
{
    public string? ErrorCode { get; }

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
