using SGVO.Application.Common;

namespace SGVO.Application.Features.Cargos.Queries;

public sealed record GetAllCargosQuery(PageParameters Pagination) : IQuery<PagedResult<CargoDto>>;
