using Microsoft.AspNetCore.Mvc;
using SGVO.Application.Common;
using SGVO.Application.Features.Postulantes.Queries;

namespace SGVO.Api.Controllers;

[Route("api/v1/postulantes")]
public class PostulantesController : BaseReadOnlyController<GetAllPostulantesQuery, PostulanteDto>
{
    public PostulantesController(IQueryHandler<GetAllPostulantesQuery, PagedResult<PostulanteDto>> getAll)
        : base(getAll)
    {
    }

    protected override GetAllPostulantesQuery CreateQuery(PageParameters pagination)
    {
        return new GetAllPostulantesQuery(pagination);
    }
}
