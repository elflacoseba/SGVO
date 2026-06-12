using SGVO.Domain.Entities;

namespace SGVO.Application.Features.Skills.Queries;

/// <summary>
/// DTO for single skill detail response.
/// </summary>
public class SkillDetailDto
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Categoria { get; set; }
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
    public DateTime CreadoEn { get; set; }
    public DateTime? ModificadoEn { get; set; }

    /// <summary>
    /// Creates a SkillDetailDto from a Skill domain entity.
    /// </summary>
    public static SkillDetailDto FromEntity(Skill skill) => new()
    {
        Id = skill.Id,
        Nombre = skill.Nombre,
        Categoria = skill.Categoria,
        Descripcion = skill.Descripcion,
        Activo = skill.Activo,
        CreadoEn = skill.CreadoEn,
        ModificadoEn = skill.ModificadoEn
    };
}
