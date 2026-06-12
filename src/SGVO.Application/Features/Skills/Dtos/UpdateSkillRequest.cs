namespace SGVO.Application.Features.Skills.Dtos;

/// <summary>
/// Request DTO for updating an existing skill.
/// </summary>
public record UpdateSkillRequest(string Nombre, string? Categoria, string? Descripcion);
