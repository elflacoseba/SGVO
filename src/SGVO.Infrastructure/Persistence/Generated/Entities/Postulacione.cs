using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SGVO.Infrastructure.Persistence.Generated.Entities;

[Index("EliminadoPor", Name = "FK_Postulaciones_EliminadoPor")]
[Index("EvaluadorId", Name = "FK_Postulaciones_Evaluador")]
[Index("PostulanteId", Name = "FK_Postulaciones_Postulante")]
[Index("PuntajeMatch", Name = "IX_Postulaciones_PuntajeMatch")]
[Index("VacanteId", "Estado", Name = "IX_Postulaciones_VacanteEstado")]
[Index("VacanteId", "PostulanteId", Name = "IX_Postulaciones_VacantePostulante", IsUnique = true)]
public partial class Postulacione
{
    [Key]
    public ulong Id { get; set; }

    public ulong VacanteId { get; set; }

    public ulong PostulanteId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaPostulacion { get; set; }

    /// <summary>
    /// Postulado, Preseleccionado, Entrevistado, Aprobado, Rechazado, Contratado
    /// </summary>
    [StringLength(50)]
    public string Estado { get; set; } = null!;

    /// <summary>
    /// 0.00 - 100.00
    /// </summary>
    [Precision(5, 2)]
    public decimal? PuntajeMatch { get; set; }

    public bool? CumplePerfilTotal { get; set; }

    [StringLength(2000)]
    public string? Observaciones { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaEvaluacion { get; set; }

    public ulong? EvaluadorId { get; set; }

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
    [InverseProperty("Postulaciones")]
    public virtual Usuario? EliminadoPorNavigation { get; set; }

    [ForeignKey("EvaluadorId")]
    [InverseProperty("Postulaciones")]
    public virtual Persona? Evaluador { get; set; }

    [ForeignKey("PostulanteId")]
    [InverseProperty("Postulaciones")]
    public virtual Postulante Postulante { get; set; } = null!;

    [ForeignKey("VacanteId")]
    [InverseProperty("Postulaciones")]
    public virtual Vacante Vacante { get; set; } = null!;
}
