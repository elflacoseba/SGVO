using Microsoft.EntityFrameworkCore;
using SGVO.Application.Common;
using SGVO.Application.Features.Postulantes.Queries;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Queries;

public sealed class GetAllPostulantesQueryHandler : IQueryHandler<GetAllPostulantesQuery, PagedResult<PostulanteDto>>
{
    private readonly SgvoDbContext _db;

    public GetAllPostulantesQueryHandler(SgvoDbContext db)
    {
        _db = db;
    }

    public async Task<Result<PagedResult<PostulanteDto>>> Handle(
        GetAllPostulantesQuery query,
        CancellationToken cancellationToken = default)
    {
        var page = query.Pagination.Normalize();

        var filtered = _db.Postulantes
            .AsNoTracking()
            .Where(p => p.EliminadoEn == null && p.EliminadoPor == null);

        var totalCount = await filtered.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)page.PageSize);

        var postulantes = await filtered
            .OrderBy(p => p.Id)
            .Skip(page.SkipCount)
            .Take(page.PageSize)
            .Select(p => new PostulanteDto
            {
                Id = (long)p.Id,
                Nombre = p.Nombre,
                Apellido = p.Apellido,
                Email = p.Email,
                Origen = p.Origen
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult<PostulanteDto>>.Success(
            new PagedResult<PostulanteDto>(postulantes, totalCount, page.Page, page.PageSize, totalPages));
    }
}
