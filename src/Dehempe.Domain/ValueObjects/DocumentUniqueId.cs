namespace Dehempe.Domain.ValueObjects;

public sealed class DocumentUniqueId : IEquatable<DocumentUniqueId>
{
    public string Value { get; }

    public DocumentUniqueId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("L'identifiant unique du document ne peut pas être vide.", nameof(value));
        Value = value;
    }

    public override string ToString() => Value;
    public bool Equals(DocumentUniqueId? other) => other is not null && Value == other.Value;
    public override bool Equals(object? obj) => Equals(obj as DocumentUniqueId);
    public override int GetHashCode() => Value.GetHashCode();
}
