using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SGVO.Infrastructure.Persistence.Entities;

[Index("EliminadoPor", Name = "FK_Usuarios_EliminadoPor")]
[Index("Email", Name = "IX_Usuarios_Email", IsUnique = true)]
[Index("PersonaId", Name = "IX_Usuarios_PersonaId")]
[Index("Username", Name = "IX_Usuarios_Username", IsUnique = true)]
public partial class Usuario
{
    [Key]
    public ulong Id { get; set; }

    [StringLength(100)]
    public string Username { get; set; } = null!;

    [StringLength(200)]
    public string Email { get; set; } = null!;

    [StringLength(500)]
    public string PasswordHash { get; set; } = null!;

    /// <summary>
    /// Vincula con Personas si el usuario es empleado
    /// </summary>
    public ulong? PersonaId { get; set; }

    [Required]
    public bool? Activo { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UltimoAcceso { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreadoEn { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModificadoEn { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EliminadoEn { get; set; }

    public ulong? EliminadoPor { get; set; }

    [InverseProperty("Usuario")]
    public virtual ICollection<Auditoria> Auditoria { get; set; } = new List<Auditoria>();

    [InverseProperty("EliminadoPorNavigation")]
    public virtual ICollection<CargoSkill> CargoSkills { get; set; } = new List<CargoSkill>();

    [InverseProperty("EliminadoPorNavigation")]
    public virtual ICollection<Cargo> Cargos { get; set; } = new List<Cargo>();

    [ForeignKey("EliminadoPor")]
    [InverseProperty("InverseEliminadoPorNavigation")]
    public virtual Usuario? EliminadoPorNavigation { get; set; }

    [InverseProperty("EliminadoPorNavigation")]
    public virtual ICollection<Usuario> InverseEliminadoPorNavigation { get; set; } = new List<Usuario>();

    [InverseProperty("EliminadoPorNavigation")]
    public virtual ICollection<Ocupacione> Ocupaciones { get; set; } = new List<Ocupacione>();

    [ForeignKey("PersonaId")]
    [InverseProperty("Usuarios")]
    public virtual Persona? Persona { get; set; }

    [InverseProperty("EliminadoPorNavigation")]
    public virtual ICollection<PersonaSkill> PersonaSkills { get; set; } = new List<PersonaSkill>();

    [InverseProperty("EliminadoPorNavigation")]
    public virtual ICollection<Persona> Personas { get; set; } = new List<Persona>();

    [InverseProperty("EliminadoPorNavigation")]
    public virtual ICollection<Postulacione> Postulaciones { get; set; } = new List<Postulacione>();

    [InverseProperty("EliminadoPorNavigation")]
    public virtual ICollection<Postulante> Postulantes { get; set; } = new List<Postulante>();

    [InverseProperty("EliminadoPorNavigation")]
    public virtual ICollection<Puesto> Puestos { get; set; } = new List<Puesto>();

    [InverseProperty("EliminadoPorNavigation")]
    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

    [InverseProperty("EliminadoPorNavigation")]
    public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();

    [InverseProperty("EliminadoPorNavigation")]
    public virtual ICollection<UnidadesOrganizativa> UnidadesOrganizativas { get; set; } = new List<UnidadesOrganizativa>();

    [InverseProperty("EliminadoPorNavigation")]
    public virtual ICollection<UsuarioRole> UsuarioRoleEliminadoPorNavigations { get; set; } = new List<UsuarioRole>();

    [InverseProperty("Usuario")]
    public virtual ICollection<UsuarioRole> UsuarioRoleUsuarios { get; set; } = new List<UsuarioRole>();

    [InverseProperty("EliminadoPorNavigation")]
    public virtual ICollection<Vacante> Vacantes { get; set; } = new List<Vacante>();
}
