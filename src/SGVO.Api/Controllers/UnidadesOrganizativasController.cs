using Microsoft.AspNetCore.Mvc;
using SGVO.Api.Extensions;
using SGVO.Application.Common;
using SGVO.Application.Features.UnidadesOrganizativas.Queries;

namespace SGVO.Api.Controllers;

/// <summary>
/// API controller for organizational units. Full ControllerBase with 3 endpoints.
/// </summary>
[ApiController]
[Route("api/v1/unidades-organizativas")]
public class UnidadesOrganizativasController : ControllerBase
{
    private readonly IQueryHandler<GetAllUnidadesOrganizativasQuery, PagedResult<UnidadOrganizativaDto>> _getAll;
    private readonly IQueryHandler<GetUnidadOrganizativaByIdQuery, UnidadOrganizativaDetailDto?> _getById;
    private readonly IQueryHandler<GetUnidadesOrganizativasTreeQuery, IReadOnlyList<UnidadOrganizativaTreeDto>> _getTree;

    public UnidadesOrganizativasController(
        IQueryHandler<GetAllUnidadesOrganizativasQuery, PagedResult<UnidadOrganizativaDto>> getAll,
        IQueryHandler<GetUnidadOrganizativaByIdQuery, UnidadOrganizativaDetailDto?> getById,
        IQueryHandler<GetUnidadesOrganizativasTreeQuery, IReadOnlyList<UnidadOrganizativaTreeDto>> getTree)
    {
        _getAll = getAll;
        _getById = getById;
        _getTree = getTree;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? tipo = null,
        CancellationToken ct = default)
    {
        var pagination = new PageParameters(page, pageSize);
        var result = await _getAll.Handle(new GetAllUnidadesOrganizativasQuery(pagination, tipo), ct);

        if (result.IsFailure)
            return result.ToErrorActionResult();

        return Ok(result.Value);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken ct)
    {
        var result = await _getById.Handle(new GetUnidadOrganizativaByIdQuery(id), ct);

        if (result.IsFailure)
            return result.ToErrorActionResult();

        return result.Value is null
            ? NotFound(new { error = $"Unidad organizativa con id {id} no encontrada." })
            : Ok(result.Value);
    }

    [HttpGet("tree")]
    public async Task<IActionResult> GetTree(CancellationToken ct)
    {
        var result = await _getTree.Handle(new GetUnidadesOrganizativasTreeQuery(), ct);

        if (result.IsFailure)
            return result.ToErrorActionResult();

        return Ok(result.Value);
    }
}
