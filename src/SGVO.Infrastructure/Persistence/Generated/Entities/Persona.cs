using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SGVO.Infrastructure.Persistence.Generated.Entities;

[Index("EliminadoPor", Name = "FK_Personas_EliminadoPor")]
[Index("Documento", Name = "IX_Personas_Documento", IsUnique = true)]
[Index("Email", Name = "IX_Personas_Email", IsUnique = true)]
[Index("Legajo", Name = "IX_Personas_Legajo", IsUnique = true)]
public partial class Persona
{
    [Key]
    public ulong Id { get; set; }

    [StringLength(150)]
    public string Nombre { get; set; } = null!;

    [StringLength(150)]
    public string Apellido { get; set; } = null!;

    [StringLength(200)]
    public string Email { get; set; } = null!;

    [StringLength(50)]
    public string? Legajo { get; set; }

    [StringLength(50)]
    public string Documento { get; set; } = null!;

    public DateOnly? FechaNacimiento { get; set; }

    [Required]
    public bool? Activo { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreadoEn { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModificadoEn { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EliminadoEn { get; set; }

    public ulong? EliminadoPor { get; set; }

    [ForeignKey("EliminadoPor")]
    [InverseProperty("Personas")]
    public virtual Usuario? EliminadoPorNavigation { get; set; }

    [InverseProperty("Persona")]
    public virtual ICollection<Ocupacione> Ocupaciones { get; set; } = new List<Ocupacione>();

    [InverseProperty("Persona")]
    public virtual ICollection<PersonaSkill> PersonaSkills { get; set; } = new List<PersonaSkill>();

    [InverseProperty("Evaluador")]
    public virtual ICollection<Postulacione> Postulaciones { get; set; } = new List<Postulacione>();

    [InverseProperty("Persona")]
    public virtual ICollection<Postulante> Postulantes { get; set; } = new List<Postulante>();

    [InverseProperty("Persona")]
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();

    [InverseProperty("Responsable")]
    public virtual ICollection<Vacante> Vacantes { get; set; } = new List<Vacante>();
}
