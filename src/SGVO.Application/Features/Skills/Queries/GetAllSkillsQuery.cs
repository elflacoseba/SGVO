using SGVO.Application.Common;

namespace SGVO.Application.Features.Skills.Queries;

public sealed record GetAllSkillsQuery(PageParameters Pagination) : IQuery<PagedResult<SkillDto>>;
