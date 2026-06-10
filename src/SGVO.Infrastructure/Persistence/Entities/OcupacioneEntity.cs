using System.ComponentModel.DataAnnotations.Schema;

namespace SGVO.Infrastructure.Persistence.Entities;

/// <summary>
/// Entidad de persistencia para Ocupacione. Mapea a la tabla 'Ocupaciones' en MySQL.
/// </summary>
public class OcupacioneEntity
{
    public ulong Id { get; set; }
    public ulong PersonaId { get; set; }
    public ulong PuestoId { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string TipoOcupacion { get; set; } = null!;
    public bool? Activo { get; set; }
    public DateTime CreadoEn { get; set; }
    public DateTime? EliminadoEn { get; set; }
    public ulong? EliminadoPor { get; set; }

    // Navigation properties
    public virtual UsuarioEntity? EliminadoPorNavigation { get; set; }
    public virtual PersonaEntity Persona { get; set; } = null!;
    public virtual PuestoEntity Puesto { get; set; } = null!;
}
