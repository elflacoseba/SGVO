using SGVO.Application.Common;
using SGVO.Application.Features.Auth.Dtos;
using SGVO.Shared;

namespace SGVO.Application.Features.Auth.Commands;

/// <summary>
/// Comando para iniciar sesión.
/// </summary>
public sealed class LoginCommand : ICommand<LoginResponseDto>
{
    public string Username { get; }
    public string Password { get; }

    public LoginCommand(string username, string password)
    {
        Username = username;
        Password = password;
    }
}
