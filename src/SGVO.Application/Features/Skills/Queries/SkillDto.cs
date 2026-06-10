namespace SGVO.Application.Features.Skills.Queries;

/// <summary>
/// DTO de respuesta para un skill.
/// </summary>
public class SkillDto
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Categoria { get; set; }
}
