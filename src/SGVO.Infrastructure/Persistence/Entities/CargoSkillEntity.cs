using System.ComponentModel.DataAnnotations.Schema;
using SGVO.Domain.Entities;

namespace SGVO.Infrastructure.Persistence.Entities;

/// <summary>
/// Entidad de persistencia para CargoSkill. Mapea a la tabla 'CargoSkills' en MySQL.
/// </summary>
public class CargoSkillEntity
{
    public long Id { get; set; }
    public long CargoId { get; set; }
    public long SkillId { get; set; }
    public int NivelImportancia { get; set; }
    public bool? Activo { get; set; }
    public DateTime? EliminadoEn { get; set; }
    public long? EliminadoPor { get; set; }

    // Navigation properties
    public virtual Cargo Cargo { get; set; } = null!;
    public virtual Skill Skill { get; set; } = null!;
    public virtual UsuarioEntity? EliminadoPorNavigation { get; set; }
}
