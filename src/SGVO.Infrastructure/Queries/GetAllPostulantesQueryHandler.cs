using Microsoft.EntityFrameworkCore;
using SGVO.Application.Common;
using SGVO.Application.Features.Postulantes.Queries;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Queries;

public sealed class GetAllPostulantesQueryHandler : IQueryHandler<GetAllPostulantesQuery, IReadOnlyList<PostulanteDto>>
{
    private readonly SgvoDbContext _db;

    public GetAllPostulantesQueryHandler(SgvoDbContext db)
    {
        _db = db;
    }

    public async Task<Result<IReadOnlyList<PostulanteDto>>> Handle(
        GetAllPostulantesQuery query,
        CancellationToken cancellationToken = default)
    {
        var postulantes = await _db.Postulantes
            .AsNoTracking()
            .Select(p => new PostulanteDto
            {
                Id = (long)p.Id,
                Nombre = p.Nombre,
                Apellido = p.Apellido,
                Email = p.Email,
                Origen = p.Origen
            })
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<PostulanteDto>>.Success(postulantes);
    }
}
