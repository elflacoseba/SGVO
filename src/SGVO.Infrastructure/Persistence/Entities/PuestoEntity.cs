using System.ComponentModel.DataAnnotations.Schema;

namespace SGVO.Infrastructure.Persistence.Entities;

/// <summary>
/// Entidad de persistencia para Puesto. Mapea a la tabla 'Puestos' en MySQL.
/// </summary>
public class PuestoEntity
{
    public long Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public long? UnidadOrganizativaId { get; set; }
    public long? CargoId { get; set; }
    public long? SuperiorId { get; set; }
    public bool? Activo { get; set; }
    public DateTime CreadoEn { get; set; }
    public DateTime? ModificadoEn { get; set; }
    public DateTime? EliminadoEn { get; set; }
    public long? EliminadoPor { get; set; }

    // Navigation properties
    public virtual UsuarioEntity? EliminadoPorNavigation { get; set; }
    public virtual CargoEntity? Cargo { get; set; }
    public virtual UnidadesOrganizativaEntity? UnidadOrganizativa { get; set; }
    public virtual PuestoEntity? Superior { get; set; }
    public virtual ICollection<PuestoEntity> InverseSuperior { get; set; } = new List<PuestoEntity>();
    public virtual ICollection<OcupacioneEntity> Ocupaciones { get; set; } = new List<OcupacioneEntity>();
    public virtual ICollection<VacanteEntity> Vacantes { get; set; } = new List<VacanteEntity>();
}
