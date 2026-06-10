using Microsoft.EntityFrameworkCore;
using SGVO.Application.Common;
using SGVO.Application.Features.Skills.Queries;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Queries;

public sealed class GetAllSkillsQueryHandler : IQueryHandler<GetAllSkillsQuery, IReadOnlyList<SkillDto>>
{
    private readonly SgvoDbContext _db;

    public GetAllSkillsQueryHandler(SgvoDbContext db)
    {
        _db = db;
    }

    public async Task<Result<IReadOnlyList<SkillDto>>> Handle(
        GetAllSkillsQuery query,
        CancellationToken cancellationToken = default)
    {
        var skills = await _db.Skills
            .AsNoTracking()
            .Where(s => s.EliminadoEn == null && s.EliminadoPor == null)
            .Select(s => new SkillDto
            {
                Id = (long)s.Id,
                Nombre = s.Nombre,
                Categoria = s.Categoria
            })
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<SkillDto>>.Success(skills);
    }
}
