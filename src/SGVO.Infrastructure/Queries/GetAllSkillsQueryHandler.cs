using Microsoft.EntityFrameworkCore;
using SGVO.Application.Common;
using SGVO.Application.Features.Skills.Queries;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Queries;

public sealed class GetAllSkillsQueryHandler : IQueryHandler<GetAllSkillsQuery, PagedResult<SkillDto>>
{
    private readonly SgvoDbContext _db;

    public GetAllSkillsQueryHandler(SgvoDbContext db)
    {
        _db = db;
    }

    public async Task<Result<PagedResult<SkillDto>>> Handle(
        GetAllSkillsQuery query,
        CancellationToken cancellationToken = default)
    {
        var page = query.Pagination.Normalize();

        var filtered = _db.Skills
            .AsNoTracking()
            .Where(s => s.EliminadoEn == null && s.EliminadoPor == null);

        var totalCount = await filtered.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)page.PageSize);

        var skills = await filtered
            .OrderBy(s => s.Id)
            .Skip(page.SkipCount)
            .Take(page.PageSize)
            .Select(s => new SkillDto
            {
                Id = (long)s.Id,
                Nombre = s.Nombre,
                Categoria = s.Categoria
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult<SkillDto>>.Success(
            new PagedResult<SkillDto>(skills, totalCount, page.Page, page.PageSize, totalPages));
    }
}
