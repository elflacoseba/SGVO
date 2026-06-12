using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGVO.Api.Extensions;
using SGVO.Application.Behaviors;
using SGVO.Application.Common;
using SGVO.Application.Features.Skills.Commands;
using SGVO.Application.Features.Skills.Dtos;
using SGVO.Application.Features.Skills.Queries;
using SGVO.Shared;

namespace SGVO.Api.Controllers;

/// <summary>
/// API controller for skills. Full CRUD with reactivation (5 endpoints).
/// </summary>
[ApiController]
[Route("api/v1/skills")]
public class SkillsController : ControllerBase
{
    private readonly IQueryHandler<GetAllSkillsQuery, PagedResult<SkillDto>> _getAll;
    private readonly IQueryHandler<GetSkillByIdQuery, SkillDetailDto?> _getById;
    private readonly ICommandHandler<CrearSkillCommand, SkillDetailDto> _crear;
    private readonly ICommandHandler<ActualizarSkillCommand, SkillDetailDto> _actualizar;
    private readonly ICommandHandler<EliminarSkillCommand, Unit> _eliminar;
    private readonly ICommandHandler<ReactivarSkillCommand, SkillDetailDto> _reactivar;
    private readonly IValidator<CrearSkillCommand> _crearValidator;
    private readonly IValidator<ActualizarSkillCommand> _actualizarValidator;
    private readonly IValidator<EliminarSkillCommand> _eliminarValidator;
    private readonly IValidator<ReactivarSkillCommand> _reactivarValidator;

    public SkillsController(
        IQueryHandler<GetAllSkillsQuery, PagedResult<SkillDto>> getAll,
        IQueryHandler<GetSkillByIdQuery, SkillDetailDto?> getById,
        ICommandHandler<CrearSkillCommand, SkillDetailDto> crear,
        ICommandHandler<ActualizarSkillCommand, SkillDetailDto> actualizar,
        ICommandHandler<EliminarSkillCommand, Unit> eliminar,
        ICommandHandler<ReactivarSkillCommand, SkillDetailDto> reactivar,
        IValidator<CrearSkillCommand> crearValidator,
        IValidator<ActualizarSkillCommand> actualizarValidator,
        IValidator<EliminarSkillCommand> eliminarValidator,
        IValidator<ReactivarSkillCommand> reactivarValidator)
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
    /// Lists all active skills with pagination.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var pagination = new PageParameters(page, pageSize);
        var result = await _getAll.Handle(new GetAllSkillsQuery(pagination), ct);

        if (result.IsFailure)
            return result.ToErrorActionResult();

        return Ok(result.Value);
    }

    /// <summary>
    /// Gets a single skill by ID.
    /// </summary>
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken ct)
    {
        var result = await _getById.Handle(new GetSkillByIdQuery(id), ct);

        if (result.IsFailure)
            return result.ToErrorActionResult();

        return result.Value is null
            ? NotFound(new { error = $"Skill con id {id} no encontrado." })
            : Ok(result.Value);
    }

    /// <summary>
    /// Creates a new skill.
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateSkillRequest request, CancellationToken ct)
    {
        var command = new CrearSkillCommand(request.Nombre, request.Categoria, request.Descripcion);

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
    /// Updates an existing skill.
    /// </summary>
    [HttpPut("{id:long}")]
    [Authorize]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateSkillRequest request, CancellationToken ct)
    {
        var command = new ActualizarSkillCommand(id, request.Nombre, request.Categoria, request.Descripcion);

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
    /// Soft-deletes a skill. Blocked if active CargoSkills or PersonaSkills reference it.
    /// </summary>
    [HttpDelete("{id:long}")]
    [Authorize]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        var userId = User.GetUserId();
        if (!userId.HasValue)
            return Unauthorized(new { error = "No se pudo identificar al usuario." });

        var command = new EliminarSkillCommand(id, userId.Value);

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
    /// Reactivates a soft-deleted skill. Idempotent if already active.
    /// </summary>
    [HttpPost("{id:long}/reactivate")]
    [Authorize]
    public async Task<IActionResult> Reactivate(long id, CancellationToken ct)
    {
        var command = new ReactivarSkillCommand(id);

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
