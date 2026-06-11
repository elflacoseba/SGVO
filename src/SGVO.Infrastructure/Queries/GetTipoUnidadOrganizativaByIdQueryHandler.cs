using Microsoft.EntityFrameworkCore;
using SGVO.Application.Common;
using SGVO.Application.Features.TiposUnidadOrganizativa.Dtos;
using SGVO.Application.Features.TiposUnidadOrganizativa.Queries;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Queries;

/// <summary>
/// Handler for GetTipoUnidadOrganizativaByIdQuery. Returns a single organizational unit type by ID.
/// </summary>
public class GetTipoUnidadOrganizativaByIdQueryHandler : IQueryHandler<GetTipoUnidadOrganizativaByIdQuery, TipoUnidadOrganizativaDto?>
{
    private readonly SgvoDbContext _dbContext;

    public GetTipoUnidadOrganizativaByIdQueryHandler(SgvoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<TipoUnidadOrganizativaDto?>> Handle(
        GetTipoUnidadOrganizativaByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.TiposUnidadOrganizativa
            .AsNoTracking()
            .Where(e => e.Id == (ulong)request.Id && e.EliminadoEn == null && e.EliminadoPor == null)
            .FirstOrDefaultAsync(cancellationToken);

        if (entity is null)
            return Result<TipoUnidadOrganizativaDto?>.Success(null);

        var dto = new TipoUnidadOrganizativaDto
        {
            Id = (long)entity.Id,
            Nombre = entity.Nombre,
            Activo = entity.Activo ?? false,
            CreadoEn = entity.CreadoEn,
            ModificadoEn = entity.ModificadoEn
        };

        return Result<TipoUnidadOrganizativaDto?>.Success(dto);
    }
}
