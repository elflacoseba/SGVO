using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SGVO.Infrastructure.Persistence.Generated.Entities;

[Index("CargoId", Name = "FK_Puestos_Cargo")]
[Index("EliminadoPor", Name = "FK_Puestos_EliminadoPor")]
[Index("Codigo", Name = "IX_Puestos_Codigo", IsUnique = true)]
[Index("SuperiorId", Name = "IX_Puestos_SuperiorId")]
[Index("UnidadOrganizativaId", "CargoId", Name = "IX_Puestos_UnidadCargo")]
public partial class Puesto
{
    [Key]
    public ulong Id { get; set; }

    [StringLength(200)]
    public string Nombre { get; set; } = null!;

    public ulong UnidadOrganizativaId { get; set; }

    public ulong CargoId { get; set; }

    public ulong? SuperiorId { get; set; }

    [StringLength(50)]
    public string? Codigo { get; set; }

    [Required]
    public bool? Activo { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreadoEn { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModificadoEn { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EliminadoEn { get; set; }

    public ulong? EliminadoPor { get; set; }

    [ForeignKey("CargoId")]
    [InverseProperty("Puestos")]
    public virtual Cargo Cargo { get; set; } = null!;

    [ForeignKey("EliminadoPor")]
    [InverseProperty("Puestos")]
    public virtual Usuario? EliminadoPorNavigation { get; set; }

    [InverseProperty("Superior")]
    public virtual ICollection<Puesto> InverseSuperior { get; set; } = new List<Puesto>();

    [InverseProperty("Puesto")]
    public virtual ICollection<Ocupacione> Ocupaciones { get; set; } = new List<Ocupacione>();

    [ForeignKey("SuperiorId")]
    [InverseProperty("InverseSuperior")]
    public virtual Puesto? Superior { get; set; }

    [ForeignKey("UnidadOrganizativaId")]
    [InverseProperty("Puestos")]
    public virtual UnidadesOrganizativa UnidadOrganizativa { get; set; } = null!;

    [InverseProperty("Puesto")]
    public virtual ICollection<Vacante> Vacantes { get; set; } = new List<Vacante>();
}
