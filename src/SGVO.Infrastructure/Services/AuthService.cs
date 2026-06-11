using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SGVO.Domain.Interfaces;
using SGVO.Infrastructure.Persistence;
using SGVO.Infrastructure.Persistence.Entities;
using SGVO.Shared;

namespace SGVO.Infrastructure.Services;

/// <summary>
/// Servicio de autenticación que implementa login, refresh y logout.
/// </summary>
public class AuthService : IAuthService
{
    private readonly SgvoDbContext _dbContext;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IConfiguration _configuration;

    public AuthService(
        SgvoDbContext dbContext,
        ITokenService tokenService,
        IPasswordHasher passwordHasher,
        IConfiguration configuration)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }

    public async Task<Result<(string AccessToken, string RefreshToken, DateTime ExpiresAt)>> LoginAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        var usuario = await _dbContext.Usuarios
            .AsNoTracking()
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(
                u => u.NombreUsuario == username && u.Activo == true,
                cancellationToken);

        if (usuario is null)
            return Result<(string, string, DateTime)>.Failure("Credenciales inválidas.", "UNAUTHORIZED");

        if (!_passwordHasher.VerifyPassword(password, usuario.PasswordHash))
            return Result<(string, string, DateTime)>.Failure("Credenciales inválidas.", "UNAUTHORIZED");

        var roles = usuario.Roles
            .Where(r => r.Activo == true)
            .Select(r => r.Nombre)
            .ToList();

        var (accessToken, expiresAt) = _tokenService.GenerateAccessToken(usuario.Id, usuario.NombreUsuario, roles);
        var refreshToken = GenerateRefreshToken();
        var familyId = Guid.NewGuid().ToString();
        var refreshDays = 7;
        if (int.TryParse(_configuration["Jwt:RefreshTokenExpirationDays"], out var parsedDays))
            refreshDays = parsedDays;

        var refreshTokenEntity = new RefreshTokenEntity
        {
            UsuarioId = usuario.Id,
            TokenHash = HashRefreshToken(refreshToken),
            FamilyId = familyId,
            FechaCreacion = DateTime.UtcNow,
            FechaExpiracion = DateTime.UtcNow.AddDays(refreshDays),
            Revocado = false
        };

        _dbContext.RefreshTokens.Add(refreshTokenEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<(string, string, DateTime)>.Success((accessToken, refreshToken, expiresAt));
    }

    public async Task<Result<(string AccessToken, string RefreshToken, DateTime ExpiresAt)>> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = HashRefreshToken(refreshToken);

        var storedToken = await _dbContext.RefreshTokens
            .Include(t => t.Usuario)
            .ThenInclude(u => u.Roles)
            .FirstOrDefaultAsync(
                t => t.TokenHash == tokenHash && t.Revocado == false,
                cancellationToken);

        if (storedToken is null)
            return Result<(string, string, DateTime)>.Failure("Refresh token inválido.", "UNAUTHORIZED");

        if (storedToken.FechaExpiracion < DateTime.UtcNow)
            return Result<(string, string, DateTime)>.Failure("Refresh token expirado.", "UNAUTHORIZED");

        if (storedToken.FechaUso.HasValue)
        {
            // Reutilización detectada: revocar toda la familia
            await RevokeFamilyAsync(storedToken.FamilyId, "Reutilización detectada", cancellationToken);
            return Result<(string, string, DateTime)>.Failure("Refresh token reutilizado. Revocado por seguridad.", "UNAUTHORIZED");
        }

        // Marcar el token actual como usado
        storedToken.FechaUso = DateTime.UtcNow;

        var usuario = storedToken.Usuario;
        var roles = usuario.Roles
            .Where(r => r.Activo == true)
            .Select(r => r.Nombre)
            .ToList();

        var (newAccessToken, expiresAt) = _tokenService.GenerateAccessToken(usuario.Id, usuario.NombreUsuario, roles);
        var newRefreshToken = GenerateRefreshToken();
        var refreshDays = 7;
        if (int.TryParse(_configuration["Jwt:RefreshTokenExpirationDays"], out var parsedDays))
            refreshDays = parsedDays;

        var newRefreshTokenEntity = new RefreshTokenEntity
        {
            UsuarioId = usuario.Id,
            TokenHash = HashRefreshToken(newRefreshToken),
            FamilyId = storedToken.FamilyId,
            FechaCreacion = DateTime.UtcNow,
            FechaExpiracion = DateTime.UtcNow.AddDays(refreshDays),
            Revocado = false,
            ReemplazadoPorId = storedToken.Id
        };

        _dbContext.RefreshTokens.Add(newRefreshTokenEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<(string, string, DateTime)>.Success((newAccessToken, newRefreshToken, expiresAt));
    }

    public async Task<Result> LogoutAsync(
        long userId,
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = HashRefreshToken(refreshToken);

        var storedToken = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(
                t => t.TokenHash == tokenHash && t.UsuarioId == userId,
                cancellationToken);

        if (storedToken is null)
            return Result.Failure("Refresh token no encontrado.", "NOT_FOUND");

        storedToken.Revocado = true;
        storedToken.FechaRevocacion = DateTime.UtcNow;
        storedToken.MotivoRevocacion = "Cierre de sesión";

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<(long Id, string Username, string Email, IEnumerable<string> Roles)>> GetUserByIdAsync(
        long userId,
        CancellationToken cancellationToken = default)
    {
        var usuario = await _dbContext.Usuarios
            .AsNoTracking()
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(
                u => u.Id == userId && u.Activo == true,
                cancellationToken);

        if (usuario is null)
            return Result<(long, string, string, IEnumerable<string>)>.Failure("Usuario no encontrado.", "NOT_FOUND");

        var roles = usuario.Roles
            .Where(r => r.Activo == true)
            .Select(r => r.Nombre)
            .ToList();

        return Result<(long, string, string, IEnumerable<string>)>.Success(
            (usuario.Id, usuario.NombreUsuario, usuario.Email ?? string.Empty, roles));
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        RandomNumberGenerator.Fill(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private static string HashRefreshToken(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }

    private async Task RevokeFamilyAsync(
        string familyId,
        string reason,
        CancellationToken cancellationToken)
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
}
