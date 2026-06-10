namespace SGVO.Application.Features.Cargos.Queries;

/// <summary>
/// DTO de respuesta para un cargo.
/// </summary>
public class CargoDto
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}
