using System.ComponentModel.DataAnnotations.Schema;
using SGVO.Domain.Entities;

namespace SGVO.Infrastructure.Persistence.Entities;

/// <summary>
/// Entidad de persistencia para Usuario. Mapea a la tabla 'Usuarios' en MySQL.
/// </summary>
public class UsuarioEntity
{
    public long Id { get; set; }
    public long? PersonaId { get; set; }
    public string NombreUsuario { get; set; } = null!;
    public string? Email { get; set; }
    public string PasswordHash { get; set; } = null!;
    public bool Activo { get; set; }
    public DateTime CreadoEn { get; set; }
    public DateTime? ModificadoEn { get; set; }
    public DateTime? EliminadoEn { get; set; }
    public long? EliminadoPor { get; set; }

    // Navigation properties
    public virtual PersonaEntity? Persona { get; set; }
    public virtual UsuarioEntity? EliminadoPorNavigation { get; set; }
    public virtual ICollection<UsuarioEntity> InverseEliminadoPorNavigation { get; set; } = new List<UsuarioEntity>();
    public virtual ICollection<AuditoriaEntity> Auditoria { get; set; } = new List<AuditoriaEntity>();
    public virtual ICollection<Cargo> Cargos { get; set; } = new List<Cargo>();
    public virtual ICollection<CargoSkillEntity> CargoSkills { get; set; } = new List<CargoSkillEntity>();
    public virtual ICollection<OcupacioneEntity> Ocupaciones { get; set; } = new List<OcupacioneEntity>();
    public virtual ICollection<PersonaEntity> Personas { get; set; } = new List<PersonaEntity>();
    public virtual ICollection<PersonaSkillEntity> PersonaSkills { get; set; } = new List<PersonaSkillEntity>();
    public virtual ICollection<PostulacioneEntity> Postulaciones { get; set; } = new List<PostulacioneEntity>();
    public virtual ICollection<PostulanteEntity> Postulantes { get; set; } = new List<PostulanteEntity>();
    public virtual ICollection<PuestoEntity> Puestos { get; set; } = new List<PuestoEntity>();
    public virtual ICollection<RoleEntity> Roles { get; set; } = new List<RoleEntity>();
    public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();
    public virtual ICollection<UnidadesOrganizativaEntity> UnidadesOrganizativas { get; set; } = new List<UnidadesOrganizativaEntity>();
    public virtual ICollection<UsuarioRoleEntity> UsuarioRoleEliminadoPorNavigations { get; set; } = new List<UsuarioRoleEntity>();
    public virtual ICollection<UsuarioRoleEntity> UsuarioRoleUsuarios { get; set; } = new List<UsuarioRoleEntity>();
    public virtual ICollection<VacanteEntity> Vacantes { get; set; } = new List<VacanteEntity>();
}
