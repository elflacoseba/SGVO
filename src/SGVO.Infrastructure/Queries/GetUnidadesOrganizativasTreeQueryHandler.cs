using Microsoft.EntityFrameworkCore;
using SGVO.Application.Features.UnidadesOrganizativas.Queries;
using SGVO.Application.Common;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Queries;

/// <summary>
/// Handler for GetUnidadesOrganizativasTreeQuery.
/// Fetches all active units with raw SQL and assembles the tree in memory.
/// </summary>
public class GetUnidadesOrganizativasTreeQueryHandler : IQueryHandler<GetUnidadesOrganizativasTreeQuery, IReadOnlyList<UnidadOrganizativaTreeDto>>
{
    private readonly SgvoDbContext _dbContext;

    public GetUnidadesOrganizativasTreeQueryHandler(SgvoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<IReadOnlyList<UnidadOrganizativaTreeDto>>> Handle(
        GetUnidadesOrganizativasTreeQuery request,
        CancellationToken cancellationToken)
    {
        // Fetch all active units flat from DB — avoids N+1
        var flat = await _dbContext.UnidadesOrganizativas
            .AsNoTracking()
            .Include(e => e.TipoUnidadOrganizativa)
            .Where(e => e.EliminadoEn == null && e.EliminadoPor == null)
            .OrderBy(e => e.NivelJerarquico)
            .ThenBy(e => e.Id)
            .Select(e => new UnidadOrganizativaTreeDto
            {
                Id = (long)e.Id,
                Nombre = e.Nombre,
                TipoUnidadOrganizativaId = (long)e.TipoUnidadOrganizativaId,
                TipoNombre = e.TipoUnidadOrganizativa != null ? e.TipoUnidadOrganizativa.Nombre : string.Empty,
                NivelJerarquico = e.NivelJerarquico,
                PadreId = e.PadreId != null ? (long)e.PadreId : null,
                Activo = e.Activo ?? false
            })
            .ToListAsync(cancellationToken);

        // Assemble tree in memory
        var lookup = flat.ToDictionary(u => u.Id);
        var roots = new List<UnidadOrganizativaTreeDto>();

        foreach (var unit in flat)
        {
            if (unit.PadreId.HasValue && lookup.TryGetValue(unit.PadreId.Value, out var parent))
            {
                parent.Hijos.Add(unit);
            }
            else
            {
                roots.Add(unit);
            }
        }

        return Result<IReadOnlyList<UnidadOrganizativaTreeDto>>.Success(roots);
    }
}
