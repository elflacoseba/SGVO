using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SGVO.Infrastructure.Persistence.Generated.Entities;

[Index("EliminadoPor", Name = "FK_CargoSkills_EliminadoPor")]
[Index("SkillId", Name = "FK_CargoSkills_Skill")]
[Index("CargoId", "SkillId", Name = "IX_CargoSkills_Unique", IsUnique = true)]
public partial class CargoSkill
{
    [Key]
    public ulong Id { get; set; }

    public ulong CargoId { get; set; }

    public ulong SkillId { get; set; }

    /// <summary>
    /// 1=Crítico, 2=Importante, 3=Deseable, 4=Secundario
    /// </summary>
    public byte NivelImportancia { get; set; }

    [Required]
    public bool? Activo { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EliminadoEn { get; set; }

    public ulong? EliminadoPor { get; set; }

    [ForeignKey("CargoId")]
    [InverseProperty("CargoSkills")]
    public virtual Cargo Cargo { get; set; } = null!;

    [ForeignKey("EliminadoPor")]
    [InverseProperty("CargoSkills")]
    public virtual Usuario? EliminadoPorNavigation { get; set; }

    [ForeignKey("SkillId")]
    [InverseProperty("CargoSkills")]
    public virtual Skill Skill { get; set; } = null!;
}
