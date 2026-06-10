using Microsoft.AspNetCore.Mvc;
using SGVO.Api.Extensions;
using SGVO.Application.Common;

namespace SGVO.Api.Controllers;

/// <summary>
/// Controlador base genérico para endpoints GetAll con paginación.
/// Elimina duplicación entre controladores que solo necesitan GetAll.
/// </summary>
/// <typeparam name="TQuery">Tipo del query GetAll</typeparam>
/// <typeparam name="TDto">Tipo del DTO retornado</typeparam>
[ApiController]
public abstract class BaseReadOnlyController<TQuery, TDto> : ControllerBase
    where TQuery : class, IQuery<PagedResult<TDto>>
{
    private readonly IQueryHandler<TQuery, PagedResult<TDto>> _getAll;

    protected BaseReadOnlyController(IQueryHandler<TQuery, PagedResult<TDto>> getAll)
    {
        _getAll = getAll;
    }

    /// <summary>
    /// Endpoint GetAll con paginación.
    /// </summary>
    /// <param name="page">Número de página (default: 1)</param>
    /// <param name="pageSize">Elementos por página (default: 20, max: 100)</param>
    /// <param name="ct">Cancellation token</param>
    /// <param name="queryFactory">Función que crea la instancia del query con los parámetros</param>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var pagination = new PageParameters(page, pageSize);
        var query = CreateQuery(pagination);
        var result = await _getAll.Handle(query, ct);

        if (result.IsFailure)
            return result.ToErrorActionResult();

        return Ok(result.Value);
    }

    /// <summary>
    /// Factory method para crear la instancia del query con los parámetros de paginación.
    /// Cada controlador concreto debe implementar este método.
    /// </summary>
    protected abstract TQuery CreateQuery(PageParameters pagination);
}
