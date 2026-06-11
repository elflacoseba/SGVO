using Microsoft.EntityFrameworkCore;
using SGVO.Application.Common;
using SGVO.Application.Features.TiposUnidadOrganizativa.Commands;
using SGVO.Application.Features.TiposUnidadOrganizativa.Dtos;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Commands;

/// <summary>
/// Handler for ReactivarTipoUnidadOrganizativaCommand. Reactivates a soft-deleted organizational unit type.
/// Rejects if the name conflicts with another active type.
/// </summary>
public class ReactivarTipoUnidadOrganizativaCommandHandler : ICommandHandler<ReactivarTipoUnidadOrganizativaCommand, TipoUnidadOrganizativaDto>
{
    private readonly SgvoDbContext _dbContext;

    public ReactivarTipoUnidadOrganizativaCommandHandler(SgvoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<TipoUnidadOrganizativaDto>> Handle(
        ReactivarTipoUnidadOrganizativaCommand command,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.TiposUnidadOrganizativa
            .FirstOrDefaultAsync(e => e.Id == command.Id, cancellationToken);

        if (entity is null)
            return Result<TipoUnidadOrganizativaDto>.Failure(
                $"Tipo de unidad organizativa con id {command.Id} no encontrado.",
                "NOT_FOUND");

        if (entity.EliminadoEn is null)
            return Result<TipoUnidadOrganizativaDto>.Failure(
                $"Tipo de unidad organizativa con id {command.Id} ya está activo.",
                "CONFLICT");

        // Check name conflict with active records
        var nameConflict = await _dbContext.TiposUnidadOrganizativa
            .AnyAsync(e => e.Nombre == entity.Nombre && e.Id != entity.Id && e.EliminadoEn == null, cancellationToken);

        if (nameConflict)
            return Result<TipoUnidadOrganizativaDto>.Failure(
                $"Ya existe un tipo de unidad organizativa activo con el nombre '{entity.Nombre}'.",
                "CONFLICT");

        entity.Activo = true;
        entity.EliminadoEn = null;
        entity.EliminadoPor = null;
        entity.ModificadoEn = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<TipoUnidadOrganizativaDto>.Success(new TipoUnidadOrganizativaDto
        {
            Id = entity.Id,
            Nombre = entity.Nombre,
            Activo = entity.Activo,
            CreadoEn = entity.CreadoEn,
            ModificadoEn = entity.ModificadoEn
        });
    }
}
