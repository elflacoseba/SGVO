using System.ComponentModel.DataAnnotations.Schema;

namespace SGVO.Infrastructure.Persistence.Entities;

/// <summary>
/// Entidad de persistencia para PersonaSkill. Mapea a la tabla 'PersonaSkills' en MySQL.
/// </summary>
public class PersonaSkillEntity
{
    public long Id { get; set; }
    public long PersonaId { get; set; }
    public long SkillId { get; set; }
    public int NivelDominio { get; set; }
    public bool? Activo { get; set; }
    public DateTime? EliminadoEn { get; set; }
    public long? EliminadoPor { get; set; }

    // Navigation properties
    public virtual PersonaEntity Persona { get; set; } = null!;
    public virtual SkillEntity Skill { get; set; } = null!;
    public virtual UsuarioEntity? EliminadoPorNavigation { get; set; }
}
