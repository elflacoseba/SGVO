using Microsoft.EntityFrameworkCore;
using SGVO.Application.Common;
using SGVO.Application.Features.Skills.Queries;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Queries;

/// <summary>
/// Handler para GetAllSkillsQuery. Retorna skills paginados excluyendo soft-deleted.
/// </summary>
public class GetAllSkillsQueryHandler : IQueryHandler<GetAllSkillsQuery, PagedResult<SkillDto>>
{
    private readonly SgvoDbContext _dbContext;

    public GetAllSkillsQueryHandler(SgvoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<PagedResult<SkillDto>>> Handle(
        GetAllSkillsQuery request,
        CancellationToken cancellationToken)
    {
        var normalized = request.Pagination.Normalize();

        var query = _dbContext.Skills
            .AsNoTracking()
            .Where(s => s.EliminadoEn == null && s.EliminadoPor == null);

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)normalized.PageSize);

        var skills = await query
            .OrderBy(s => s.Id)
            .Skip(normalized.SkipCount)
            .Take(normalized.PageSize)
            .Select(s => new SkillDto
            {
                Id = (long)s.Id,
                Nombre = s.Nombre,
                Categoria = s.Categoria
            })
            .ToListAsync(cancellationToken);

        var result = new PagedResult<SkillDto>(skills, totalCount, normalized.Page, normalized.PageSize, totalPages);
        return Result<PagedResult<SkillDto>>.Success(result);
    }
}
