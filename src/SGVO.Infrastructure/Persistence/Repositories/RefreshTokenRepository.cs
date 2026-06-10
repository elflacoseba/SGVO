using Microsoft.EntityFrameworkCore;
using SGVO.Domain.Interfaces;
using SGVO.Infrastructure.Persistence;
using SGVO.Infrastructure.Persistence.Entities;

namespace SGVO.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repositorio para gestionar los refresh tokens en la base de datos.
/// </summary>
public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly SgvoDbContext _dbContext;

    public RefreshTokenRepository(SgvoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateAsync(object token, CancellationToken cancellationToken = default)
    {
        if (token is RefreshTokenEntity entity)
        {
            _dbContext.RefreshTokens.Add(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        else
        {
            throw new ArgumentException("El token debe ser de tipo RefreshTokenEntity.", nameof(token));
        }
    }

    public async Task<object?> GetByTokenHashAsync(string hash, CancellationToken cancellationToken = default)
    {
        return await _dbContext.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(
                t => t.TokenHash == hash,
                cancellationToken);
    }

    public async Task RevokeAsync(ulong tokenId, string reason, CancellationToken cancellationToken = default)
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

    public async Task RevokeFamilyAsync(string familyId, string reason, CancellationToken cancellationToken = default)
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

    public async Task<IReadOnlyList<object>> GetActiveByUserIdAsync(ulong userId, CancellationToken cancellationToken = default)
    {
        var tokens = await _dbContext.RefreshTokens
            .AsNoTracking()
            .Where(t => t.UsuarioId == userId && t.Revocado == false && t.FechaExpiracion > DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        return tokens.Cast<object>().ToList();
    }
}
