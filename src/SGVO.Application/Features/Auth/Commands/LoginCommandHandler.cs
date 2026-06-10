using SGVO.Application.Common;
using SGVO.Application.Features.Auth.Dtos;
using SGVO.Domain.Interfaces;
using SGVO.Shared;

namespace SGVO.Application.Features.Auth.Commands;

/// <summary>
/// Handler para procesar el comando de inicio de sesión.
/// </summary>
public class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResponseDto>
{
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(IAuthService authService, ITokenService tokenService)
    {
        _authService = authService;
        _tokenService = tokenService;
    }

    public async Task<Result<LoginResponseDto>> Handle(
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(
            command.Username,
            command.Password,
            cancellationToken);

        if (result.IsFailure)
            return Result<LoginResponseDto>.Failure(result.Error!, result.ErrorCode);

        var (accessToken, refreshToken) = result.Value;

        var response = new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = _tokenService.GetAccessTokenExpiration(),
            TokenType = "Bearer"
        };

        return Result<LoginResponseDto>.Success(response);
    }
}
