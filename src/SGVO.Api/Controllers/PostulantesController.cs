using Microsoft.AspNetCore.Mvc;
using SGVO.Application.Common;
using SGVO.Application.Features.Postulantes.Queries;

namespace SGVO.Api.Controllers;

[ApiController]
[Route("api/v1/postulantes")]
public class PostulantesController : ControllerBase
{
    private readonly IQueryHandler<GetAllPostulantesQuery, IReadOnlyList<PostulanteDto>> _getAll;

    public PostulantesController(IQueryHandler<GetAllPostulantesQuery, IReadOnlyList<PostulanteDto>> getAll)
    {
        _getAll = getAll;
    }

    [HttpGet]
    [EndpointName("GetPostulantes")]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _getAll.Handle(new GetAllPostulantesQuery(), ct);

        if (result.IsFailure)
            return StatusCode(500, new { error = result.Error });

        return Ok(result.Value);
    }
}
