using Microsoft.EntityFrameworkCore;
using SGVO.Application.Common;
using SGVO.Application.Features.Skills.Commands;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Commands;

/// <summary>
/// Handler for EliminarSkillCommand. Soft-deletes a skill using domain entity method.
/// Blocks deletion if active CargoSkills or active PersonaSkills reference the skill.
/// </summary>
public class EliminarSkillCommandHandler : ICommandHandler<EliminarSkillCommand, Unit>
{
    private readonly SgvoDbContext _dbContext;

    public EliminarSkillCommandHandler(SgvoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Unit>> Handle(
        EliminarSkillCommand command,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Skills
            .FirstOrDefaultAsync(s => s.Id == command.Id, cancellationToken);

        if (entity is null)
            return Result<Unit>.Failure(
                $"Skill con id {command.Id} no encontrado.",
                "NOT_FOUND");

        // Check if already soft-deleted
        if (entity.EliminadoEn.HasValue)
            return Result<Unit>.Failure(
                $"Skill con id {command.Id} ya está eliminado.",
                "NOT_FOUND");

        // Guard: check for active CargoSkills referencing this skill
        var hasActiveCargoSkills = await _dbContext.CargoSkills
            .AnyAsync(cs => cs.SkillId == command.Id && cs.EliminadoEn == null, cancellationToken);

        if (hasActiveCargoSkills)
            return Result<Unit>.Failure(
                "No se puede eliminar: tiene relaciones activas con Cargos.",
                "CONFLICT");

        // Guard: check for active PersonaSkills referencing this skill
        var hasActivePersonaSkills = await _dbContext.PersonaSkills
            .AnyAsync(ps => ps.SkillId == command.Id && ps.EliminadoEn == null, cancellationToken);

        if (hasActivePersonaSkills)
            return Result<Unit>.Failure(
                "No se puede eliminar: tiene relaciones activas con Personas.",
                "CONFLICT");

        // Apply soft delete
        entity.EliminadoEn = DateTime.UtcNow;
        entity.EliminadoPor = command.EliminadoPor;
        entity.Activo = false;
        entity.ModificadoEn = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}
