using System.ComponentModel.DataAnnotations.Schema;

namespace SGVO.Infrastructure.Persistence.Entities;

/// <summary>
/// Entidad de persistencia para Skill. Mapea a la tabla 'Skills' en MySQL.
/// </summary>
public class SkillEntity
{
    public long Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Categoria { get; set; }
    public string? Descripcion { get; set; }
    public bool? Activo { get; set; }
    public DateTime CreadoEn { get; set; }
    public DateTime? EliminadoEn { get; set; }
    public long? EliminadoPor { get; set; }

    // Navigation properties
    public virtual UsuarioEntity? EliminadoPorNavigation { get; set; }
    public virtual ICollection<CargoSkillEntity> CargoSkills { get; set; } = new List<CargoSkillEntity>();
    public virtual ICollection<PersonaSkillEntity> PersonaSkills { get; set; } = new List<PersonaSkillEntity>();
}
