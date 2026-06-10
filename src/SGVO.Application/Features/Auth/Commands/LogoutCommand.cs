using SGVO.Application.Common;
using SGVO.Shared;

namespace SGVO.Application.Features.Auth.Commands;

/// <summary>
/// Comando para cerrar sesión.
/// </summary>
public sealed record LogoutCommand(ulong UserId, string RefreshToken) : ICommand<Result>;
