using SGVO.Application.Common;
using SGVO.Application.Features.Skills.Queries;
using SGVO.Shared;

namespace SGVO.Application.Features.Skills.Commands;

/// <summary>
/// Command to create a new skill.
/// </summary>
public sealed record CrearSkillCommand(string Nombre, string? Categoria, string? Descripcion) : ICommand<SkillDetailDto>;

/// <summary>
/// Command to update an existing skill.
/// </summary>
public sealed record ActualizarSkillCommand(long Id, string Nombre, string? Categoria, string? Descripcion) : ICommand<SkillDetailDto>;

/// <summary>
/// Command to soft-delete a skill.
/// </summary>
public sealed record EliminarSkillCommand(long Id, long EliminadoPor) : ICommand<Unit>;

/// <summary>
/// Command to reactivate a soft-deleted skill.
/// </summary>
public sealed record ReactivarSkillCommand(long Id) : ICommand<SkillDetailDto>;
