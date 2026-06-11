using Microsoft.EntityFrameworkCore;
using SGVO.Application.Common;
using SGVO.Application.Features.TiposUnidadOrganizativa.Dtos;
using SGVO.Application.Features.TiposUnidadOrganizativa.Queries;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Queries;

/// <summary>
/// Handler for GetAllTiposUnidadOrganizativaQuery. Returns paginated active organizational unit types.
/// </summary>
public class GetAllTiposUnidadOrganizativaQueryHandler : IQueryHandler<GetAllTiposUnidadOrganizativaQuery, PagedResult<TipoUnidadOrganizativaDto>>
{
    private readonly SgvoDbContext _dbContext;

    public GetAllTiposUnidadOrganizativaQueryHandler(SgvoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<PagedResult<TipoUnidadOrganizativaDto>>> Handle(
        GetAllTiposUnidadOrganizativaQuery request,
        CancellationToken cancellationToken)
    {
        var normalized = request.Pagination.Normalize();

        var query = _dbContext.TiposUnidadOrganizativa
            .AsNoTracking()
            .Where(e => e.EliminadoEn == null && e.EliminadoPor == null);

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)normalized.PageSize);

        var items = await query
            .OrderBy(e => e.Id)
            .Skip(normalized.SkipCount)
            .Take(normalized.PageSize)
            .Select(e => new TipoUnidadOrganizativaDto
            {
                Id = (long)e.Id,
                Nombre = e.Nombre,
                Activo = e.Activo ?? false,
                CreadoEn = e.CreadoEn,
                ModificadoEn = e.ModificadoEn
            })
            .ToListAsync(cancellationToken);

        var result = new PagedResult<TipoUnidadOrganizativaDto>(items, totalCount, normalized.Page, normalized.PageSize, totalPages);
        return Result<PagedResult<TipoUnidadOrganizativaDto>>.Success(result);
    }
}
