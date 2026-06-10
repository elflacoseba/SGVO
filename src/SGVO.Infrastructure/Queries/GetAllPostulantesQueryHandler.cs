using Microsoft.EntityFrameworkCore;
using SGVO.Application.Common;
using SGVO.Application.Features.Postulantes.Queries;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Queries;

/// <summary>
/// Handler para GetAllPostulantesQuery. Retorna postulantes paginados excluyendo soft-deleted.
/// </summary>
public class GetAllPostulantesQueryHandler : IQueryHandler<GetAllPostulantesQuery, PagedResult<PostulanteDto>>
{
    private readonly SgvoDbContext _dbContext;

    public GetAllPostulantesQueryHandler(SgvoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<PagedResult<PostulanteDto>>> Handle(
        GetAllPostulantesQuery request,
        CancellationToken cancellationToken)
    {
        var normalized = request.Pagination.Normalize();

        var query = _dbContext.Postulantes
            .AsNoTracking()
            .Where(p => p.EliminadoEn == null && p.EliminadoPor == null);

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)normalized.PageSize);

        var postulantes = await query
            .OrderBy(p => p.Id)
            .Skip(normalized.SkipCount)
            .Take(normalized.PageSize)
            .Select(p => new PostulanteDto
            {
                Id = (long)p.Id,
                Nombre = p.Nombre,
                Apellido = p.Apellido,
                Email = p.Email,
                Origen = p.Origen
            })
            .ToListAsync(cancellationToken);

        var result = new PagedResult<PostulanteDto>(postulantes, totalCount, normalized.Page, normalized.PageSize, totalPages);
        return Result<PagedResult<PostulanteDto>>.Success(result);
    }
}
