using Microsoft.AspNetCore.Mvc;
using SGVO.Api.Extensions;
using SGVO.Application.Common;
using SGVO.Application.Features.Vacantes.Queries;

namespace SGVO.Api.Controllers;

[ApiController]
[Route("api/v1/vacantes")]
public class VacantesController : ControllerBase
{
    private readonly IQueryHandler<GetAllVacantesQuery, IReadOnlyList<VacanteDto>> _getAll;
    private readonly IQueryHandler<GetVacanteByIdQuery, VacanteDto?> _getById;

    public VacantesController(
        IQueryHandler<GetAllVacantesQuery, IReadOnlyList<VacanteDto>> getAll,
        IQueryHandler<GetVacanteByIdQuery, VacanteDto?> getById)
    {
        _getAll = getAll;
        _getById = getById;
    }

    [HttpGet]
    [EndpointName("GetVacantes")]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _getAll.Handle(new GetAllVacantesQuery(), ct);

        if (result.IsFailure)
            return result.ToErrorActionResult();

        return Ok(result.Value);
    }

    [HttpGet("{id:long}")]
    [EndpointName("GetVacanteById")]
    public async Task<IActionResult> GetById(long id, CancellationToken ct)
    {
        var result = await _getById.Handle(new GetVacanteByIdQuery(id), ct);

        if (result.IsFailure)
            return result.ToErrorActionResult();

        return result.Value is null
            ? NotFound(new { error = $"Vacante con id {id} no encontrada." })
            : Ok(result.Value);
    }
}
