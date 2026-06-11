using Microsoft.EntityFrameworkCore;
using SGVO.Application.Common;
using SGVO.Application.Features.UnidadesOrganizativas.Commands;
using SGVO.Application.Features.UnidadesOrganizativas.Queries;
using SGVO.Infrastructure.Persistence;
using SGVO.Infrastructure.Persistence.Entities;
using SGVO.Shared;

namespace SGVO.Infrastructure.Commands;

/// <summary>
/// Handler for CrearUnidadOrganizativaCommand. Creates a new organizational unit.
/// Validates that TipoUnidadOrganizativaId and PadreId reference active records.
/// </summary>
public class CrearUnidadOrganizativaCommandHandler : ICommandHandler<CrearUnidadOrganizativaCommand, UnidadOrganizativaDetailDto>
{
    private readonly SgvoDbContext _dbContext;

    public CrearUnidadOrganizativaCommandHandler(SgvoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<UnidadOrganizativaDetailDto>> Handle(
        CrearUnidadOrganizativaCommand command,
        CancellationToken cancellationToken)
    {
        // Validate TipoUnidadOrganizativaId exists and is active
        var tipo = await _dbContext.TiposUnidadOrganizativa
            .FirstOrDefaultAsync(e => e.Id == (ulong)command.TipoUnidadOrganizativaId && e.EliminadoEn == null, cancellationToken);

        if (tipo is null)
            return Result<UnidadOrganizativaDetailDto>.Failure(
                $"Tipo de unidad organizativa con id {command.TipoUnidadOrganizativaId} no encontrado o está eliminado.",
                "NOT_FOUND");

        // Validate PadreId if provided
        if (command.PadreId.HasValue)
        {
            var padre = await _dbContext.UnidadesOrganizativas
                .FirstOrDefaultAsync(e => e.Id == (ulong)command.PadreId.Value, cancellationToken);

            if (padre is null)
                return Result<UnidadOrganizativaDetailDto>.Failure(
                    $"Unidad organizativa padre con id {command.PadreId.Value} no encontrada.",
                    "NOT_FOUND");

            if (padre.EliminadoEn is not null)
                return Result<UnidadOrganizativaDetailDto>.Failure(
                    $"Unidad organizativa padre con id {command.PadreId.Value} está eliminada.",
                    "VALIDATION");
        }

        var entity = new UnidadesOrganizativaEntity
        {
            Nombre = command.Nombre,
            TipoUnidadOrganizativaId = (ulong)command.TipoUnidadOrganizativaId,
            NivelJerarquico = command.NivelJerarquico,
            PadreId = command.PadreId.HasValue ? (ulong)command.PadreId.Value : null,
            Activo = true,
            CreadoEn = DateTime.UtcNow
        };

        _dbContext.UnidadesOrganizativas.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<UnidadOrganizativaDetailDto>.Success(new UnidadOrganizativaDetailDto
        {
            Id = (long)entity.Id,
            Nombre = entity.Nombre,
            TipoUnidadOrganizativaId = (long)entity.TipoUnidadOrganizativaId,
            TipoNombre = tipo.Nombre,
            NivelJerarquico = entity.NivelJerarquico,
            PadreId = entity.PadreId.HasValue ? (long)entity.PadreId.Value : null,
            Activo = entity.Activo ?? true,
            CreadoEn = entity.CreadoEn,
            ModificadoEn = entity.ModificadoEn,
            ChildrenCount = 0
        });
    }
}
