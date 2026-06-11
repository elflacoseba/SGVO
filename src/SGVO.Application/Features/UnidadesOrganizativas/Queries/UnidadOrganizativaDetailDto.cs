namespace SGVO.Application.Features.UnidadesOrganizativas.Queries;

/// <summary>
/// DTO for a detailed view of a single organizational unit.
/// </summary>
public class UnidadOrganizativaDetailDto
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public int? NivelJerarquico { get; set; }
    public long? PadreId { get; set; }
    public bool Activo { get; set; }
    public DateTime CreadoEn { get; set; }
    public DateTime? ModificadoEn { get; set; }
    public UnidadOrganizativaParentDto? Padre { get; set; }
    public int ChildrenCount { get; set; }
}

/// <summary>
/// DTO for the parent reference of an organizational unit.
/// </summary>
public class UnidadOrganizativaParentDto
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}
