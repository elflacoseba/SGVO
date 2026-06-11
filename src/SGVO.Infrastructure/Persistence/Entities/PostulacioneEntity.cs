using System.ComponentModel.DataAnnotations.Schema;

namespace SGVO.Infrastructure.Persistence.Entities;

/// <summary>
/// Entidad de persistencia para Postulacione. Mapea a la tabla 'Postulaciones' en MySQL.
/// </summary>
public class PostulacioneEntity
{
    public long Id { get; set; }
    public long VacanteId { get; set; }
    public long PostulanteId { get; set; }
    public DateTime FechaPostulacion { get; set; }
    public string Estado { get; set; } = null!;
    public decimal? PuntajeMatch { get; set; }
    public long? EvaluadorId { get; set; }
    public string? Observaciones { get; set; }
    public bool? Activo { get; set; }
    public DateTime CreadoEn { get; set; }
    public DateTime? ModificadoEn { get; set; }
    public DateTime? EliminadoEn { get; set; }
    public long? EliminadoPor { get; set; }

    // Navigation properties
    public virtual UsuarioEntity? EliminadoPorNavigation { get; set; }
    public virtual UsuarioEntity? Evaluador { get; set; }
    public virtual PostulanteEntity Postulante { get; set; } = null!;
    public virtual VacanteEntity Vacante { get; set; } = null!;
}
