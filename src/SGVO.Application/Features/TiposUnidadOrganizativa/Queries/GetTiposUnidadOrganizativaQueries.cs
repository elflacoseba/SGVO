using SGVO.Application.Common;
using SGVO.Application.Features.TiposUnidadOrganizativa.Dtos;

namespace SGVO.Application.Features.TiposUnidadOrganizativa.Queries;

/// <summary>
/// Query to retrieve all active organizational unit types with pagination.
/// </summary>
public sealed record GetAllTiposUnidadOrganizativaQuery(PageParameters Pagination) : IQuery<PagedResult<TipoUnidadOrganizativaDto>>;

/// <summary>
/// Query to retrieve a single organizational unit type by ID.
/// </summary>
public sealed record GetTipoUnidadOrganizativaByIdQuery(long Id) : IQuery<TipoUnidadOrganizativaDto?>;
