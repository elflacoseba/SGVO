using Microsoft.EntityFrameworkCore;
using SGVO.Application.Common;
using SGVO.Application.Features.TiposUnidadOrganizativa.Commands;
using SGVO.Application.Features.TiposUnidadOrganizativa.Dtos;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Commands;

/// <summary>
/// Handler for ActualizarTipoUnidadOrganizativaCommand. Updates an existing organizational unit type.
/// </summary>
public class ActualizarTipoUnidadOrganizativaCommandHandler : ICommandHandler<ActualizarTipoUnidadOrganizativaCommand, TipoUnidadOrganizativaDto>
{
    private readonly SgvoDbContext _dbContext;

    public ActualizarTipoUnidadOrganizativaCommandHandler(SgvoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<TipoUnidadOrganizativaDto>> Handle(
        ActualizarTipoUnidadOrganizativaCommand command,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.TiposUnidadOrganizativa
            .FirstOrDefaultAsync(e => e.Id == command.Id, cancellationToken);

        if (entity is null)
            return Result<TipoUnidadOrganizativaDto>.Failure(
                $"Tipo de unidad organizativa con id {command.Id} no encontrado.",
                "NOT_FOUND");

        if (entity.EliminadoEn is not null)
            return Result<TipoUnidadOrganizativaDto>.Failure(
                $"Tipo de unidad organizativa con id {command.Id} está eliminado.",
                "NOT_FOUND");

        // Check uniqueness among active records, excluding self
        var duplicateExists = await _dbContext.TiposUnidadOrganizativa
            .AnyAsync(e => e.Nombre == command.Nombre && e.Id != entity.Id && e.EliminadoEn == null, cancellationToken);

        if (duplicateExists)
            return Result<TipoUnidadOrganizativaDto>.Failure(
                $"Ya existe un tipo de unidad organizativa activo con el nombre '{command.Nombre}'.",
                "CONFLICT");

        entity.Nombre = command.Nombre;
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
