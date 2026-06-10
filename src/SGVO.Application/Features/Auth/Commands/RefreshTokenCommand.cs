using SGVO.Application.Common;
using SGVO.Application.Features.Auth.Dtos;
using SGVO.Shared;

namespace SGVO.Application.Features.Auth.Commands;

/// <summary>
/// Comando para refrescar el token de acceso.
/// </summary>
public sealed class RefreshTokenCommand : ICommand<LoginResponseDto>
{
    public string RefreshToken { get; }

    public RefreshTokenCommand(string refreshToken)
    {
        RefreshToken = refreshToken;
    }
}
