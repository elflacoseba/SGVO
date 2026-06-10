using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGVO.Infrastructure.Persistence;

namespace SGVO.Api.Controllers;

[ApiController]
[Route("api/v1/vacantes")]
public class VacantesController : ControllerBase
{
    private readonly SgvoDbContext _db;

    public VacantesController(SgvoDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    [EndpointName("GetVacantes")]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var vacantes = await _db.Vacantes
            .AsNoTracking()
            .Select(v => new
            {
                v.Id,
                v.PuestoId,
                v.FechaApertura,
                v.FechaCierre,
                v.Motivo,
                v.Estado
            })
            .ToListAsync(ct);

        return Ok(vacantes);
    }

    [HttpGet("{id:long}")]
    [EndpointName("GetVacanteById")]
    public async Task<IActionResult> GetById(long id, CancellationToken ct)
    {
        var vacante = await _db.Vacantes
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == (ulong)id, ct);

        return vacante is null
            ? NotFound(new { error = $"Vacante con id {id} no encontrada." })
            : Ok(new
            {
                vacante.Id,
                vacante.PuestoId,
                vacante.FechaApertura,
                vacante.FechaCierre,
                vacante.Motivo,
                vacante.Estado
            });
    }
}
