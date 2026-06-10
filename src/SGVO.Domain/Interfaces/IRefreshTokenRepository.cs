namespace SGVO.Domain.Interfaces;

/// <summary>
/// Repositorio para gestionar los refresh tokens en la base de datos.
/// </summary>
public interface IRefreshTokenRepository
{
    /// <summary>
    /// Crea un nuevo refresh token en la base de datos.
    /// </summary>
    /// <param name="token">Entidad del refresh token.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Tarea completada.</returns>
    Task CreateAsync(
        object token,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un refresh token por su hash.
    /// </summary>
    /// <param name="hash">Hash del token.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Entidad del refresh token si existe; null en caso contrario.</returns>
    Task<object?> GetByTokenHashAsync(
        string hash,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoca un refresh token específico.
    /// </summary>
    /// <param name="tokenId">Identificador del token.</param>
    /// <param name="reason">Motivo de la revocación.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Tarea completada.</returns>
    Task RevokeAsync(
        ulong tokenId,
        string reason,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoca todos los refresh tokens de una familia.
    /// </summary>
    /// <param name="familyId">Identificador de la familia de tokens.</param>
    /// <param name="reason">Motivo de la revocación.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Tarea completada.</returns>
    Task RevokeFamilyAsync(
        string familyId,
        string reason,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todos los refresh tokens activos de un usuario.
    /// </summary>
    /// <param name="userId">Identificador del usuario.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Lista de refresh tokens activos.</returns>
    Task<IReadOnlyList<object>> GetActiveByUserIdAsync(
        ulong userId,
        CancellationToken cancellationToken = default);
}
