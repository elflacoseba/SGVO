using Microsoft.EntityFrameworkCore;
using SGVO.Application.Common;
using SGVO.Application.Features.Vacantes.Queries;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Queries;

public sealed class GetVacanteByIdQueryHandler : IQueryHandler<GetVacanteByIdQuery, VacanteDto?>
{
    private readonly SgvoDbContext _db;

    public GetVacanteByIdQueryHandler(SgvoDbContext db)
    {
        _db = db;
    }

    public async Task<Result<VacanteDto?>> Handle(
        GetVacanteByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var vacante = await _db.Vacantes
            .AsNoTracking()
            .Where(v => v.Id == (ulong)query.Id && v.EliminadoEn == null && v.EliminadoPor == null)
            .Select(v => new VacanteDto
            {
                Id = (long)v.Id,
                PuestoId = (long)v.PuestoId,
                FechaApertura = v.FechaApertura,
                FechaCierre = v.FechaCierre,
                Motivo = v.Motivo,
                Estado = v.Estado
            })
            .FirstOrDefaultAsync(cancellationToken);

        return Result<VacanteDto?>.Success(vacante);
    }
}
