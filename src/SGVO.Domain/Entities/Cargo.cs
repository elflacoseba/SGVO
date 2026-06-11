namespace SGVO.Domain.Entities;

/// <summary>
/// Representa un cargo o posición dentro de la organización.
/// </summary>
public class Cargo : IEntity
{
    public long Id { get; private set; }
    public string Nombre { get; private set; } = null!;
    public string? Descripcion { get; private set; }
    public bool Activo { get; private set; }
    public DateTime CreadoEn { get; private set; }
    public DateTime? ModificadoEn { get; private set; }
    public DateTime? EliminadoEn { get; private set; }
    public long? EliminadoPor { get; private set; }

    // Constructor privado para EF Core
    private Cargo() { }

    /// <summary>
    /// Constructor para crear un nuevo cargo.
    /// </summary>
    public Cargo(string nombre, string? descripcion = null)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del cargo es obligatorio.", nameof(nombre));

        if (nombre.Length > 150)
            throw new ArgumentException("El nombre del cargo no puede exceder 150 caracteres.", nameof(nombre));

        if (descripcion?.Length > 500)
            throw new ArgumentException("La descripción no puede exceder 500 caracteres.", nameof(descripcion));

        Nombre = nombre.Trim();
        Descripcion = descripcion?.Trim();
        Activo = true;
        CreadoEn = DateTime.UtcNow;
    }

    /// <summary>
    /// Actualiza los datos del cargo.
    /// </summary>
    public void Actualizar(string nombre, string? descripcion)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del cargo es obligatorio.", nameof(nombre));

        if (nombre.Length > 150)
            throw new ArgumentException("El nombre del cargo no puede exceder 150 caracteres.", nameof(nombre));

        if (descripcion?.Length > 500)
            throw new ArgumentException("La descripción no puede exceder 500 caracteres.", nameof(descripcion));

        Nombre = nombre.Trim();
        Descripcion = descripcion?.Trim();
        ModificadoEn = DateTime.UtcNow;
    }

    /// <summary>
    /// Marca el cargo como eliminado (soft delete).
    /// </summary>
    public void Eliminar(long eliminadoPor)
    {
        EliminadoEn = DateTime.UtcNow;
        EliminadoPor = eliminadoPor;
        Activo = false;
        ModificadoEn = DateTime.UtcNow;
    }

    /// <summary>
    /// Reactiva el cargo.
    /// </summary>
    public void Reactivar()
    {
        EliminadoEn = null;
        EliminadoPor = null;
        Activo = true;
        ModificadoEn = DateTime.UtcNow;
    }
}
