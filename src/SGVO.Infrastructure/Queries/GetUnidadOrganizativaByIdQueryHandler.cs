using Microsoft.EntityFrameworkCore;
using SGVO.Application.Features.UnidadesOrganizativas.Queries;
using SGVO.Application.Common;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Queries;

/// <summary>
/// Handler for GetUnidadOrganizativaByIdQuery. Returns a single organizational unit by ID.
/// </summary>
public class GetUnidadOrganizativaByIdQueryHandler : IQueryHandler<GetUnidadOrganizativaByIdQuery, UnidadOrganizativaDetailDto?>
{
    private readonly SgvoDbContext _dbContext;

    public GetUnidadOrganizativaByIdQueryHandler(SgvoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<UnidadOrganizativaDetailDto?>> Handle(
        GetUnidadOrganizativaByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.UnidadesOrganizativas
            .AsNoTracking()
            .Where(e => e.Id == (ulong)request.Id && e.EliminadoEn == null && e.EliminadoPor == null)
            .FirstOrDefaultAsync(cancellationToken);

        if (entity is null)
            return Result<UnidadOrganizativaDetailDto?>.Success(null);

        // Fetch parent info if PadreId exists
        UnidadOrganizativaParentDto? parent = null;
        if (entity.PadreId.HasValue)
        {
            var parentEntity = await _dbContext.UnidadesOrganizativas
                .AsNoTracking()
                .Where(e => e.Id == entity.PadreId.Value && e.EliminadoEn == null && e.EliminadoPor == null)
                .Select(e => new UnidadOrganizativaParentDto { Id = (long)e.Id, Nombre = e.Nombre })
                .FirstOrDefaultAsync(cancellationToken);
            parent = parentEntity;
        }

        // Count active children
        var childrenCount = await _dbContext.UnidadesOrganizativas
            .AsNoTracking()
            .CountAsync(e => e.PadreId == entity.Id && e.EliminadoEn == null && e.EliminadoPor == null, cancellationToken);

        var dto = new UnidadOrganizativaDetailDto
        {
            Id = (long)entity.Id,
            Nombre = entity.Nombre,
            Tipo = entity.Tipo,
            NivelJerarquico = entity.NivelJerarquico,
            PadreId = entity.PadreId != null ? (long)entity.PadreId : null,
            Activo = entity.Activo ?? false,
            CreadoEn = entity.CreadoEn,
            ModificadoEn = entity.ModificadoEn,
            Padre = parent,
            ChildrenCount = childrenCount
        };

        return Result<UnidadOrganizativaDetailDto?>.Success(dto);
    }
}
