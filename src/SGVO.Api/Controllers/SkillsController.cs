using Microsoft.AspNetCore.Mvc;
using SGVO.Application.Common;
using SGVO.Application.Features.Skills.Queries;

namespace SGVO.Api.Controllers;

[ApiController]
[Route("api/v1/skills")]
public class SkillsController : ControllerBase
{
    private readonly IQueryHandler<GetAllSkillsQuery, IReadOnlyList<SkillDto>> _getAll;

    public SkillsController(IQueryHandler<GetAllSkillsQuery, IReadOnlyList<SkillDto>> getAll)
    {
        _getAll = getAll;
    }

    [HttpGet]
    [EndpointName("GetSkills")]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _getAll.Handle(new GetAllSkillsQuery(), ct);

        if (result.IsFailure)
            return StatusCode(500, new { error = result.Error });

        return Ok(result.Value);
    }
}
