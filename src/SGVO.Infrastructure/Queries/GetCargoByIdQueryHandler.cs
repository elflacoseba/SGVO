using Microsoft.EntityFrameworkCore;
using SGVO.Application.Common;
using SGVO.Application.Features.Cargos.Queries;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Queries;

/// <summary>
/// Handler for GetCargoByIdQuery. Returns a single cargo by ID.
/// Soft-deleted cargos are not returned.
/// </summary>
public class GetCargoByIdQueryHandler : IQueryHandler<GetCargoByIdQuery, CargoDetailDto?>
{
    private readonly SgvoDbContext _dbContext;

    public GetCargoByIdQueryHandler(SgvoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<CargoDetailDto?>> Handle(
        GetCargoByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Cargos
            .AsNoTracking()
            .Where(c => c.Id == request.Id && c.EliminadoEn == null)
            .FirstOrDefaultAsync(cancellationToken);

        if (entity is null)
            return Result<CargoDetailDto?>.Success(null);

        var dto = new CargoDetailDto
        {
            Id = entity.Id,
            Nombre = entity.Nombre,
            Descripcion = entity.Descripcion,
            Activo = entity.Activo,
            CreadoEn = entity.CreadoEn,
            ModificadoEn = entity.ModificadoEn
        };

        return Result<CargoDetailDto?>.Success(dto);
    }
}
