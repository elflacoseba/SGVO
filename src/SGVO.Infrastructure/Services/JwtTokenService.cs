using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SGVO.Domain.Interfaces;

namespace SGVO.Infrastructure.Services;

/// <summary>
/// Servicio para generar y validar tokens JWT.
/// </summary>
public class JwtTokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtTokenService> _logger;

    public JwtTokenService(IConfiguration configuration, ILogger<JwtTokenService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private int GetExpirationMinutes()
    {
        if (int.TryParse(_configuration["Jwt:AccessTokenExpirationMinutes"], out var minutes))
            return minutes;
        return 15;
    }

    public (string Token, DateTime ExpiresAt) GenerateAccessToken(
        ulong userId,
        string username,
        IEnumerable<string> roles)
    {
        var secret = _configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException("JWT Secret no está configurado.");

        var issuer = _configuration["Jwt:Issuer"] ?? "SGVO";
        var audience = _configuration["Jwt:Audience"] ?? "SGVO-API";
        var expirationMinutes = GetExpirationMinutes();
        var expiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("uid", userId.ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }

    public ulong? ValidateToken(string token)
    {
        var secret = _configuration["Jwt:Secret"];
        if (string.IsNullOrEmpty(secret))
            return null;

        var issuer = _configuration["Jwt:Issuer"] ?? "SGVO";
        var audience = _configuration["Jwt:Audience"] ?? "SGVO-API";

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(2)
            }, out _);

            var userIdClaim = principal.FindFirst("uid")?.Value
                ?? principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (ulong.TryParse(userIdClaim, out var userId))
                return userId;

            return null;
        }
        catch (SecurityTokenExpiredException ex)
        {
            _logger.LogDebug(ex, "Token validation failed: token expired");
            return null;
        }
        catch (SecurityTokenInvalidSignatureException ex)
        {
            _logger.LogDebug(ex, "Token validation failed: invalid signature");
            return null;
        }
        catch (SecurityTokenException ex)
        {
            _logger.LogDebug(ex, "Token validation failed: {Message}", ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Token validation failed with unexpected error");
            return null;
        }
    }
}
