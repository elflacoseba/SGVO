using Microsoft.EntityFrameworkCore;
using SGVO.Application.Common;
using SGVO.Application.Features.Vacantes.Queries;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Queries;

public sealed class GetAllVacantesQueryHandler : IQueryHandler<GetAllVacantesQuery, PagedResult<VacanteDto>>
{
    private readonly SgvoDbContext _db;

    public GetAllVacantesQueryHandler(SgvoDbContext db)
    {
        _db = db;
    }

    public async Task<Result<PagedResult<VacanteDto>>> Handle(
        GetAllVacantesQuery query,
        CancellationToken cancellationToken = default)
    {
        var page = query.Pagination.Normalize();

        var filtered = _db.Vacantes
            .AsNoTracking()
            .Where(v => v.EliminadoEn == null && v.EliminadoPor == null);

        var totalCount = await filtered.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)page.PageSize);

        var vacantes = await filtered
            .OrderBy(v => v.Id)
            .Skip(page.SkipCount)
            .Take(page.PageSize)
            .Select(v => new VacanteDto
            {
                Id = (long)v.Id,
                PuestoId = (long)v.PuestoId,
                FechaApertura = v.FechaApertura,
                FechaCierre = v.FechaCierre,
                Motivo = v.Motivo,
                Estado = v.Estado
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult<VacanteDto>>.Success(
            new PagedResult<VacanteDto>(vacantes, totalCount, page.Page, page.PageSize, totalPages));
    }
}
