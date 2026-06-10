using SGVO.Application.Common;
using SGVO.Domain.Interfaces;
using SGVO.Shared;

namespace SGVO.Application.Features.Auth.Commands;

/// <summary>
/// Handler para procesar el comando de cierre de sesión.
/// </summary>
public class LogoutCommandHandler : ICommandHandler<LogoutCommand, Result>
{
    private readonly IAuthService _authService;

    public LogoutCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result<Result>> Handle(
        LogoutCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _authService.LogoutAsync(
            command.UserId,
            command.RefreshToken,
            cancellationToken);

        if (result.IsFailure)
            return Result<Result>.Failure(result.Error!, result.ErrorCode);

        return Result<Result>.Success(Result.Success());
    }
}
