using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SGVO.Infrastructure.Persistence.Entities;

[Index("EliminadoPor", Name = "FK_Postulantes_EliminadoPor")]
[Index("PersonaId", Name = "IX_Postulantes_PersonaId")]
public partial class Postulante
{
    [Key]
    public ulong Id { get; set; }

    /// <summary>
    /// NULL si es externo
    /// </summary>
    public ulong? PersonaId { get; set; }

    [StringLength(150)]
    public string Nombre { get; set; } = null!;

    [StringLength(150)]
    public string Apellido { get; set; } = null!;

    [StringLength(200)]
    public string Email { get; set; } = null!;

    [StringLength(50)]
    public string? Telefono { get; set; }

    /// <summary>
    /// Interno, Externo, Recomendado
    /// </summary>
    [StringLength(50)]
    public string Origen { get; set; } = null!;

    [StringLength(500)]
    public string? CurriculumUrl { get; set; }

    [Required]
    public bool? Activo { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreadoEn { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EliminadoEn { get; set; }

    public ulong? EliminadoPor { get; set; }

    [ForeignKey("EliminadoPor")]
    [InverseProperty("Postulantes")]
    public virtual Usuario? EliminadoPorNavigation { get; set; }

    [ForeignKey("PersonaId")]
    [InverseProperty("Postulantes")]
    public virtual Persona? Persona { get; set; }

    [InverseProperty("Postulante")]
    public virtual ICollection<Postulacione> Postulaciones { get; set; } = new List<Postulacione>();
}
