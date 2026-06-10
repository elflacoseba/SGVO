using Microsoft.AspNetCore.Mvc;
using SGVO.Application.Common;
using SGVO.Application.Features.Cargos.Queries;

namespace SGVO.Api.Controllers;

[Route("api/v1/cargos")]
public class CargosController : BaseReadOnlyController<GetAllCargosQuery, CargoDto>
{
    public CargosController(IQueryHandler<GetAllCargosQuery, PagedResult<CargoDto>> getAll)
        : base(getAll)
    {
    }

    protected override GetAllCargosQuery CreateQuery(PageParameters pagination)
    {
        return new GetAllCargosQuery(pagination);
    }
}
