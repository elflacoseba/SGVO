using System.ComponentModel.DataAnnotations.Schema;

namespace SGVO.Infrastructure.Persistence.Entities;

/// <summary>
/// Entidad de persistencia para UsuarioRole. Mapea a la tabla 'UsuarioRoles' en MySQL.
/// </summary>
public class UsuarioRoleEntity
{
    public ulong Id { get; set; }
    public ulong UsuarioId { get; set; }
    public ulong RolId { get; set; }
    public bool? Activo { get; set; }
    public DateTime? EliminadoEn { get; set; }
    public ulong? EliminadoPor { get; set; }

    // Navigation properties
    public virtual UsuarioEntity Usuario { get; set; } = null!;
    public virtual RoleEntity Rol { get; set; } = null!;
    public virtual UsuarioEntity? EliminadoPorNavigation { get; set; }
}
