namespace SGVO.Application.Common;

/// <summary>
/// Resultado paginado de una consulta GetAll.
/// </summary>
/// <typeparam name="T">Tipo de los elementos</typeparam>
public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages)
{
    /// <summary>
    /// Indica si hay una página siguiente disponible.
    /// </summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>
    /// Indica si hay una página anterior disponible.
    /// </summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>
    /// Crea un PagedResult a partir de una lista completa y parámetros de paginación.
    /// Útil para tests o cuando ya se tienen todos los datos en memoria.
    /// </summary>
    public static PagedResult<T> FromList(IReadOnlyList<T> allItems, PageParameters parameters)
    {
        var totalCount = allItems.Count;
        var totalPages = (int)Math.Ceiling(totalCount / (double)parameters.PageSize);
        var pagedItems = allItems
            .Skip(parameters.SkipCount)
            .Take(parameters.PageSize)
            .ToList();

        return new PagedResult<T>(pagedItems, totalCount, parameters.Page, parameters.PageSize, totalPages);
    }
}
