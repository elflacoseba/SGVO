using Microsoft.EntityFrameworkCore;
using SGVO.Application.Common;
using SGVO.Application.Features.Skills.Commands;
using SGVO.Application.Features.Skills.Queries;
using SGVO.Domain.Entities;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Commands;

/// <summary>
/// Handler for ActualizarSkillCommand. Updates an existing skill using the domain entity directly.
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
        var skill = await _dbContext.Skills
            .FirstOrDefaultAsync(s => s.Id == command.Id, cancellationToken);

        if (skill is null)
            return Result<SkillDetailDto>.Failure(
                $"Skill con id {command.Id} no encontrado.",
                "NOT_FOUND");

        // Check if already soft-deleted
        if (skill.EliminadoEn.HasValue)
            return Result<SkillDetailDto>.Failure(
                $"Skill con id {command.Id} está eliminado.",
                "NOT_FOUND");

        // Check for duplicate active name (exclude self)
        var trimmedNombre = command.Nombre.Trim();
        var duplicateExists = await _dbContext.Skills
            .AnyAsync(s => s.Nombre == trimmedNombre && s.Id != command.Id && s.Activo && s.EliminadoEn == null, cancellationToken);

        if (duplicateExists)
        {
            return Result<SkillDetailDto>.Failure(
                $"Ya existe otro skill activo con el nombre '{trimmedNombre}'.",
                "CONFLICT");
        }

        // Domain method validates and trims internally
        skill.Actualizar(trimmedNombre, command.Categoria?.Trim(), command.Descripcion?.Trim());

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<SkillDetailDto>.Success(SkillDetailDto.FromEntity(skill));
    }
}
