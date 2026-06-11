using System.ComponentModel.DataAnnotations.Schema;

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
    public virtual CargoEntity Cargo { get; set; } = null!;
    public virtual SkillEntity Skill { get; set; } = null!;
    public virtual UsuarioEntity? EliminadoPorNavigation { get; set; }
}
