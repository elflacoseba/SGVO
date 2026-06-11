using System.ComponentModel.DataAnnotations.Schema;

namespace SGVO.Infrastructure.Persistence.Entities;

/// <summary>
/// Entidad de persistencia para UnidadesOrganizativa. Mapea a la tabla 'UnidadesOrganizativas' en MySQL.
/// </summary>
public class UnidadesOrganizativaEntity
{
    public long Id { get; set; }
    public string Nombre { get; set; } = null!;
    public long TipoUnidadOrganizativaId { get; set; }
    public int? NivelJerarquico { get; set; }
    public long? PadreId { get; set; }
    public bool Activo { get; set; }
    public DateTime CreadoEn { get; set; }
    public DateTime? ModificadoEn { get; set; }
    public DateTime? EliminadoEn { get; set; }
    public long? EliminadoPor { get; set; }

    // Navigation properties
    public virtual TipoUnidadOrganizativaEntity? TipoUnidadOrganizativa { get; set; }
    public virtual UsuarioEntity? EliminadoPorNavigation { get; set; }
    public virtual UnidadesOrganizativaEntity? Padre { get; set; }
    public virtual ICollection<UnidadesOrganizativaEntity> InversePadre { get; set; } = new List<UnidadesOrganizativaEntity>();
    public virtual ICollection<PuestoEntity> Puestos { get; set; } = new List<PuestoEntity>();
}
