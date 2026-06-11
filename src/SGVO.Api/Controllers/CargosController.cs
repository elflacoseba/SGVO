using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGVO.Api.Extensions;
using SGVO.Application.Behaviors;
using SGVO.Application.Common;
using SGVO.Application.Features.Cargos.Commands;
using SGVO.Application.Features.Cargos.Dtos;
using SGVO.Application.Features.Cargos.Queries;
using SGVO.Shared;

namespace SGVO.Api.Controllers;

/// <summary>
/// API controller for cargos. Full CRUD with reactivation (5 endpoints).
/// </summary>
[ApiController]
[Route("api/v1/cargos")]
public class CargosController : ControllerBase
{
    private readonly IQueryHandler<GetAllCargosQuery, PagedResult<CargoDto>> _getAll;
    private readonly IQueryHandler<GetCargoByIdQuery, CargoDetailDto?> _getById;
    private readonly ICommandHandler<CrearCargoCommand, CargoDetailDto> _crear;
    private readonly ICommandHandler<ActualizarCargoCommand, CargoDetailDto> _actualizar;
    private readonly ICommandHandler<EliminarCargoCommand, Unit> _eliminar;
    private readonly ICommandHandler<ReactivarCargoCommand, CargoDetailDto> _reactivar;
    private readonly IValidator<CrearCargoCommand> _crearValidator;
    private readonly IValidator<ActualizarCargoCommand> _actualizarValidator;
    private readonly IValidator<EliminarCargoCommand> _eliminarValidator;
    private readonly IValidator<ReactivarCargoCommand> _reactivarValidator;

    public CargosController(
        IQueryHandler<GetAllCargosQuery, PagedResult<CargoDto>> getAll,
        IQueryHandler<GetCargoByIdQuery, CargoDetailDto?> getById,
        ICommandHandler<CrearCargoCommand, CargoDetailDto> crear,
        ICommandHandler<ActualizarCargoCommand, CargoDetailDto> actualizar,
        ICommandHandler<EliminarCargoCommand, Unit> eliminar,
        ICommandHandler<ReactivarCargoCommand, CargoDetailDto> reactivar,
        IValidator<CrearCargoCommand> crearValidator,
        IValidator<ActualizarCargoCommand> actualizarValidator,
        IValidator<EliminarCargoCommand> eliminarValidator,
        IValidator<ReactivarCargoCommand> reactivarValidator)
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
    /// Lists all active cargos with pagination.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var pagination = new PageParameters(page, pageSize);
        var result = await _getAll.Handle(new GetAllCargosQuery(pagination), ct);

        if (result.IsFailure)
            return result.ToErrorActionResult();

        return Ok(result.Value);
    }

    /// <summary>
    /// Gets a single cargo by ID.
    /// </summary>
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken ct)
    {
        var result = await _getById.Handle(new GetCargoByIdQuery(id), ct);

        if (result.IsFailure)
            return result.ToErrorActionResult();

        return result.Value is null
            ? NotFound(new { error = $"Cargo con id {id} no encontrado." })
            : Ok(result.Value);
    }

    /// <summary>
    /// Creates a new cargo.
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateCargoRequest request, CancellationToken ct)
    {
        var command = new CrearCargoCommand(request.Nombre, request.Descripcion);

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
    /// Updates an existing cargo.
    /// </summary>
    [HttpPut("{id:long}")]
    [Authorize]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateCargoRequest request, CancellationToken ct)
    {
        var command = new ActualizarCargoCommand(id, request.Nombre, request.Descripcion);

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
    /// Soft-deletes a cargo. Blocked if active Puestos or CargoSkills reference it.
    /// </summary>
    [HttpDelete("{id:long}")]
    [Authorize]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        var userId = User.GetUserId();
        if (!userId.HasValue)
            return Unauthorized(new { error = "No se pudo identificar al usuario." });

        var command = new EliminarCargoCommand(id, userId.Value);

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
    /// Reactivates a soft-deleted cargo. Idempotent if already active.
    /// </summary>
    [HttpPost("{id:long}/reactivate")]
    [Authorize]
    public async Task<IActionResult> Reactivate(long id, CancellationToken ct)
    {
        var command = new ReactivarCargoCommand(id);

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
