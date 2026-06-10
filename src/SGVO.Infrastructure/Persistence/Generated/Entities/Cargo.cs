using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SGVO.Infrastructure.Persistence.Generated.Entities;

[Index("EliminadoPor", Name = "FK_Cargos_EliminadoPor")]
public partial class Cargo
{
    [Key]
    public ulong Id { get; set; }

    [StringLength(150)]
    public string Nombre { get; set; } = null!;

    [StringLength(500)]
    public string? Descripcion { get; set; }

    [Required]
    public bool? Activo { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreadoEn { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModificadoEn { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EliminadoEn { get; set; }

    public ulong? EliminadoPor { get; set; }

    [InverseProperty("Cargo")]
    public virtual ICollection<CargoSkill> CargoSkills { get; set; } = new List<CargoSkill>();

    [ForeignKey("EliminadoPor")]
    [InverseProperty("Cargos")]
    public virtual Usuario? EliminadoPorNavigation { get; set; }

    [InverseProperty("Cargo")]
    public virtual ICollection<Puesto> Puestos { get; set; } = new List<Puesto>();
}
