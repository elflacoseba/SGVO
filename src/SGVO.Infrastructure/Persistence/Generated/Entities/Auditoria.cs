using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SGVO.Infrastructure.Persistence.Generated.Entities;

[Index("UsuarioId", Name = "FK_Auditorias_Usuario")]
[Index("FechaHora", Name = "IX_Auditorias_FechaHora")]
[Index("Tabla", "EntidadId", Name = "IX_Auditorias_TablaEntidad")]
public partial class Auditoria
{
    [Key]
    public ulong Id { get; set; }

    /// <summary>
    /// Nombre de la tabla afectada
    /// </summary>
    [StringLength(100)]
    public string Tabla { get; set; } = null!;

    /// <summary>
    /// PK de la entidad modificada
    /// </summary>
    public ulong EntidadId { get; set; }

    /// <summary>
    /// CREATE, UPDATE, DELETE
    /// </summary>
    [StringLength(20)]
    public string Operacion { get; set; } = null!;

    public ulong? UsuarioId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaHora { get; set; }

    /// <summary>
    /// Estado anterior en JSON
    /// </summary>
    [Column(TypeName = "json")]
    public string? ValoresAnterior { get; set; }

    /// <summary>
    /// Estado nuevo en JSON
    /// </summary>
    [Column(TypeName = "json")]
    public string? ValoresNuevo { get; set; }

    [StringLength(50)]
    public string? IpAddress { get; set; }

    [StringLength(500)]
    public string? UserAgent { get; set; }

    [ForeignKey("UsuarioId")]
    [InverseProperty("Auditoria")]
    public virtual Usuario? Usuario { get; set; }
}
