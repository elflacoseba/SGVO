using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGVO.Api.Extensions;
using SGVO.Application.Behaviors;
using SGVO.Application.Common;
using SGVO.Application.Features.UnidadesOrganizativas.Commands;
using SGVO.Application.Features.UnidadesOrganizativas.Dtos;
using SGVO.Application.Features.UnidadesOrganizativas.Queries;
using SGVO.Shared;

namespace SGVO.Api.Controllers;

/// <summary>
/// API controller for organizational units. Full CRUD with 7 endpoints.
/// </summary>
[ApiController]
[Route("api/v1/unidades-organizativas")]
public class UnidadesOrganizativasController : ControllerBase
{
    private readonly IQueryHandler<GetAllUnidadesOrganizativasQuery, PagedResult<UnidadOrganizativaDto>> _getAll;
    private readonly IQueryHandler<GetUnidadOrganizativaByIdQuery, UnidadOrganizativaDetailDto?> _getById;
    private readonly IQueryHandler<GetUnidadesOrganizativasTreeQuery, IReadOnlyList<UnidadOrganizativaTreeDto>> _getTree;
    private readonly ICommandHandler<CrearUnidadOrganizativaCommand, UnidadOrganizativaDetailDto> _crear;
    private readonly ICommandHandler<ActualizarUnidadOrganizativaCommand, UnidadOrganizativaDetailDto> _actualizar;
    private readonly ICommandHandler<EliminarUnidadOrganizativaCommand, Unit> _eliminar;
    private readonly ICommandHandler<ReactivarUnidadOrganizativaCommand, UnidadOrganizativaDetailDto> _reactivar;
    private readonly IValidator<CrearUnidadOrganizativaCommand> _crearValidator;
    private readonly IValidator<ActualizarUnidadOrganizativaCommand> _actualizarValidator;
    private readonly IValidator<EliminarUnidadOrganizativaCommand> _eliminarValidator;
    private readonly IValidator<ReactivarUnidadOrganizativaCommand> _reactivarValidator;

    public UnidadesOrganizativasController(
        IQueryHandler<GetAllUnidadesOrganizativasQuery, PagedResult<UnidadOrganizativaDto>> getAll,
        IQueryHandler<GetUnidadOrganizativaByIdQuery, UnidadOrganizativaDetailDto?> getById,
        IQueryHandler<GetUnidadesOrganizativasTreeQuery, IReadOnlyList<UnidadOrganizativaTreeDto>> getTree,
        ICommandHandler<CrearUnidadOrganizativaCommand, UnidadOrganizativaDetailDto> crear,
        ICommandHandler<ActualizarUnidadOrganizativaCommand, UnidadOrganizativaDetailDto> actualizar,
        ICommandHandler<EliminarUnidadOrganizativaCommand, Unit> eliminar,
        ICommandHandler<ReactivarUnidadOrganizativaCommand, UnidadOrganizativaDetailDto> reactivar,
        IValidator<CrearUnidadOrganizativaCommand> crearValidator,
        IValidator<ActualizarUnidadOrganizativaCommand> actualizarValidator,
        IValidator<EliminarUnidadOrganizativaCommand> eliminarValidator,
        IValidator<ReactivarUnidadOrganizativaCommand> reactivarValidator)
    {
        _getAll = getAll;
        _getById = getById;
        _getTree = getTree;
        _crear = crear;
        _actualizar = actualizar;
        _eliminar = eliminar;
        _reactivar = reactivar;
        _crearValidator = crearValidator;
        _actualizarValidator = actualizarValidator;
        _eliminarValidator = eliminarValidator;
        _reactivarValidator = reactivarValidator;
    }

    /// <summary>
    /// Lists all active organizational units with pagination.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] long? tipoUnidadOrganizativaId = null,
        CancellationToken ct = default)
    {
        var pagination = new PageParameters(page, pageSize);
        var result = await _getAll.Handle(new GetAllUnidadesOrganizativasQuery(pagination, tipoUnidadOrganizativaId), ct);

        if (result.IsFailure)
            return result.ToErrorActionResult();

        return Ok(result.Value);
    }

    /// <summary>
    /// Gets a single organizational unit by ID.
    /// </summary>
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

    /// <summary>
    /// Gets the organizational unit tree.
    /// </summary>
    [HttpGet("tree")]
    public async Task<IActionResult> GetTree(CancellationToken ct)
    {
        var result = await _getTree.Handle(new GetUnidadesOrganizativasTreeQuery(), ct);

        if (result.IsFailure)
            return result.ToErrorActionResult();

        return Ok(result.Value);
    }

    /// <summary>
    /// Creates a new organizational unit.
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateUnidadOrganizativaRequest request, CancellationToken ct)
    {
        var command = new CrearUnidadOrganizativaCommand(
            request.Nombre,
            request.TipoUnidadOrganizativaId,
            request.NivelJerarquico,
            request.PadreId);

        var result = await ValidationBehavior.HandleAsync(
            command,
            _crearValidator,
            (cmd, token) => _crear.Handle(cmd, token),
            ct);

        if (result.IsFailure)
            return result.ToErrorActionResult();

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>
    /// Updates an existing organizational unit.
    /// </summary>
    [HttpPut("{id:long}")]
    [Authorize]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateUnidadOrganizativaRequest request, CancellationToken ct)
    {
        var command = new ActualizarUnidadOrganizativaCommand(
            id,
            request.Nombre,
            request.TipoUnidadOrganizativaId,
            request.NivelJerarquico,
            request.PadreId);

        var result = await ValidationBehavior.HandleAsync(
            command,
            _actualizarValidator,
            (cmd, token) => _actualizar.Handle(cmd, token),
            ct);

        if (result.IsFailure)
            return result.ToErrorActionResult();

        return Ok(result.Value);
    }

    /// <summary>
    /// Soft-deletes an organizational unit.
    /// </summary>
    [HttpDelete("{id:long}")]
    [Authorize]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        var userId = User.GetUserId();
        if (!userId.HasValue)
            return Unauthorized(new { error = "No se pudo identificar al usuario." });

        var command = new EliminarUnidadOrganizativaCommand(id, userId.Value);

        var result = await ValidationBehavior.HandleAsync(
            command,
            _eliminarValidator,
            (cmd, token) => _eliminar.Handle(cmd, token),
            ct);

        if (result.IsFailure)
            return result.ToErrorActionResult();

        return NoContent();
    }

    /// <summary>
    /// Reactivates a soft-deleted organizational unit.
    /// </summary>
    [HttpPost("{id:long}/reactivate")]
    [Authorize]
    public async Task<IActionResult> Reactivate(long id, CancellationToken ct)
    {
        var command = new ReactivarUnidadOrganizativaCommand(id);

        var result = await ValidationBehavior.HandleAsync(
            command,
            _reactivarValidator,
            (cmd, token) => _reactivar.Handle(cmd, token),
            ct);

        if (result.IsFailure)
            return result.ToErrorActionResult();

        return Ok(result.Value);
    }
}
