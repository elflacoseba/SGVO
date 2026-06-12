using Microsoft.EntityFrameworkCore;
using SGVO.Application.Common;
using SGVO.Application.Features.Skills.Queries;
using SGVO.Infrastructure.Persistence;
using SGVO.Shared;

namespace SGVO.Infrastructure.Queries;

/// <summary>
/// Handler for GetSkillByIdQuery. Returns a single skill by ID.
/// Soft-deleted skills are not returned.
/// </summary>
public class GetSkillByIdQueryHandler : IQueryHandler<GetSkillByIdQuery, SkillDetailDto?>
{
    private readonly SgvoDbContext _dbContext;

    public GetSkillByIdQueryHandler(SgvoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<SkillDetailDto?>> Handle(
        GetSkillByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Skills
            .AsNoTracking()
            .Where(s => s.Id == request.Id && s.EliminadoEn == null)
            .FirstOrDefaultAsync(cancellationToken);

        if (entity is null)
            return Result<SkillDetailDto?>.Success(null);

        var dto = new SkillDetailDto
        {
            Id = entity.Id,
            Nombre = entity.Nombre,
            Categoria = entity.Categoria,
            Descripcion = entity.Descripcion,
            Activo = entity.Activo ?? false,
            CreadoEn = entity.CreadoEn,
            ModificadoEn = entity.ModificadoEn
        };

        return Result<SkillDetailDto?>.Success(dto);
    }
}
