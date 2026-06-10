using SGVO.Application.Common;
using SGVO.Application.Features.Auth.Dtos;
using SGVO.Domain.Interfaces;
using SGVO.Shared;

namespace SGVO.Application.Features.Auth.Commands;

/// <summary>
/// Handler para procesar el comando de refresco de token.
/// </summary>
public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, LoginResponseDto>
{
    private readonly IAuthService _authService;

    public RefreshTokenCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result<LoginResponseDto>> Handle(
        RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _authService.RefreshTokenAsync(
            command.RefreshToken,
            cancellationToken);

        if (result.IsFailure)
            return Result<LoginResponseDto>.Failure(result.Error!, result.ErrorCode);

        var (accessToken, refreshToken) = result.Value;

        var response = new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            TokenType = "Bearer"
        };

        return Result<LoginResponseDto>.Success(response);
    }
}
