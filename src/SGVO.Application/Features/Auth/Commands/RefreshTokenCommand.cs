using SGVO.Application.Common;
using SGVO.Application.Features.Auth.Dtos;
using SGVO.Shared;

namespace SGVO.Application.Features.Auth.Commands;

/// <summary>
/// Comando para refrescar el token de acceso.
/// </summary>
public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<LoginResponseDto>;
