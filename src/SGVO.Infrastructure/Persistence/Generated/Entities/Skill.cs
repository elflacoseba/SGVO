using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SGVO.Infrastructure.Persistence.Generated.Entities;

[Index("EliminadoPor", Name = "FK_Skills_EliminadoPor")]
public partial class Skill
{
    [Key]
    public ulong Id { get; set; }

    [StringLength(150)]
    public string Nombre { get; set; } = null!;

    /// <summary>
    /// Técnica, Blanda, Gerencial, etc.
    /// </summary>
    [StringLength(100)]
    public string? Categoria { get; set; }

    [StringLength(500)]
    public string? Descripcion { get; set; }

    [Required]
    public bool? Activo { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreadoEn { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EliminadoEn { get; set; }

    public ulong? EliminadoPor { get; set; }

    [InverseProperty("Skill")]
    public virtual ICollection<CargoSkill> CargoSkills { get; set; } = new List<CargoSkill>();

    [ForeignKey("EliminadoPor")]
    [InverseProperty("Skills")]
    public virtual Usuario? EliminadoPorNavigation { get; set; }

    [InverseProperty("Skill")]
    public virtual ICollection<PersonaSkill> PersonaSkills { get; set; } = new List<PersonaSkill>();
}
