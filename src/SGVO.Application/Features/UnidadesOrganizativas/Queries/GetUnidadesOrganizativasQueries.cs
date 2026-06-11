using SGVO.Application.Common;

namespace SGVO.Application.Features.UnidadesOrganizativas.Queries;

/// <summary>
/// Query to retrieve all active organizational units with pagination.
/// </summary>
public sealed record GetAllUnidadesOrganizativasQuery(PageParameters Pagination, long? TipoUnidadOrganizativaId = null) : IQuery<PagedResult<UnidadOrganizativaDto>>;

/// <summary>
/// Query to retrieve a single organizational unit by ID.
/// </summary>
public sealed record GetUnidadOrganizativaByIdQuery(long Id) : IQuery<UnidadOrganizativaDetailDto?>;

/// <summary>
/// Query to retrieve the organizational unit hierarchy as a nested tree.
/// </summary>
public sealed record GetUnidadesOrganizativasTreeQuery() : IQuery<IReadOnlyList<UnidadOrganizativaTreeDto>>;
