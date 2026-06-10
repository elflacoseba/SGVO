using SGVO.Application.Common;
using SGVO.Shared;

namespace SGVO.Application.Features.Auth.Commands;

/// <summary>
/// Comando para cerrar sesión.
/// </summary>
public sealed class LogoutCommand : ICommand<Result>
{
    public ulong UserId { get; }
    public string RefreshToken { get; }

    public LogoutCommand(ulong userId, string refreshToken)
    {
        UserId = userId;
        RefreshToken = refreshToken;
    }
}
