using SGVO.Application.Common;

namespace SGVO.Application.Features.Postulantes.Queries;

public sealed record GetAllPostulantesQuery(PageParameters Pagination) : IQuery<PagedResult<PostulanteDto>>;
