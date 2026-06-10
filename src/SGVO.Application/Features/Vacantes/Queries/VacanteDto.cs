namespace SGVO.Application.Features.Vacantes.Queries;

/// <summary>
/// DTO de respuesta para una vacante.
/// </summary>
public class VacanteDto
{
    public long Id { get; set; }
    public long PuestoId { get; set; }
    public string PuestoNombre { get; set; } = string.Empty;
    public DateTime FechaApertura { get; set; }
    public DateTime? FechaCierre { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
}
