using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SGVO.Infrastructure.Persistence.Entities;

[Index("EliminadoPor", Name = "FK_PersonaSkills_EliminadoPor")]
[Index("SkillId", Name = "FK_PersonaSkills_Skill")]
[Index("PersonaId", "SkillId", Name = "IX_PersonaSkills_Unique", IsUnique = true)]
public partial class PersonaSkill
{
    [Key]
    public ulong Id { get; set; }

    public ulong PersonaId { get; set; }

    public ulong SkillId { get; set; }

    /// <summary>
    /// 1=Básico, 2=Intermedio, 3=Avanzado, 4=Experto
    /// </summary>
    public byte NivelDominio { get; set; }

    public DateOnly? FechaCertificacion { get; set; }

    [Required]
    public bool? Activo { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EliminadoEn { get; set; }

    public ulong? EliminadoPor { get; set; }

    [ForeignKey("EliminadoPor")]
    [InverseProperty("PersonaSkills")]
    public virtual Usuario? EliminadoPorNavigation { get; set; }

    [ForeignKey("PersonaId")]
    [InverseProperty("PersonaSkills")]
    public virtual Persona Persona { get; set; } = null!;

    [ForeignKey("SkillId")]
    [InverseProperty("PersonaSkills")]
    public virtual Skill Skill { get; set; } = null!;
}
