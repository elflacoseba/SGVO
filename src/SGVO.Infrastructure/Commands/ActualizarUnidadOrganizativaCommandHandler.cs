using Microsoft.EntityFrameworkCore;
using SGVO.Application.Common;
using SGVO.Application.Features.UnidadesOrganizativas.Commands;
using SGVO.Application.Features.UnidadesOrganizativas.Queries;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Commands;

/// <summary>
/// Handler for ActualizarUnidadOrganizativaCommand. Updates an existing organizational unit.
/// Includes circular hierarchy detection when PadreId changes.
/// </summary>
public class ActualizarUnidadOrganizativaCommandHandler : ICommandHandler<ActualizarUnidadOrganizativaCommand, UnidadOrganizativaDetailDto>
{
    private readonly SgvoDbContext _dbContext;

    public ActualizarUnidadOrganizativaCommandHandler(SgvoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<UnidadOrganizativaDetailDto>> Handle(
        ActualizarUnidadOrganizativaCommand command,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.UnidadesOrganizativas
            .FirstOrDefaultAsync(e => e.Id == command.Id, cancellationToken);

        if (entity is null)
            return Result<UnidadOrganizativaDetailDto>.Failure(
                $"Unidad organizativa con id {command.Id} no encontrada.",
                "NOT_FOUND");

        if (entity.EliminadoEn is not null)
            return Result<UnidadOrganizativaDetailDto>.Failure(
                $"Unidad organizativa con id {command.Id} está eliminada.",
                "NOT_FOUND");

        // Validate TipoUnidadOrganizativaId exists and is active
        var tipo = await _dbContext.TiposUnidadOrganizativa
            .FirstOrDefaultAsync(e => e.Id == command.TipoUnidadOrganizativaId && e.EliminadoEn == null, cancellationToken);

        if (tipo is null)
            return Result<UnidadOrganizativaDetailDto>.Failure(
                $"Tipo de unidad organizativa con id {command.TipoUnidadOrganizativaId} no encontrado o está eliminado.",
                "NOT_FOUND");

        // Validate PadreId if provided
        if (command.PadreId.HasValue)
        {
            // Cannot set parent to self
            if (command.PadreId.Value == command.Id)
                return Result<UnidadOrganizativaDetailDto>.Failure(
                    "Una unidad organizativa no puede ser padre de sí misma.",
                    "CONFLICT");

            var padre = await _dbContext.UnidadesOrganizativas
                .FirstOrDefaultAsync(e => e.Id == command.PadreId.Value, cancellationToken);

            if (padre is null)
                return Result<UnidadOrganizativaDetailDto>.Failure(
                    $"Unidad organizativa padre con id {command.PadreId.Value} no encontrada.",
                    "NOT_FOUND");

            if (padre.EliminadoEn is not null)
                return Result<UnidadOrganizativaDetailDto>.Failure(
                    $"Unidad organizativa padre con id {command.PadreId.Value} está eliminada.",
                    "VALIDATION");

            // Circular hierarchy detection: walk ancestors from proposed parent
            // If we reach command.Id, it would create a cycle
            var hasCycle = await HasCircularHierarchyAsync(
                command.PadreId.Value, command.Id, cancellationToken);

            if (hasCycle)
                return Result<UnidadOrganizativaDetailDto>.Failure(
                    "La asignación del padre crearía un ciclo en la jerarquía.",
                    "CONFLICT");
        }

        entity.Nombre = command.Nombre;
        entity.TipoUnidadOrganizativaId = command.TipoUnidadOrganizativaId;
        entity.NivelJerarquico = command.NivelJerarquico;
        entity.PadreId = command.PadreId;
        entity.ModificadoEn = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

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
            ModificadoEn = entity.ModificadoEn
        });
    }

    /// <summary>
    /// Walks the ancestor chain from startAncestorId upward.
    /// Returns true if targetId is found in the chain (meaning a cycle would form).
    /// </summary>
    private async Task<bool> HasCircularHierarchyAsync(
        long startAncestorId, long targetId, CancellationToken cancellationToken)
    {
        var currentId = startAncestorId;
        var visited = new HashSet<long>();

        while (currentId != 0)
        {
            if (currentId == targetId)
                return true;

            if (!visited.Add(currentId))
                return true; // Already visited = existing cycle (shouldn't happen, but defensive)

            var parent = await _dbContext.UnidadesOrganizativas
                .Where(e => e.Id == currentId)
                .Select(e => new { e.PadreId })
                .FirstOrDefaultAsync(cancellationToken);

            if (parent is null || parent.PadreId is null)
                break;

            currentId = parent.PadreId.Value;
        }

        return false;
    }
}
