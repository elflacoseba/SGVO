namespace SGVO.Domain.Entities;

/// <summary>
/// Representa una vacante o posición abierta.
/// </summary>
public class Vacante
{
    public ulong Id { get; private set; }
    public ulong PuestoId { get; private set; }
    public DateTime FechaApertura { get; private set; }
    public DateTime? FechaCierre { get; private set; }
    public string Motivo { get; private set; } = null!;
    public string Estado { get; private set; } = null!;
    public string? Observaciones { get; private set; }
    public ulong? ResponsableId { get; private set; }
    public bool Activo { get; private set; }
    public DateTime CreadoEn { get; private set; }
    public DateTime? ModificadoEn { get; private set; }
    public DateTime? EliminadoEn { get; private set; }
    public ulong? EliminadoPor { get; private set; }

    // Constructor privado para EF Core
    private Vacante() { }

    /// <summary>
    /// Constructor para crear una nueva vacante.
    /// </summary>
    public Vacante(ulong puestoId, string motivo, string estado = "Abierta", ulong? responsableId = null)
    {
        if (puestoId == 0)
            throw new ArgumentException("El ID del puesto es obligatorio.", nameof(puestoId));

        if (string.IsNullOrWhiteSpace(motivo))
            throw new ArgumentException("El motivo es obligatorio.", nameof(motivo));

        if (string.IsNullOrWhiteSpace(estado))
            throw new ArgumentException("El estado es obligatorio.", nameof(estado));

        if (motivo.Length > 500)
            throw new ArgumentException("El motivo no puede exceder 500 caracteres.", nameof(motivo));

        if (estado.Length > 50)
            throw new ArgumentException("El estado no puede exceder 50 caracteres.", nameof(estado));

        PuestoId = puestoId;
        Motivo = motivo.Trim();
        Estado = estado.Trim();
        ResponsableId = responsableId;
        FechaApertura = DateTime.UtcNow;
        Activo = true;
        CreadoEn = DateTime.UtcNow;
    }

    /// <summary>
    /// Actualiza el estado de la vacante.
    /// </summary>
    public void CambiarEstado(string nuevoEstado)
    {
        if (string.IsNullOrWhiteSpace(nuevoEstado))
            throw new ArgumentException("El estado es obligatorio.", nameof(nuevoEstado));

        if (nuevoEstado.Length > 50)
            throw new ArgumentException("El estado no puede exceder 50 caracteres.", nameof(nuevoEstado));

        Estado = nuevoEstado.Trim();
        ModificadoEn = DateTime.UtcNow;

        if (nuevoEstado == "Cubierta" || nuevoEstado == "Cancelada")
        {
            FechaCierre = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Actualiza las observaciones de la vacante.
    /// </summary>
    public void ActualizarObservaciones(string? observaciones)
    {
        if (observaciones?.Length > 1000)
            throw new ArgumentException("Las observaciones no pueden exceder 1000 caracteres.", nameof(observaciones));

        Observaciones = observaciones?.Trim();
        ModificadoEn = DateTime.UtcNow;
    }

    /// <summary>
    /// Marca la vacante como eliminada (soft delete).
    /// </summary>
    public void Eliminar(ulong eliminadoPor)
    {
        EliminadoEn = DateTime.UtcNow;
        EliminadoPor = eliminadoPor;
        Activo = false;
        ModificadoEn = DateTime.UtcNow;
    }

    /// <summary>
    /// Reactiva la vacante.
    /// </summary>
    public void Reactivar()
    {
        EliminadoEn = null;
        EliminadoPor = null;
        Activo = true;
        ModificadoEn = DateTime.UtcNow;
    }
}
