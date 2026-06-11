namespace SGVO.Application.Features.UnidadesOrganizativas.Dtos;

/// <summary>
/// Request DTO for creating a new organizational unit.
/// </summary>
public class CreateUnidadOrganizativaRequest
{
    public string Nombre { get; set; } = string.Empty;
    public long TipoUnidadOrganizativaId { get; set; }
    public int? NivelJerarquico { get; set; }
    public long? PadreId { get; set; }
}
