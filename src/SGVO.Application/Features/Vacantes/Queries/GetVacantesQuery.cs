using SGVO.Application.Common;

namespace SGVO.Application.Features.Vacantes.Queries;

public sealed record GetAllVacantesQuery(PageParameters Pagination) : IQuery<PagedResult<VacanteDto>>;

public sealed record GetVacanteByIdQuery(long Id) : IQuery<VacanteDto?>;
