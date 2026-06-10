using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SGVO.Infrastructure.Persistence.Entities;

[Index("EliminadoPor", Name = "FK_Ocupaciones_EliminadoPor")]
[Index("PersonaId", "FechaFin", Name = "IX_Ocupaciones_PersonaVigente")]
[Index("PuestoId", "FechaFin", Name = "IX_Ocupaciones_PuestoVigente")]
public partial class Ocupacione
{
    [Key]
    public ulong Id { get; set; }

    public ulong PersonaId { get; set; }

    public ulong PuestoId { get; set; }

    public DateOnly FechaInicio { get; set; }

    /// <summary>
    /// NULL = vigente
    /// </summary>
    public DateOnly? FechaFin { get; set; }

    /// <summary>
    /// Permanente, Interina, Suplente
    /// </summary>
    [StringLength(50)]
    public string TipoOcupacion { get; set; } = null!;

    [Required]
    public bool? Activo { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreadoEn { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EliminadoEn { get; set; }

    public ulong? EliminadoPor { get; set; }

    [ForeignKey("EliminadoPor")]
    [InverseProperty("Ocupaciones")]
    public virtual Usuario? EliminadoPorNavigation { get; set; }

    [ForeignKey("PersonaId")]
    [InverseProperty("Ocupaciones")]
    public virtual Persona Persona { get; set; } = null!;

    [ForeignKey("PuestoId")]
    [InverseProperty("Ocupaciones")]
    public virtual Puesto Puesto { get; set; } = null!;
}
