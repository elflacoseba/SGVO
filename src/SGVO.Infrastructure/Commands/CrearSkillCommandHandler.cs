using Microsoft.EntityFrameworkCore;
using SGVO.Application.Common;
using SGVO.Application.Features.Skills.Commands;
using SGVO.Application.Features.Skills.Queries;
using SGVO.Domain.Entities;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Commands;

/// <summary>
/// Handler for CrearSkillCommand. Creates a new skill using domain entity for validation
/// and SkillEntity for persistence.
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
        // Check for duplicate active name
        var duplicateExists = await _dbContext.Skills
            .AnyAsync(s => s.Nombre == command.Nombre && s.Activo == true && s.EliminadoEn == null, cancellationToken);

        if (duplicateExists)
        {
            return Result<SkillDetailDto>.Failure(
                $"Ya existe un skill activo con el nombre '{command.Nombre}'.",
                "CONFLICT");
        }

        // Create domain entity for validation
        var skill = new Skill(command.Nombre, command.Categoria, command.Descripcion);

        // Map to persistence entity
        var entity = new Persistence.Entities.SkillEntity
        {
            Nombre = skill.Nombre,
            Categoria = skill.Categoria,
            Descripcion = skill.Descripcion,
            Activo = true,
            CreadoEn = skill.CreadoEn
        };

        _dbContext.Skills.Add(entity);
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
