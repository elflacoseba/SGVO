using System.ComponentModel.DataAnnotations.Schema;

namespace SGVO.Infrastructure.Persistence.Entities;

/// <summary>
/// Entidad de persistencia para Postulante. Mapea a la tabla 'Postulantes' en MySQL.
/// </summary>
public class PostulanteEntity
{
    public long Id { get; set; }
    public long? PersonaId { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Telefono { get; set; }
    public string Origen { get; set; } = null!;
    public string? CurriculumUrl { get; set; }
    public bool? Activo { get; set; }
    public DateTime CreadoEn { get; set; }
    public DateTime? EliminadoEn { get; set; }
    public long? EliminadoPor { get; set; }

    // Navigation properties
    public virtual UsuarioEntity? EliminadoPorNavigation { get; set; }
    public virtual PersonaEntity? Persona { get; set; }
    public virtual ICollection<PostulacioneEntity> Postulaciones { get; set; } = new List<PostulacioneEntity>();
}
