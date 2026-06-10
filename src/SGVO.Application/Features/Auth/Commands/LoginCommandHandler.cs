using SGVO.Application.Common;
using SGVO.Application.Features.Auth;
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

    public LoginCommandHandler(IAuthService authService)
    {
        _authService = authService;
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

        var (accessToken, refreshToken, expiresAt) = result.Value;

        return Result<LoginResponseDto>.Success(
            LoginResponseMapper.Map(accessToken, refreshToken, expiresAt));
    }
}
