namespace SGVO.Application.Features.TiposUnidadOrganizativa.Dtos;

/// <summary>
/// Request DTO for updating an existing organizational unit type.
/// </summary>
public class UpdateTipoUnidadOrganizativaRequest
{
    public string Nombre { get; set; } = string.Empty;
}
