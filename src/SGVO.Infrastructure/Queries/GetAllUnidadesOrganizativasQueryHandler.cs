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
            .Where(e => e.EliminadoEn == null && e.EliminadoPor == null);

        if (!string.IsNullOrWhiteSpace(request.Tipo))
            query = query.Where(e => e.Tipo == request.Tipo);

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)normalized.PageSize);

        var items = await query
            .OrderBy(e => e.Id)
            .Skip(normalized.SkipCount)
            .Take(normalized.PageSize)
            .Select(e => new UnidadOrganizativaDto
            {
                Id = (long)e.Id,
                Nombre = e.Nombre,
                Tipo = e.Tipo,
                NivelJerarquico = e.NivelJerarquico,
                PadreId = e.PadreId != null ? (long)e.PadreId : null,
                Activo = e.Activo ?? false
            })
            .ToListAsync(cancellationToken);

        var result = new PagedResult<UnidadOrganizativaDto>(items, totalCount, normalized.Page, normalized.PageSize, totalPages);
        return Result<PagedResult<UnidadOrganizativaDto>>.Success(result);
    }
}
