namespace SGVO.Domain.Interfaces;

/// <summary>
/// Servicio para generar y validar tokens JWT de acceso.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Genera un token JWT de acceso para el usuario.
    /// </summary>
    /// <param name="userId">Identificador del usuario.</param>
    /// <param name="username">Nombre de usuario.</param>
    /// <param name="roles">Roles asignados al usuario.</param>
    /// <returns>Tuple con el token JWT y su fecha de expiración en UTC.</returns>
    (string Token, DateTime ExpiresAt) GenerateAccessToken(
        long userId,
        string username,
        IEnumerable<string> roles);

    /// <summary>
    /// Valida un token JWT y extrae el identificador del usuario si es válido.
    /// </summary>
    /// <param name="token">Token JWT a validar.</param>
    /// <returns>Identificador del usuario si el token es válido; null en caso contrario.</returns>
    long? ValidateToken(string token);
}
