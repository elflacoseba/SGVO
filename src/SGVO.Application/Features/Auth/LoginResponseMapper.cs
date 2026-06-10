using SGVO.Application.Features.Auth.Dtos;
using SGVO.Domain.Interfaces;

namespace SGVO.Application.Features.Auth;

/// <summary>
/// Shared mapper for building LoginResponseDto from token service results.
/// Eliminates duplication between LoginCommandHandler and RefreshTokenCommandHandler.
/// </summary>
public static class LoginResponseMapper
{
    public static LoginResponseDto Map(
        string accessToken,
        string refreshToken,
        DateTime expiresAt)
    {
        return new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            TokenType = "Bearer"
        };
    }
}
