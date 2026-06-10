using Microsoft.EntityFrameworkCore;
using SGVO.Application.Common;
using SGVO.Application.Features.Vacantes.Queries;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Queries;

public sealed class GetAllVacantesQueryHandler : IQueryHandler<GetAllVacantesQuery, IReadOnlyList<VacanteDto>>
{
    private readonly SgvoDbContext _db;

    public GetAllVacantesQueryHandler(SgvoDbContext db)
    {
        _db = db;
    }

    public async Task<Result<IReadOnlyList<VacanteDto>>> Handle(
        GetAllVacantesQuery query,
        CancellationToken cancellationToken = default)
    {
        var vacantes = await _db.Vacantes
            .AsNoTracking()
            .Where(v => v.EliminadoEn == null && v.EliminadoPor == null)
            .Select(v => new VacanteDto
            {
                Id = (long)v.Id,
                PuestoId = (long)v.PuestoId,
                FechaApertura = v.FechaApertura,
                FechaCierre = v.FechaCierre,
                Motivo = v.Motivo,
                Estado = v.Estado
            })
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<VacanteDto>>.Success(vacantes);
    }
}
