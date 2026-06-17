namespace Dehempe.Domain.ValueObjects;

/// <summary>
/// Identifiant National de Santé (NIR ou NIA) — INS-C ou INS-NIR.
/// Format matricule : 13 chiffres + clé 2 chiffres.
/// </summary>
public sealed class Ins : IEquatable<Ins>
{
    public string Value { get; }
    public InsOid Oid { get; }

    private Ins(string value, InsOid oid)
    {
        Value = value;
        Oid = oid;
    }

    public static Ins CreateNir(string nir)
    {
        if (!IsValidMatricule(nir))
            throw new ArgumentException($"NIR invalide : {nir}", nameof(nir));
        return new Ins(nir, InsOid.Nir);
    }

    public static Ins CreateNia(string nia)
    {
        if (string.IsNullOrWhiteSpace(nia))
            throw new ArgumentException("NIA invalide.", nameof(nia));
        return new Ins(nia, InsOid.Nia);
    }

    private static bool IsValidMatricule(string v)
        => v?.Length == 15 && v.All(char.IsDigit);

    public override string ToString() => Value;
    public bool Equals(Ins? other) => other is not null && Value == other.Value && Oid == other.Oid;
    public override bool Equals(object? obj) => Equals(obj as Ins);
    public override int GetHashCode() => HashCode.Combine(Value, Oid);
}

public enum InsOid
{
    /// <summary>OID ANS pour le NIR : 1.2.250.1.213.1.4.8</summary>
    Nir,
    /// <summary>OID ANS pour le NIA : 1.2.250.1.213.1.4.9</summary>
    Nia
}

public static class InsOidValues
{
    public const string Nir = "1.2.250.1.213.1.4.8";
    public const string Nia = "1.2.250.1.213.1.4.9";

    public static string ToOidString(this InsOid oid) => oid switch
    {
        InsOid.Nir => Nir,
        InsOid.Nia => Nia,
        _ => throw new ArgumentOutOfRangeException(nameof(oid))
    };
}
