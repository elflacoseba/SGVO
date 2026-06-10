using Microsoft.EntityFrameworkCore;
using SGVO.Application.Common;
using SGVO.Application.Features.Vacantes.Queries;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Queries;

/// <summary>
/// Handler para GetAllVacantesQuery. Retorna vacantes paginadas excluyendo soft-deleted.
/// </summary>
public class GetAllVacantesQueryHandler : IQueryHandler<GetAllVacantesQuery, PagedResult<VacanteDto>>
{
    private readonly SgvoDbContext _dbContext;

    public GetAllVacantesQueryHandler(SgvoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<PagedResult<VacanteDto>>> Handle(
        GetAllVacantesQuery request,
        CancellationToken cancellationToken)
    {
        var normalized = request.Pagination.Normalize();

        var query = _dbContext.Vacantes
            .AsNoTracking()
            .Where(v => v.EliminadoEn == null && v.EliminadoPor == null);

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)normalized.PageSize);

        var vacantes = await query
            .OrderBy(v => v.Id)
            .Skip(normalized.SkipCount)
            .Take(normalized.PageSize)
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

        var result = new PagedResult<VacanteDto>(vacantes, totalCount, normalized.Page, normalized.PageSize, totalPages);
        return Result<PagedResult<VacanteDto>>.Success(result);
    }
}
