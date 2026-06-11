namespace SGVO.Application.Features.TiposUnidadOrganizativa.Dtos;

/// <summary>
/// DTO for a single organizational unit type.
/// </summary>
public class TipoUnidadOrganizativaDto
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public DateTime CreadoEn { get; set; }
    public DateTime? ModificadoEn { get; set; }
}
