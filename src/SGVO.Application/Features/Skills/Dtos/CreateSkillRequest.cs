namespace SGVO.Application.Features.Skills.Dtos;

/// <summary>
/// Request DTO for creating a new skill.
/// </summary>
public record CreateSkillRequest(string Nombre, string? Categoria, string? Descripcion);
