using Microsoft.AspNetCore.Mvc;
using SGVO.Application.Common;
using SGVO.Application.Features.Skills.Queries;

namespace SGVO.Api.Controllers;

[Route("api/v1/skills")]
public class SkillsController : BaseReadOnlyController<GetAllSkillsQuery, SkillDto>
{
    public SkillsController(IQueryHandler<GetAllSkillsQuery, PagedResult<SkillDto>> getAll)
        : base(getAll)
    {
    }

    protected override GetAllSkillsQuery CreateQuery(PageParameters pagination)
    {
        return new GetAllSkillsQuery(pagination);
    }
}
