using Microsoft.EntityFrameworkCore;
using SGVO.Domain.Entities;
using SGVO.Domain.Interfaces;
using SGVO.Infrastructure.Persistence;
using SGVO.Infrastructure.Persistence.Entities;

namespace SGVO.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repositorio para gestionar los refresh tokens en la base de datos.
/// Maps between domain RefreshToken and infrastructure RefreshTokenEntity.
/// </summary>
public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly SgvoDbContext _dbContext;

    public RefreshTokenRepository(SgvoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateAsync(
        RefreshToken token,
        CancellationToken cancellationToken = default)
    {
        var entity = ToEntity(token);
        _dbContext.RefreshTokens.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<RefreshToken?> GetByTokenHashAsync(
        string hash,
        CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(
                t => t.TokenHash == hash,
                cancellationToken);

        return entity is null ? null : ToDomain(entity);
    }

    public async Task RevokeAsync(
        ulong tokenId,
        string reason,
        CancellationToken cancellationToken = default)
    {
        var token = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.Id == tokenId, cancellationToken);

        if (token is null)
            return;

        token.Revocado = true;
        token.FechaRevocacion = DateTime.UtcNow;
        token.MotivoRevocacion = reason;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeFamilyAsync(
        string familyId,
        string reason,
        CancellationToken cancellationToken = default)
    {
        var tokens = await _dbContext.RefreshTokens
            .Where(t => t.FamilyId == familyId && t.Revocado == false)
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            token.Revocado = true;
            token.FechaRevocacion = DateTime.UtcNow;
            token.MotivoRevocacion = reason;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(
        ulong userId,
        CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.RefreshTokens
            .AsNoTracking()
            .Where(t => t.UsuarioId == userId && t.Revocado == false && t.FechaExpiracion > DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        return entities.Select(ToDomain).ToList();
    }

    private static RefreshToken ToDomain(RefreshTokenEntity entity) => new()
    {
        Id = entity.Id,
        UsuarioId = entity.UsuarioId,
        TokenHash = entity.TokenHash,
        FamilyId = entity.FamilyId,
        FechaCreacion = entity.FechaCreacion,
        FechaExpiracion = entity.FechaExpiracion,
        FechaUso = entity.FechaUso,
        ReemplazadoPorId = entity.ReemplazadoPorId,
        Revocado = entity.Revocado,
        FechaRevocacion = entity.FechaRevocacion,
        MotivoRevocacion = entity.MotivoRevocacion
    };

    private static RefreshTokenEntity ToEntity(RefreshToken domain) => new()
    {
        Id = domain.Id,
        UsuarioId = domain.UsuarioId,
        TokenHash = domain.TokenHash,
        FamilyId = domain.FamilyId,
        FechaCreacion = domain.FechaCreacion,
        FechaExpiracion = domain.FechaExpiracion,
        FechaUso = domain.FechaUso,
        ReemplazadoPorId = domain.ReemplazadoPorId,
        Revocado = domain.Revocado,
        FechaRevocacion = domain.FechaRevocacion,
        MotivoRevocacion = domain.MotivoRevocacion
    };
}
