namespace Dehempe.Domain.ValueObjects;

/// <summary>
/// OID du dépôt XDS qui héberge le document (repositoryUniqueId).
/// </summary>
public sealed class RepositoryUniqueId
{
    public string Value { get; }

    public RepositoryUniqueId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("RepositoryUniqueId ne peut pas être vide.", nameof(value));
        Value = value;
    }

    public override string ToString() => Value;
}
