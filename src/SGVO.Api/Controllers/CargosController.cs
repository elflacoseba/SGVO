using Microsoft.AspNetCore.Mvc;
using SGVO.Application.Common;
using SGVO.Application.Features.Cargos.Queries;

namespace SGVO.Api.Controllers;

[ApiController]
[Route("api/v1/cargos")]
public class CargosController : ControllerBase
{
    private readonly IQueryHandler<GetAllCargosQuery, IReadOnlyList<CargoDto>> _getAll;

    public CargosController(IQueryHandler<GetAllCargosQuery, IReadOnlyList<CargoDto>> getAll)
    {
        _getAll = getAll;
    }

    [HttpGet]
    [EndpointName("GetCargos")]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _getAll.Handle(new GetAllCargosQuery(), ct);

        if (result.IsFailure)
            return StatusCode(500, new { error = result.Error });

        return Ok(result.Value);
    }
}
