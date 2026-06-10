namespace SGVO.Domain.ValueObjects;

/// <summary>
/// Clase base para objetos de valor (Value Objects).
/// Un objeto de valor se identifica por sus atributos, no por un identificador único.
/// Dos instancias con los mismos valores son consideradas iguales.
/// </summary>
public abstract class ValueObject : IEquatable<ValueObject>
{
    /// <summary>
    /// Obtiene los componentes de igualdad que definen la identidad del objeto de valor.
    /// </summary>
    protected abstract IEnumerable<object> GetEqualityComponents();

    public bool Equals(ValueObject? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (GetType() != other.GetType()) return false;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not ValueObject other) return false;

        return Equals(other);
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Aggregate(0, (hash, component) =>
                hash ^ (component?.GetHashCode() ?? 0));
    }

    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;

        return left.Equals(right);
    }

    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !(left == right);
    }
}
