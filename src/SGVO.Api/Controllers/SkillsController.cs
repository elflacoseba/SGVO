using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGVO.Infrastructure.Persistence;

namespace SGVO.Api.Controllers;

[ApiController]
[Route("api/v1/skills")]
public class SkillsController : ControllerBase
{
    private readonly SgvoDbContext _db;

    public SkillsController(SgvoDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    [EndpointName("GetSkills")]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var skills = await _db.Skills
            .AsNoTracking()
            .Select(s => new
            {
                s.Id,
                s.Nombre,
                s.Categoria
            })
            .ToListAsync(ct);

        return Ok(skills);
    }
}
