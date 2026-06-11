using System.ComponentModel.DataAnnotations.Schema;

namespace SGVO.Infrastructure.Persistence.Entities;

/// <summary>
/// Entidad de persistencia para Auditoria. Mapea a la tabla 'Auditorias' en MySQL.
/// </summary>
public class AuditoriaEntity
{
    public long Id { get; set; }
    public string Tabla { get; set; } = null!;
    public long EntidadId { get; set; }
    public string Operacion { get; set; } = null!;
    public DateTime FechaHora { get; set; }
    public long? UsuarioId { get; set; }
    public string? ValoresAnterior { get; set; }
    public string? ValoresNuevo { get; set; }

    // Navigation properties
    public virtual UsuarioEntity? Usuario { get; set; }
}
