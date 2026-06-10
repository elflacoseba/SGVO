using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SGVO.Infrastructure.Persistence.Entities;

[Index("EliminadoPor", Name = "FK_Vacantes_EliminadoPor")]
[Index("ResponsableId", Name = "FK_Vacantes_Responsable")]
[Index("Estado", Name = "IX_Vacantes_Estado")]
[Index("PuestoId", "Estado", Name = "IX_Vacantes_PuestoEstado")]
public partial class Vacante
{
    [Key]
    public ulong Id { get; set; }

    public ulong PuestoId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaApertura { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaCierre { get; set; }

    [StringLength(500)]
    public string Motivo { get; set; } = null!;

    /// <summary>
    /// Abierta, EnSeleccion, Cubierta, Cancelada
    /// </summary>
    [StringLength(50)]
    public string Estado { get; set; } = null!;

    [StringLength(1000)]
    public string? Observaciones { get; set; }

    public ulong? ResponsableId { get; set; }

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
    [InverseProperty("Vacantes")]
    public virtual Usuario? EliminadoPorNavigation { get; set; }

    [InverseProperty("Vacante")]
    public virtual ICollection<Postulacione> Postulaciones { get; set; } = new List<Postulacione>();

    [ForeignKey("PuestoId")]
    [InverseProperty("Vacantes")]
    public virtual Puesto Puesto { get; set; } = null!;

    [ForeignKey("ResponsableId")]
    [InverseProperty("Vacantes")]
    public virtual Persona? Responsable { get; set; }
}
