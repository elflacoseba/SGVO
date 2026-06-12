using Microsoft.EntityFrameworkCore;
using SGVO.Application.Common;
using SGVO.Application.Features.Skills.Commands;
using SGVO.Application.Features.Skills.Queries;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Commands;

/// <summary>
/// Handler for ReactivarSkillCommand. Reactivates a soft-deleted skill.
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
        var entity = await _dbContext.Skills
            .FirstOrDefaultAsync(s => s.Id == command.Id, cancellationToken);

        if (entity is null)
            return Result<SkillDetailDto>.Failure(
                $"Skill con id {command.Id} no encontrado.",
                "NOT_FOUND");

        // Idempotent: if already active, return as-is
        if (entity.EliminadoEn is null && entity.Activo == true)
            return Result<SkillDetailDto>.Success(new SkillDetailDto
            {
                Id = entity.Id,
                Nombre = entity.Nombre,
                Categoria = entity.Categoria,
                Descripcion = entity.Descripcion,
                Activo = entity.Activo ?? false,
                CreadoEn = entity.CreadoEn,
                ModificadoEn = entity.ModificadoEn
            });

        // Reactivate
        entity.EliminadoEn = null;
        entity.EliminadoPor = null;
        entity.Activo = true;
        entity.ModificadoEn = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<SkillDetailDto>.Success(new SkillDetailDto
        {
            Id = entity.Id,
            Nombre = entity.Nombre,
            Categoria = entity.Categoria,
            Descripcion = entity.Descripcion,
            Activo = entity.Activo ?? false,
            CreadoEn = entity.CreadoEn,
            ModificadoEn = entity.ModificadoEn
        });
    }
}
