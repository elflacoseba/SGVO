using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGVO.Api.Extensions;
using SGVO.Application.Behaviors;
using SGVO.Application.Common;
using SGVO.Application.Features.TiposUnidadOrganizativa.Commands;
using SGVO.Application.Features.TiposUnidadOrganizativa.Dtos;
using SGVO.Application.Features.TiposUnidadOrganizativa.Queries;
using SGVO.Shared;

namespace SGVO.Api.Controllers;

/// <summary>
/// API controller for organizational unit types. Full CRUD with 6 endpoints.
/// </summary>
[ApiController]
[Route("api/v1/tipos-unidad-organizativa")]
public class TiposUnidadOrganizativaController : ControllerBase
{
    private readonly IQueryHandler<GetAllTiposUnidadOrganizativaQuery, PagedResult<TipoUnidadOrganizativaDto>> _getAll;
    private readonly IQueryHandler<GetTipoUnidadOrganizativaByIdQuery, TipoUnidadOrganizativaDto?> _getById;
    private readonly ICommandHandler<CrearTipoUnidadOrganizativaCommand, TipoUnidadOrganizativaDto> _crear;
    private readonly ICommandHandler<ActualizarTipoUnidadOrganizativaCommand, TipoUnidadOrganizativaDto> _actualizar;
    private readonly ICommandHandler<EliminarTipoUnidadOrganizativaCommand, Unit> _eliminar;
    private readonly ICommandHandler<ReactivarTipoUnidadOrganizativaCommand, TipoUnidadOrganizativaDto> _reactivar;
    private readonly IValidator<CrearTipoUnidadOrganizativaCommand> _crearValidator;
    private readonly IValidator<ActualizarTipoUnidadOrganizativaCommand> _actualizarValidator;
    private readonly IValidator<EliminarTipoUnidadOrganizativaCommand> _eliminarValidator;
    private readonly IValidator<ReactivarTipoUnidadOrganizativaCommand> _reactivarValidator;

    public TiposUnidadOrganizativaController(
        IQueryHandler<GetAllTiposUnidadOrganizativaQuery, PagedResult<TipoUnidadOrganizativaDto>> getAll,
        IQueryHandler<GetTipoUnidadOrganizativaByIdQuery, TipoUnidadOrganizativaDto?> getById,
        ICommandHandler<CrearTipoUnidadOrganizativaCommand, TipoUnidadOrganizativaDto> crear,
        ICommandHandler<ActualizarTipoUnidadOrganizativaCommand, TipoUnidadOrganizativaDto> actualizar,
        ICommandHandler<EliminarTipoUnidadOrganizativaCommand, Unit> eliminar,
        ICommandHandler<ReactivarTipoUnidadOrganizativaCommand, TipoUnidadOrganizativaDto> reactivar,
        IValidator<CrearTipoUnidadOrganizativaCommand> crearValidator,
        IValidator<ActualizarTipoUnidadOrganizativaCommand> actualizarValidator,
        IValidator<EliminarTipoUnidadOrganizativaCommand> eliminarValidator,
        IValidator<ReactivarTipoUnidadOrganizativaCommand> reactivarValidator)
    {
        _getAll = getAll;
        _getById = getById;
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
    /// Lists all active organizational unit types with pagination.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var pagination = new PageParameters(page, pageSize);
        var result = await _getAll.Handle(new GetAllTiposUnidadOrganizativaQuery(pagination), ct);

        if (result.IsFailure)
            return result.ToErrorActionResult();

        return Ok(result.Value);
    }

    /// <summary>
    /// Gets a single organizational unit type by ID.
    /// </summary>
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken ct)
    {
        var result = await _getById.Handle(new GetTipoUnidadOrganizativaByIdQuery(id), ct);

        if (result.IsFailure)
            return result.ToErrorActionResult();

        return result.Value is null
            ? NotFound(new { error = $"Tipo de unidad organizativa con id {id} no encontrado." })
            : Ok(result.Value);
    }

    /// <summary>
    /// Creates a new organizational unit type.
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateTipoUnidadOrganizativaRequest request, CancellationToken ct)
    {
        var command = new CrearTipoUnidadOrganizativaCommand(request.Nombre);

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
    /// Updates an existing organizational unit type.
    /// </summary>
    [HttpPut("{id:long}")]
    [Authorize]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateTipoUnidadOrganizativaRequest request, CancellationToken ct)
    {
        var command = new ActualizarTipoUnidadOrganizativaCommand(id, request.Nombre);

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
    /// Soft-deletes an organizational unit type.
    /// </summary>
    [HttpDelete("{id:long}")]
    [Authorize]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        var userId = User.GetUserId();
        if (!userId.HasValue)
            return Unauthorized(new { error = "No se pudo identificar al usuario." });

        var command = new EliminarTipoUnidadOrganizativaCommand(id, userId.Value);

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
    /// Reactivates a soft-deleted organizational unit type.
    /// </summary>
    [HttpPost("{id:long}/reactivate")]
    [Authorize]
    public async Task<IActionResult> Reactivate(long id, CancellationToken ct)
    {
        var command = new ReactivarTipoUnidadOrganizativaCommand(id);

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
