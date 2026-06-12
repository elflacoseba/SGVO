using Microsoft.EntityFrameworkCore;
using SGVO.Application.Common;
using SGVO.Application.Features.Skills.Commands;
using SGVO.Application.Features.Skills.Queries;
using SGVO.Domain.Entities;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Commands;

/// <summary>
/// Handler for CrearSkillCommand. Creates a new skill using the domain entity directly.
/// </summary>
public class CrearSkillCommandHandler : ICommandHandler<CrearSkillCommand, SkillDetailDto>
{
    private readonly SgvoDbContext _dbContext;

    public CrearSkillCommandHandler(SgvoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<SkillDetailDto>> Handle(
        CrearSkillCommand command,
        CancellationToken cancellationToken)
    {
        var trimmedNombre = command.Nombre.Trim();

        // Check for duplicate active name (trimmed)
        var duplicateExists = await _dbContext.Skills
            .AnyAsync(s => s.Nombre == trimmedNombre && s.Activo && s.EliminadoEn == null, cancellationToken);

        if (duplicateExists)
        {
            return Result<SkillDetailDto>.Failure(
                $"Ya existe un skill activo con el nombre '{trimmedNombre}'.",
                "CONFLICT");
        }

        // Create domain entity (validates and trims internally)
        var skill = new Skill(trimmedNombre, command.Categoria?.Trim(), command.Descripcion?.Trim());

        _dbContext.Skills.Add(skill);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<SkillDetailDto>.Success(SkillDetailDto.FromEntity(skill));
    }
}
