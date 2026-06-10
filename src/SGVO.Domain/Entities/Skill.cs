namespace SGVO.Domain.Entities;

/// <summary>
/// Representa una habilidad o competencia.
/// </summary>
public class Skill
{
    public ulong Id { get; private set; }
    public string Nombre { get; private set; } = null!;
    public string? Categoria { get; private set; }
    public string? Descripcion { get; private set; }
    public bool Activo { get; private set; }
    public DateTime CreadoEn { get; private set; }
    public DateTime? EliminadoEn { get; private set; }
    public ulong? EliminadoPor { get; private set; }

    // Constructor privado para EF Core
    private Skill() { }

    /// <summary>
    /// Constructor para crear un nuevo skill.
    /// </summary>
    public Skill(string nombre, string? categoria = null, string? descripcion = null)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del skill es obligatorio.", nameof(nombre));

        if (nombre.Length > 150)
            throw new ArgumentException("El nombre del skill no puede exceder 150 caracteres.", nameof(nombre));

        if (categoria?.Length > 100)
            throw new ArgumentException("La categoría no puede exceder 100 caracteres.", nameof(categoria));

        if (descripcion?.Length > 500)
            throw new ArgumentException("La descripción no puede exceder 500 caracteres.", nameof(descripcion));

        Nombre = nombre.Trim();
        Categoria = categoria?.Trim();
        Descripcion = descripcion?.Trim();
        Activo = true;
        CreadoEn = DateTime.UtcNow;
    }

    /// <summary>
    /// Actualiza los datos del skill.
    /// </summary>
    public void Actualizar(string nombre, string? categoria, string? descripcion)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del skill es obligatorio.", nameof(nombre));

        if (nombre.Length > 150)
            throw new ArgumentException("El nombre del skill no puede exceder 150 caracteres.", nameof(nombre));

        if (categoria?.Length > 100)
            throw new ArgumentException("La categoría no puede exceder 100 caracteres.", nameof(categoria));

        if (descripcion?.Length > 500)
            throw new ArgumentException("La descripción no puede exceder 500 caracteres.", nameof(descripcion));

        Nombre = nombre.Trim();
        Categoria = categoria?.Trim();
        Descripcion = descripcion?.Trim();
    }

    /// <summary>
    /// Marca el skill como eliminado (soft delete).
    /// </summary>
    public void Eliminar(ulong eliminadoPor)
    {
        EliminadoEn = DateTime.UtcNow;
        EliminadoPor = eliminadoPor;
        Activo = false;
    }

    /// <summary>
    /// Reactiva el skill.
    /// </summary>
    public void Reactivar()
    {
        EliminadoEn = null;
        EliminadoPor = null;
        Activo = true;
    }
}
