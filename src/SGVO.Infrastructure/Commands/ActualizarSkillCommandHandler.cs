using Microsoft.EntityFrameworkCore;
using SGVO.Application.Common;
using SGVO.Application.Features.Skills.Commands;
using SGVO.Application.Features.Skills.Queries;
using SGVO.Domain.Entities;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Commands;

/// <summary>
/// Handler for ActualizarSkillCommand. Updates an existing skill using domain entity
/// for validation and SkillEntity for persistence.
/// </summary>
public class ActualizarSkillCommandHandler : ICommandHandler<ActualizarSkillCommand, SkillDetailDto>
{
    private readonly SgvoDbContext _dbContext;

    public ActualizarSkillCommandHandler(SgvoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<SkillDetailDto>> Handle(
        ActualizarSkillCommand command,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Skills
            .FirstOrDefaultAsync(s => s.Id == command.Id, cancellationToken);

        if (entity is null)
            return Result<SkillDetailDto>.Failure(
                $"Skill con id {command.Id} no encontrado.",
                "NOT_FOUND");

        // Check if already soft-deleted
        if (entity.EliminadoEn.HasValue)
            return Result<SkillDetailDto>.Failure(
                $"Skill con id {command.Id} está eliminado.",
                "NOT_FOUND");

        // Create domain entity for validation
        var skill = new Skill(entity.Nombre, entity.Categoria, entity.Descripcion);
        skill.Actualizar(command.Nombre, command.Categoria, command.Descripcion);

        // Update persistence entity
        entity.Nombre = skill.Nombre;
        entity.Categoria = skill.Categoria;
        entity.Descripcion = skill.Descripcion;
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
