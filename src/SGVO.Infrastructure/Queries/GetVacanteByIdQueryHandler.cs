using Microsoft.EntityFrameworkCore;
using SGVO.Application.Common;
using SGVO.Application.Features.Vacantes.Queries;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Queries;

/// <summary>
/// Handler para GetVacanteByIdQuery. Retorna una vacante por ID excluyendo soft-deleted.
/// </summary>
public class GetVacanteByIdQueryHandler : IQueryHandler<GetVacanteByIdQuery, VacanteDto?>
{
    private readonly SgvoDbContext _dbContext;

    public GetVacanteByIdQueryHandler(SgvoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<VacanteDto?>> Handle(
        GetVacanteByIdQuery request,
        CancellationToken cancellationToken)
    {
        var vacante = await _dbContext.Vacantes
            .AsNoTracking()
            .Where(v => v.Id == request.Id && v.EliminadoEn == null)
            .Select(v => new VacanteDto
            {
                Id = v.Id,
                PuestoId = v.PuestoId,
                FechaApertura = v.FechaApertura,
                FechaCierre = v.FechaCierre,
                Motivo = v.Motivo,
                Estado = v.Estado
            })
            .FirstOrDefaultAsync(cancellationToken);

        return Result<VacanteDto?>.Success(vacante);
    }
}
