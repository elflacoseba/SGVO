using Microsoft.EntityFrameworkCore;
using SGVO.Application.Common;
using SGVO.Application.Features.Skills.Queries;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Queries;

/// <summary>
/// Handler for GetSkillByIdQuery. Returns a single skill by ID.
/// Soft-deleted skills are not returned.
/// </summary>
public class GetSkillByIdQueryHandler : IQueryHandler<GetSkillByIdQuery, SkillDetailDto?>
{
    private readonly SgvoDbContext _dbContext;

    public GetSkillByIdQueryHandler(SgvoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<SkillDetailDto?>> Handle(
        GetSkillByIdQuery request,
        CancellationToken cancellationToken)
    {
        var skill = await _dbContext.Skills
            .AsNoTracking()
            .Where(s => s.Id == request.Id && s.EliminadoEn == null)
            .FirstOrDefaultAsync(cancellationToken);

        if (skill is null)
            return Result<SkillDetailDto?>.Success(null);

        return Result<SkillDetailDto?>.Success(SkillDetailDto.FromEntity(skill));
    }
}
