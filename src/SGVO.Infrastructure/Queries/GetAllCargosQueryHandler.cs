using Microsoft.EntityFrameworkCore;
using SGVO.Application.Common;
using SGVO.Application.Features.Cargos.Queries;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Queries;

/// <summary>
/// Handler para GetAllCargosQuery. Retorna cargos paginados excluyendo soft-deleted.
/// </summary>
public class GetAllCargosQueryHandler : IQueryHandler<GetAllCargosQuery, PagedResult<CargoDto>>
{
    private readonly SgvoDbContext _dbContext;

    public GetAllCargosQueryHandler(SgvoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<PagedResult<CargoDto>>> Handle(
        GetAllCargosQuery request,
        CancellationToken cancellationToken)
    {
        var normalized = request.Pagination.Normalize();

        var query = _dbContext.Cargos
            .AsNoTracking()
            .Where(c => c.EliminadoEn == null && c.EliminadoPor == null);

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)normalized.PageSize);

        var cargos = await query
            .OrderBy(c => c.Id)
            .Skip(normalized.SkipCount)
            .Take(normalized.PageSize)
            .Select(c => new CargoDto
            {
                Id = (long)c.Id,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion
            })
            .ToListAsync(cancellationToken);

        var result = new PagedResult<CargoDto>(cargos, totalCount, normalized.Page, normalized.PageSize, totalPages);
        return Result<PagedResult<CargoDto>>.Success(result);
    }
}
