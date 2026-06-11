namespace SGVO.Domain.Entities;

/// <summary>
/// Represents an organizational unit (faculty, department, division, etc.) in the hierarchy.
/// </summary>
public class UnidadOrganizativa
{
    public ulong Id { get; private set; }
    public string Nombre { get; private set; } = null!;
    public string Tipo { get; private set; } = null!;
    public int? NivelJerarquico { get; private set; }
    public ulong? PadreId { get; private set; }
    public bool Activo { get; private set; }
    public DateTime CreadoEn { get; private set; }
    public DateTime? ModificadoEn { get; private set; }
    public DateTime? EliminadoEn { get; private set; }
    public ulong? EliminadoPor { get; private set; }

    // Private constructor for EF Core
    private UnidadOrganizativa() { }

    /// <summary>
    /// Factory constructor to create a new organizational unit.
    /// </summary>
    public UnidadOrganizativa(string nombre, string tipo, int? nivelJerarquico = null, ulong? padreId = null)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de la unidad organizativa es obligatorio.", nameof(nombre));

        if (nombre.Length > 200)
            throw new ArgumentException("El nombre no puede exceder 200 caracteres.", nameof(nombre));

        if (string.IsNullOrWhiteSpace(tipo))
            throw new ArgumentException("El tipo de unidad organizativa es obligatorio.", nameof(tipo));

        if (tipo.Length > 50)
            throw new ArgumentException("El tipo no puede exceder 50 caracteres.", nameof(tipo));

        Nombre = nombre.Trim();
        Tipo = tipo.Trim();
        NivelJerarquico = nivelJerarquico;
        PadreId = padreId;
        Activo = true;
        CreadoEn = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the organizational unit's data.
    /// </summary>
    public void Actualizar(string nombre, string tipo, int? nivelJerarquico, ulong? padreId)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de la unidad organizativa es obligatorio.", nameof(nombre));

        if (nombre.Length > 200)
            throw new ArgumentException("El nombre no puede exceder 200 caracteres.", nameof(nombre));

        if (string.IsNullOrWhiteSpace(tipo))
            throw new ArgumentException("El tipo de unidad organizativa es obligatorio.", nameof(tipo));

        if (tipo.Length > 50)
            throw new ArgumentException("El tipo no puede exceder 50 caracteres.", nameof(tipo));

        Nombre = nombre.Trim();
        Tipo = tipo.Trim();
        NivelJerarquico = nivelJerarquico;
        PadreId = padreId;
        ModificadoEn = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the organizational unit as soft-deleted.
    /// </summary>
    public void Eliminar(ulong eliminadoPor)
    {
        EliminadoEn = DateTime.UtcNow;
        EliminadoPor = eliminadoPor;
        Activo = false;
        ModificadoEn = DateTime.UtcNow;
    }

    /// <summary>
    /// Reactivates a soft-deleted organizational unit.
    /// </summary>
    public void Reactivar()
    {
        EliminadoEn = null;
        EliminadoPor = null;
        Activo = true;
        ModificadoEn = DateTime.UtcNow;
    }
}
