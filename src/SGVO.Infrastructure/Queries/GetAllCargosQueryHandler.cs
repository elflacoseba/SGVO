using Microsoft.EntityFrameworkCore;
using SGVO.Application.Common;
using SGVO.Application.Features.Cargos.Queries;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Queries;

public sealed class GetAllCargosQueryHandler : IQueryHandler<GetAllCargosQuery, PagedResult<CargoDto>>
{
    private readonly SgvoDbContext _db;

    public GetAllCargosQueryHandler(SgvoDbContext db)
    {
        _db = db;
    }

    public async Task<Result<PagedResult<CargoDto>>> Handle(
        GetAllCargosQuery query,
        CancellationToken cancellationToken = default)
    {
        var page = query.Pagination.Normalize();

        var filtered = _db.Cargos
            .AsNoTracking()
            .Where(c => c.EliminadoEn == null && c.EliminadoPor == null);

        var totalCount = await filtered.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)page.PageSize);

        var cargos = await filtered
            .OrderBy(c => c.Id)
            .Skip(page.SkipCount)
            .Take(page.PageSize)
            .Select(c => new CargoDto
            {
                Id = (long)c.Id,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult<CargoDto>>.Success(
            new PagedResult<CargoDto>(cargos, totalCount, page.Page, page.PageSize, totalPages));
    }
}
