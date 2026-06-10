using SGVO.Application.Common;
using SGVO.Application.Features.Auth.Dtos;
using SGVO.Shared;

namespace SGVO.Application.Features.Auth.Commands;

/// <summary>
/// Comando para iniciar sesión.
/// </summary>
public sealed record LoginCommand(string Username, string Password) : ICommand<LoginResponseDto>;
