using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGVO.Api.Extensions;
using SGVO.Application.Common;
using SGVO.Application.Features.Auth.Commands;
using SGVO.Application.Features.Auth.Dtos;
using SGVO.Application.Features.Auth.Queries;
using SGVO.Shared;

namespace SGVO.Api.Controllers;

/// <summary>
/// Controlador para la autenticación y autorización de usuarios.
/// </summary>
[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly ICommandHandler<LoginCommand, LoginResponseDto> _loginHandler;
    private readonly ICommandHandler<RefreshTokenCommand, LoginResponseDto> _refreshHandler;
    private readonly ICommandHandler<LogoutCommand, Unit> _logoutHandler;
    private readonly IQueryHandler<GetCurrentUserQuery, UserDto?> _currentUserHandler;

    public AuthController(
        ICommandHandler<LoginCommand, LoginResponseDto> loginHandler,
        ICommandHandler<RefreshTokenCommand, LoginResponseDto> refreshHandler,
        ICommandHandler<LogoutCommand, Unit> logoutHandler,
        IQueryHandler<GetCurrentUserQuery, UserDto?> currentUserHandler)
    {
        _loginHandler = loginHandler;
        _refreshHandler = refreshHandler;
        _logoutHandler = logoutHandler;
        _currentUserHandler = currentUserHandler;
    }

    /// <summary>
    /// Inicia sesión con nombre de usuario y contraseña.
    /// </summary>
    /// <param name="request">Datos de inicio de sesión.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Token de acceso y refresh token.</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken ct)
    {
        var command = new LoginCommand(request.Username, request.Password);
        var result = await _loginHandler.Handle(command, ct);

        if (result.IsFailure)
            return result.ToErrorActionResult();

        return Ok(result.Value);
    }

    /// <summary>
    /// Refresca el token de acceso utilizando un refresh token válido.
    /// </summary>
    /// <param name="request">Refresh token.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Nuevo token de acceso y refresh token.</returns>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto request, CancellationToken ct)
    {
        var command = new RefreshTokenCommand(request.RefreshToken);
        var result = await _refreshHandler.Handle(command, ct);

        if (result.IsFailure)
            return result.ToErrorActionResult();

        return Ok(result.Value);
    }

    /// <summary>
    /// Cierra la sesión revocando el refresh token.
    /// </summary>
    /// <param name="request">Refresh token a revocar.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Resultado de la operación.</returns>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto request, CancellationToken ct)
    {
        var userId = User.GetUserId();
        if (!userId.HasValue)
            return Unauthorized(new { error = "No se pudo identificar al usuario." });

        var command = new LogoutCommand(userId.Value, request.RefreshToken);
        var result = await _logoutHandler.Handle(command, ct);

        if (result.IsFailure)
            return result.ToErrorActionResult();

        return Ok(new { message = "Sesión cerrada correctamente." });
    }

    /// <summary>
    /// Obtiene la información del usuario autenticado actual.
    /// </summary>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Información del usuario.</returns>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser(CancellationToken ct)
    {
        var userId = User.GetUserId();
        if (!userId.HasValue)
            return Unauthorized(new { error = "No se pudo identificar al usuario." });

        var query = new GetCurrentUserQuery(userId.Value);
        var result = await _currentUserHandler.Handle(query, ct);

        if (result.IsFailure)
            return result.ToErrorActionResult();

        return result.Value is null
            ? NotFound(new { error = "Usuario no encontrado." })
            : Ok(result.Value);
    }
}
