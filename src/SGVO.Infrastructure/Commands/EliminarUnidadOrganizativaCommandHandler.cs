using Microsoft.EntityFrameworkCore;
using SGVO.Application.Common;
using SGVO.Application.Features.UnidadesOrganizativas.Commands;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Commands;

/// <summary>
/// Handler for EliminarUnidadOrganizativaCommand. Soft-deletes an organizational unit.
/// Rejects if active Puestos reference the unit.
/// </summary>
public class EliminarUnidadOrganizativaCommandHandler : ICommandHandler<EliminarUnidadOrganizativaCommand, Unit>
{
    private readonly SgvoDbContext _dbContext;

    public EliminarUnidadOrganizativaCommandHandler(SgvoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Unit>> Handle(
        EliminarUnidadOrganizativaCommand command,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.UnidadesOrganizativas
            .FirstOrDefaultAsync(e => e.Id == (ulong)command.Id, cancellationToken);

        if (entity is null)
            return Result<Unit>.Failure(
                $"Unidad organizativa con id {command.Id} no encontrada.",
                "NOT_FOUND");

        if (entity.EliminadoEn is not null)
            return Result<Unit>.Failure(
                $"Unidad organizativa con id {command.Id} ya está eliminada.",
                "NOT_FOUND");

        // Guard: check if referenced by active Puestos
        var hasActivePuestos = await _dbContext.Puestos
            .AnyAsync(e => e.UnidadOrganizativaId == entity.Id && e.EliminadoEn == null, cancellationToken);

        if (hasActivePuestos)
            return Result<Unit>.Failure(
                "No se puede eliminar la unidad organizativa porque tiene puestos activos referenciándola.",
                "CONFLICT");

        entity.Activo = false;
        entity.EliminadoEn = DateTime.UtcNow;
        entity.EliminadoPor = command.EliminadoPor;
        entity.ModificadoEn = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}
