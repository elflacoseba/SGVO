namespace SGVO.Application.Features.UnidadesOrganizativas.Dtos;

/// <summary>
/// Request DTO for updating an existing organizational unit.
/// </summary>
public class UpdateUnidadOrganizativaRequest
{
    public string Nombre { get; set; } = string.Empty;
    public long TipoUnidadOrganizativaId { get; set; }
    public int? NivelJerarquico { get; set; }
    public long? PadreId { get; set; }
}
