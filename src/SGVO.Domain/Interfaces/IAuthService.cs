using SGVO.Shared;

namespace SGVO.Domain.Interfaces;

/// <summary>
/// Servicio de autenticación para login, refresh y logout.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Autentica un usuario con nombre de usuario y contraseña.
    /// </summary>
    /// <param name="username">Nombre de usuario.</param>
    /// <param name="password">Contraseña en texto plano.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Resultado con el token de acceso, refresh token y fecha de expiración si la autenticación es exitosa.</returns>
    Task<Result<(string AccessToken, string RefreshToken, DateTime ExpiresAt)>> LoginAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Refresca el token de acceso utilizando un refresh token válido.
    /// </summary>
    /// <param name="refreshToken">Refresh token.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Resultado con el nuevo token de acceso, refresh token y fecha de expiración.</returns>
    Task<Result<(string AccessToken, string RefreshToken, DateTime ExpiresAt)>> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Cierra la sesión revocando el refresh token del usuario.
    /// </summary>
    /// <param name="userId">Identificador del usuario.</param>
    /// <param name="refreshToken">Refresh token a revocar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Resultado de la operación.</returns>
    Task<Result> LogoutAsync(
        ulong userId,
        string refreshToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un usuario por su identificador.
    /// </summary>
    /// <param name="userId">Identificador del usuario.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Resultado con el usuario si existe; fallo si no se encuentra.</returns>
    Task<Result<(ulong Id, string Username, string Email, IEnumerable<string> Roles)>> GetUserByIdAsync(
        ulong userId,
        CancellationToken cancellationToken = default);
}
