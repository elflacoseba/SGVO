using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SGVO.Infrastructure.Persistence.Generated.Entities;

[Index("EliminadoPor", Name = "FK_UsuarioRoles_EliminadoPor")]
[Index("RolId", Name = "FK_UsuarioRoles_Rol")]
[Index("UsuarioId", "RolId", Name = "IX_UsuarioRoles_Unique", IsUnique = true)]
public partial class UsuarioRole
{
    [Key]
    public ulong Id { get; set; }

    public ulong UsuarioId { get; set; }

    public ulong RolId { get; set; }

    [Required]
    public bool? Activo { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EliminadoEn { get; set; }

    public ulong? EliminadoPor { get; set; }

    [ForeignKey("EliminadoPor")]
    [InverseProperty("UsuarioRoleEliminadoPorNavigations")]
    public virtual Usuario? EliminadoPorNavigation { get; set; }

    [ForeignKey("RolId")]
    [InverseProperty("UsuarioRoles")]
    public virtual Role Rol { get; set; } = null!;

    [ForeignKey("UsuarioId")]
    [InverseProperty("UsuarioRoleUsuarios")]
    public virtual Usuario Usuario { get; set; } = null!;
}
