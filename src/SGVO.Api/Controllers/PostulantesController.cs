using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGVO.Infrastructure.Persistence;

namespace SGVO.Api.Controllers;

[ApiController]
[Route("api/v1/postulantes")]
public class PostulantesController : ControllerBase
{
    private readonly SgvoDbContext _db;

    public PostulantesController(SgvoDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    [EndpointName("GetPostulantes")]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var postulantes = await _db.Postulantes
            .AsNoTracking()
            .Select(p => new
            {
                p.Id,
                p.Nombre,
                p.Apellido,
                p.Email,
                p.Origen
            })
            .ToListAsync(ct);

        return Ok(postulantes);
    }
}
