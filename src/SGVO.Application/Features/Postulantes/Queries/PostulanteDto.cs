namespace SGVO.Application.Features.Postulantes.Queries;

/// <summary>
/// DTO de respuesta para un postulante.
/// </summary>
public class PostulanteDto
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Origen { get; set; }
}
