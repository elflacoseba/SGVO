using Microsoft.EntityFrameworkCore;
using SGVO.Application.Common;
using SGVO.Application.Features.Cargos.Queries;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Queries;

public sealed class GetAllCargosQueryHandler : IQueryHandler<GetAllCargosQuery, IReadOnlyList<CargoDto>>
{
    private readonly SgvoDbContext _db;

    public GetAllCargosQueryHandler(SgvoDbContext db)
    {
        _db = db;
    }

    public async Task<Result<IReadOnlyList<CargoDto>>> Handle(
        GetAllCargosQuery query,
        CancellationToken cancellationToken = default)
    {
        var cargos = await _db.Cargos
            .AsNoTracking()
            .Select(c => new CargoDto
            {
                Id = (long)c.Id,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion
            })
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<CargoDto>>.Success(cargos);
    }
}
