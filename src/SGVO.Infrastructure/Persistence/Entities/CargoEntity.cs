using System.ComponentModel.DataAnnotations.Schema;

namespace SGVO.Infrastructure.Persistence.Entities;

/// <summary>
/// Entidad de persistencia para Cargo. Mapea a la tabla 'Cargos' en MySQL.
/// </summary>
public class CargoEntity
{
    public long Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool? Activo { get; set; }
    public DateTime CreadoEn { get; set; }
    public DateTime? ModificadoEn { get; set; }
    public DateTime? EliminadoEn { get; set; }
    public long? EliminadoPor { get; set; }

    // Navigation properties
    public virtual UsuarioEntity? EliminadoPorNavigation { get; set; }
    public virtual ICollection<CargoSkillEntity> CargoSkills { get; set; } = new List<CargoSkillEntity>();
    public virtual ICollection<PuestoEntity> Puestos { get; set; } = new List<PuestoEntity>();
}
