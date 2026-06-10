using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGVO.Infrastructure.Persistence;

namespace SGVO.Api.Controllers;

[ApiController]
[Route("api/v1/cargos")]
public class CargosController : ControllerBase
{
    private readonly SgvoDbContext _db;

    public CargosController(SgvoDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    [EndpointName("GetCargos")]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var cargos = await _db.Cargos
            .AsNoTracking()
            .Select(c => new
            {
                c.Id,
                c.Nombre,
                c.Descripcion
            })
            .ToListAsync(ct);

        return Ok(cargos);
    }
}
