namespace SGVO.Infrastructure.Persistence.Entities;

/// <summary>
/// Persistence entity for TiposUnidadOrganizativa. Maps to 'TiposUnidadOrganizativa' table in MySQL.
/// </summary>
public class TipoUnidadOrganizativaEntity
{
    public ulong Id { get; set; }
    public string Nombre { get; set; } = null!;
    public bool? Activo { get; set; }
    public DateTime CreadoEn { get; set; }
    public DateTime? ModificadoEn { get; set; }
    public DateTime? EliminadoEn { get; set; }
    public ulong? EliminadoPor { get; set; }

    // Navigation properties
    public virtual UsuarioEntity? EliminadoPorNavigation { get; set; }
    public virtual ICollection<UnidadesOrganizativaEntity> UnidadesOrganizativas { get; set; } = new List<UnidadesOrganizativaEntity>();
}
