namespace Dehempe.Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string name, object key)
        : base($"Entité \"{name}\" ({key}) introuvable.") { }
}
