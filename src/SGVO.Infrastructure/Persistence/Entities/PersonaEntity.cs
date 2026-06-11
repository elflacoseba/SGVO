using System.ComponentModel.DataAnnotations.Schema;

namespace SGVO.Infrastructure.Persistence.Entities;

/// <summary>
/// Entidad de persistencia para Persona. Mapea a la tabla 'Personas' en MySQL.
/// </summary>
public class PersonaEntity
{
    public long Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public string? Documento { get; set; }
    public bool? Activo { get; set; }
    public DateTime CreadoEn { get; set; }
    public DateTime? ModificadoEn { get; set; }
    public DateTime? EliminadoEn { get; set; }
    public long? EliminadoPor { get; set; }

    // Navigation properties
    public virtual UsuarioEntity? EliminadoPorNavigation { get; set; }
    public virtual ICollection<OcupacioneEntity> Ocupaciones { get; set; } = new List<OcupacioneEntity>();
    public virtual ICollection<PersonaSkillEntity> PersonaSkills { get; set; } = new List<PersonaSkillEntity>();
    public virtual ICollection<PostulanteEntity> Postulantes { get; set; } = new List<PostulanteEntity>();
    public virtual ICollection<UsuarioEntity> Usuarios { get; set; } = new List<UsuarioEntity>();
}
