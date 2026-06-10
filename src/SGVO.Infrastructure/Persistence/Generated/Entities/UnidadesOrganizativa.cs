using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SGVO.Infrastructure.Persistence.Generated.Entities;

[Index("EliminadoPor", Name = "FK_UnidadesOrganizativas_EliminadoPor")]
[Index("PadreId", Name = "IX_UnidadesOrganizativas_PadreId")]
[Index("Tipo", Name = "IX_UnidadesOrganizativas_Tipo")]
public partial class UnidadesOrganizativa
{
    [Key]
    public ulong Id { get; set; }

    [StringLength(200)]
    public string Nombre { get; set; } = null!;

    /// <summary>
    /// Facultad, Secretaría, Dirección, Departamento, División, Área
    /// </summary>
    [StringLength(50)]
    public string Tipo { get; set; } = null!;

    public ulong? PadreId { get; set; }

    public byte NivelJerarquico { get; set; }

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
    [InverseProperty("UnidadesOrganizativas")]
    public virtual Usuario? EliminadoPorNavigation { get; set; }

    [InverseProperty("Padre")]
    public virtual ICollection<UnidadesOrganizativa> InversePadre { get; set; } = new List<UnidadesOrganizativa>();

    [ForeignKey("PadreId")]
    [InverseProperty("InversePadre")]
    public virtual UnidadesOrganizativa? Padre { get; set; }

    [InverseProperty("UnidadOrganizativa")]
    public virtual ICollection<Puesto> Puestos { get; set; } = new List<Puesto>();
}
