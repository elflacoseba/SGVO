namespace SGVO.Application.Features.UnidadesOrganizativas.Queries;

/// <summary>
/// DTO for the hierarchical tree view of organizational units.
/// Children are nested recursively.
/// </summary>
public class UnidadOrganizativaTreeDto
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public int? NivelJerarquico { get; set; }
    public long? PadreId { get; set; }
    public bool Activo { get; set; }
    public List<UnidadOrganizativaTreeDto> Hijos { get; set; } = new();
}
