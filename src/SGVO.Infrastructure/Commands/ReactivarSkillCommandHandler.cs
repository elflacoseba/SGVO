using Microsoft.EntityFrameworkCore;
using SGVO.Application.Common;
using SGVO.Application.Features.Skills.Commands;
using SGVO.Application.Features.Skills.Queries;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Commands;

/// <summary>
/// Handler for ReactivarSkillCommand. Reactivates a soft-deleted skill using the domain entity.
/// Idempotent: if already active, returns the skill without changes.
/// </summary>
public class ReactivarSkillCommandHandler : ICommandHandler<ReactivarSkillCommand, SkillDetailDto>
{
    private readonly SgvoDbContext _dbContext;

    public ReactivarSkillCommandHandler(SgvoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<SkillDetailDto>> Handle(
        ReactivarSkillCommand command,
        CancellationToken cancellationToken)
    {
        var skill = await _dbContext.Skills
            .FirstOrDefaultAsync(s => s.Id == command.Id, cancellationToken);

        if (skill is null)
            return Result<SkillDetailDto>.Failure(
                $"Skill con id {command.Id} no encontrado.",
                "NOT_FOUND");

        // Idempotent: if already active, return as-is
        if (skill.EliminadoEn is null && skill.Activo)
            return Result<SkillDetailDto>.Success(SkillDetailDto.FromEntity(skill));

        // Reactivate via domain method
        skill.Reactivar();

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<SkillDetailDto>.Success(SkillDetailDto.FromEntity(skill));
    }
}
