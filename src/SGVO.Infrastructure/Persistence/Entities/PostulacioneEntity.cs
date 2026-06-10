using System.ComponentModel.DataAnnotations.Schema;

namespace SGVO.Infrastructure.Persistence.Entities;

/// <summary>
/// Entidad de persistencia para Postulacione. Mapea a la tabla 'Postulaciones' en MySQL.
/// </summary>
public class PostulacioneEntity
{
    public ulong Id { get; set; }
    public ulong VacanteId { get; set; }
    public ulong PostulanteId { get; set; }
    public DateTime FechaPostulacion { get; set; }
    public string Estado { get; set; } = null!;
    public decimal? PuntajeMatch { get; set; }
    public ulong? EvaluadorId { get; set; }
    public string? Observaciones { get; set; }
    public bool? Activo { get; set; }
    public DateTime CreadoEn { get; set; }
    public DateTime? ModificadoEn { get; set; }
    public DateTime? EliminadoEn { get; set; }
    public ulong? EliminadoPor { get; set; }

    // Navigation properties
    public virtual UsuarioEntity? EliminadoPorNavigation { get; set; }
    public virtual UsuarioEntity? Evaluador { get; set; }
    public virtual PostulanteEntity Postulante { get; set; } = null!;
    public virtual VacanteEntity Vacante { get; set; } = null!;
}
