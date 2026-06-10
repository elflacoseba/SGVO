using System.ComponentModel.DataAnnotations.Schema;

namespace SGVO.Infrastructure.Persistence.Entities;

/// <summary>
/// Entidad de persistencia para Vacante. Mapea a la tabla 'Vacantes' en MySQL.
/// </summary>
public class VacanteEntity
{
    public ulong Id { get; set; }
    public ulong PuestoId { get; set; }
    public DateTime FechaApertura { get; set; }
    public DateTime? FechaCierre { get; set; }
    public string Motivo { get; set; } = null!;
    public string Estado { get; set; } = null!;
    public string? Observaciones { get; set; }
    public ulong? ResponsableId { get; set; }
    public bool? Activo { get; set; }
    public DateTime CreadoEn { get; set; }
    public DateTime? ModificadoEn { get; set; }
    public DateTime? EliminadoEn { get; set; }
    public ulong? EliminadoPor { get; set; }

    // Navigation properties
    public virtual UsuarioEntity? EliminadoPorNavigation { get; set; }
    public virtual PuestoEntity Puesto { get; set; } = null!;
    public virtual PersonaEntity? Responsable { get; set; }
    public virtual ICollection<PostulacioneEntity> Postulaciones { get; set; } = new List<PostulacioneEntity>();
}
