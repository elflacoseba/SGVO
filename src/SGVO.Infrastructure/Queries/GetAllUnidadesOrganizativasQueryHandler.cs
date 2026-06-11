using Microsoft.EntityFrameworkCore;
using SGVO.Application.Common;
using SGVO.Application.Features.UnidadesOrganizativas.Queries;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Queries;

/// <summary>
/// Handler for GetAllUnidadesOrganizativasQuery. Returns paginated active organizational units.
/// </summary>
public class GetAllUnidadesOrganizativasQueryHandler : IQueryHandler<GetAllUnidadesOrganizativasQuery, PagedResult<UnidadOrganizativaDto>>
{
    private readonly SgvoDbContext _dbContext;

    public GetAllUnidadesOrganizativasQueryHandler(SgvoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<PagedResult<UnidadOrganizativaDto>>> Handle(
        GetAllUnidadesOrganizativasQuery request,
        CancellationToken cancellationToken)
    {
        var normalized = request.Pagination.Normalize();

        var query = _dbContext.UnidadesOrganizativas
            .AsNoTracking()
            .Where(e => e.EliminadoEn == null);

        if (request.TipoUnidadOrganizativaId.HasValue)
            query = query.Where(e => e.TipoUnidadOrganizativaId == request.TipoUnidadOrganizativaId.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)normalized.PageSize);

        var items = await query
            .OrderBy(e => e.Id)
            .Skip(normalized.SkipCount)
            .Take(normalized.PageSize)
            .Select(e => new UnidadOrganizativaDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                TipoUnidadOrganizativaId = e.TipoUnidadOrganizativaId,
                TipoNombre = e.TipoUnidadOrganizativa != null ? e.TipoUnidadOrganizativa.Nombre : string.Empty,
                NivelJerarquico = e.NivelJerarquico,
                PadreId = e.PadreId,
                Activo = e.Activo
            })
            .ToListAsync(cancellationToken);

        var result = new PagedResult<UnidadOrganizativaDto>(items, totalCount, normalized.Page, normalized.PageSize, totalPages);
        return Result<PagedResult<UnidadOrganizativaDto>>.Success(result);
    }
}
