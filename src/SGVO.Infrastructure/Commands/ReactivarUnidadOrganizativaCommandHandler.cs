using Microsoft.EntityFrameworkCore;
using SGVO.Application.Common;
using SGVO.Application.Features.UnidadesOrganizativas.Commands;
using SGVO.Application.Features.UnidadesOrganizativas.Queries;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Commands;

/// <summary>
/// Handler for ReactivarUnidadOrganizativaCommand. Reactivates a soft-deleted organizational unit.
/// Rejects if parent is still soft-deleted or TipoUnidadOrganizativaId is inactive.
/// </summary>
public class ReactivarUnidadOrganizativaCommandHandler : ICommandHandler<ReactivarUnidadOrganizativaCommand, UnidadOrganizativaDetailDto>
{
    private readonly SgvoDbContext _dbContext;

    public ReactivarUnidadOrganizativaCommandHandler(SgvoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<UnidadOrganizativaDetailDto>> Handle(
        ReactivarUnidadOrganizativaCommand command,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.UnidadesOrganizativas
            .FirstOrDefaultAsync(e => e.Id == command.Id, cancellationToken);

        if (entity is null)
            return Result<UnidadOrganizativaDetailDto>.Failure(
                $"Unidad organizativa con id {command.Id} no encontrada.",
                "NOT_FOUND");

        if (entity.EliminadoEn is null)
            return Result<UnidadOrganizativaDetailDto>.Failure(
                $"Unidad organizativa con id {command.Id} ya está activa.",
                "CONFLICT");

        // Validate TipoUnidadOrganizativaId is still active
        var tipo = await _dbContext.TiposUnidadOrganizativa
            .FirstOrDefaultAsync(e => e.Id == entity.TipoUnidadOrganizativaId && e.EliminadoEn == null, cancellationToken);

        if (tipo is null)
            return Result<UnidadOrganizativaDetailDto>.Failure(
                $"Tipo de unidad organizativa con id {entity.TipoUnidadOrganizativaId} no encontrado o está eliminado.",
                "VALIDATION");

        // Validate parent is still active (if parent exists)
        if (entity.PadreId.HasValue)
        {
            var padre = await _dbContext.UnidadesOrganizativas
                .FirstOrDefaultAsync(e => e.Id == entity.PadreId.Value, cancellationToken);

            if (padre is null)
                return Result<UnidadOrganizativaDetailDto>.Failure(
                    $"Unidad organizativa padre con id {entity.PadreId.Value} no encontrada.",
                    "NOT_FOUND");

            if (padre.EliminadoEn is not null)
                return Result<UnidadOrganizativaDetailDto>.Failure(
                    $"No se puede reactivar la unidad organizativa porque su padre (id {entity.PadreId.Value}) está eliminado.",
                    "VALIDATION");
        }

        entity.Activo = true;
        entity.EliminadoEn = null;
        entity.EliminadoPor = null;
        entity.ModificadoEn = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        // Load parent info for response
        string? padreNombre = null;
        if (entity.PadreId.HasValue)
        {
            padreNombre = await _dbContext.UnidadesOrganizativas
                .Where(e => e.Id == entity.PadreId.Value)
                .Select(e => e.Nombre)
                .FirstOrDefaultAsync(cancellationToken);
        }

        var childrenCount = await _dbContext.UnidadesOrganizativas
            .CountAsync(e => e.PadreId == entity.Id && e.EliminadoEn == null, cancellationToken);

        return Result<UnidadOrganizativaDetailDto>.Success(new UnidadOrganizativaDetailDto
        {
            Id = entity.Id,
            Nombre = entity.Nombre,
            TipoUnidadOrganizativaId = entity.TipoUnidadOrganizativaId,
            TipoNombre = tipo.Nombre,
            NivelJerarquico = entity.NivelJerarquico,
            PadreId = entity.PadreId,
            Activo = entity.Activo,
            CreadoEn = entity.CreadoEn,
            ModificadoEn = entity.ModificadoEn,
            Padre = entity.PadreId.HasValue && padreNombre is not null
                ? new UnidadOrganizativaParentDto { Id = entity.PadreId.Value, Nombre = padreNombre }
                : null,
            ChildrenCount = childrenCount
        });
    }
}
