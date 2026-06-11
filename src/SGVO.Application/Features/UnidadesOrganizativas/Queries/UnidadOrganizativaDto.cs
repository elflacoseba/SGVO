namespace SGVO.Application.Features.UnidadesOrganizativas.Queries;

/// <summary>
/// DTO for a flat list of organizational units.
/// </summary>
public class UnidadOrganizativaDto
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public int? NivelJerarquico { get; set; }
    public long? PadreId { get; set; }
    public bool Activo { get; set; }
}
