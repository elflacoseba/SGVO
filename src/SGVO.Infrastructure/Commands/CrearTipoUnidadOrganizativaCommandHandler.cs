using Microsoft.EntityFrameworkCore;
using SGVO.Application.Common;
using SGVO.Application.Features.TiposUnidadOrganizativa.Commands;
using SGVO.Application.Features.TiposUnidadOrganizativa.Dtos;
using SGVO.Infrastructure.Persistence;
using SGVO.Infrastructure.Persistence.Entities;
using SGVO.Shared;

namespace SGVO.Infrastructure.Commands;

/// <summary>
/// Handler for CrearTipoUnidadOrganizativaCommand. Creates a new organizational unit type.
/// </summary>
public class CrearTipoUnidadOrganizativaCommandHandler : ICommandHandler<CrearTipoUnidadOrganizativaCommand, TipoUnidadOrganizativaDto>
{
    private readonly SgvoDbContext _dbContext;

    public CrearTipoUnidadOrganizativaCommandHandler(SgvoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<TipoUnidadOrganizativaDto>> Handle(
        CrearTipoUnidadOrganizativaCommand command,
        CancellationToken cancellationToken)
    {
        // Check uniqueness among active records
        var exists = await _dbContext.TiposUnidadOrganizativa
            .AnyAsync(e => e.Nombre == command.Nombre && e.EliminadoEn == null, cancellationToken);

        if (exists)
            return Result<TipoUnidadOrganizativaDto>.Failure(
                $"Ya existe un tipo de unidad organizativa activo con el nombre '{command.Nombre}'.",
                "CONFLICT");

        var entity = new TipoUnidadOrganizativaEntity
        {
            Nombre = command.Nombre,
            Activo = true,
            CreadoEn = DateTime.UtcNow
        };

        _dbContext.TiposUnidadOrganizativa.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<TipoUnidadOrganizativaDto>.Success(new TipoUnidadOrganizativaDto
        {
            Id = (long)entity.Id,
            Nombre = entity.Nombre,
            Activo = entity.Activo ?? true,
            CreadoEn = entity.CreadoEn,
            ModificadoEn = entity.ModificadoEn
        });
    }
}
