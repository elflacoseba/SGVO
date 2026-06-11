using Microsoft.EntityFrameworkCore;
using SGVO.Application.Common;
using SGVO.Application.Features.TiposUnidadOrganizativa.Commands;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Commands;

/// <summary>
/// Handler for EliminarTipoUnidadOrganizativaCommand. Soft-deletes an organizational unit type.
/// Rejects if the type is referenced by active UnidadesOrganizativas.
/// </summary>
public class EliminarTipoUnidadOrganizativaCommandHandler : ICommandHandler<EliminarTipoUnidadOrganizativaCommand, Unit>
{
    private readonly SgvoDbContext _dbContext;

    public EliminarTipoUnidadOrganizativaCommandHandler(SgvoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Unit>> Handle(
        EliminarTipoUnidadOrganizativaCommand command,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.TiposUnidadOrganizativa
            .FirstOrDefaultAsync(e => e.Id == (ulong)command.Id, cancellationToken);

        if (entity is null)
            return Result<Unit>.Failure(
                $"Tipo de unidad organizativa con id {command.Id} no encontrado.",
                "NOT_FOUND");

        if (entity.EliminadoEn is not null)
            return Result<Unit>.Failure(
                $"Tipo de unidad organizativa con id {command.Id} ya está eliminado.",
                "NOT_FOUND");

        // Guard: check if referenced by active UnidadesOrganizativas
        var hasActiveReferences = await _dbContext.UnidadesOrganizativas
            .AnyAsync(e => e.TipoUnidadOrganizativaId == entity.Id && e.EliminadoEn == null, cancellationToken);

        if (hasActiveReferences)
            return Result<Unit>.Failure(
                "No se puede eliminar el tipo porque está referenciado por unidades organizativas activas.",
                "CONFLICT");

        entity.Activo = false;
        entity.EliminadoEn = DateTime.UtcNow;
        entity.EliminadoPor = command.EliminadoPor;
        entity.ModificadoEn = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}
