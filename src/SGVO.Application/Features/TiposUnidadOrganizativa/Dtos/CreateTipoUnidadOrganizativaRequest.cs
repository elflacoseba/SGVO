namespace SGVO.Application.Features.TiposUnidadOrganizativa.Dtos;

/// <summary>
/// Request DTO for creating a new organizational unit type.
/// </summary>
public class CreateTipoUnidadOrganizativaRequest
{
    public string Nombre { get; set; } = string.Empty;
}
