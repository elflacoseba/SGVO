namespace SGVO.Application.Common;

/// <summary>
/// Parámetros de paginación para consultas GetAll.
/// </summary>
/// <param name="Page">Número de página (1-indexed). Default: 1</param>
/// <param name="PageSize">Cantidad de elementos por página. Default: 20, Max: 100</param>
public sealed record PageParameters(int Page = 1, int PageSize = 20)
{
    public const int MaxPageSize = 100;
    public const int DefaultPageSize = 20;

    /// <summary>
    /// Valida y normaliza los parámetros de paginación.
    /// </summary>
    public PageParameters Normalize()
    {
        var page = Page < 1 ? 1 : Page;
        var pageSize = PageSize < 1 ? DefaultPageSize : Math.Min(PageSize, MaxPageSize);
        return new PageParameters(page, pageSize);
    }

    /// <summary>
    /// Calcula el número de elementos a saltar para Skip().
    /// </summary>
    public int SkipCount => (Page - 1) * PageSize;
}
