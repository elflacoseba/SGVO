namespace SGVO.Domain.Interfaces;

/// <summary>
/// Servicio para hashear y verificar contraseñas.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashea una contraseña en texto plano.
    /// </summary>
    /// <param name="password">Contraseña en texto plano.</param>
    /// <returns>Hash de la contraseña.</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verifica si una contraseña coincide con un hash.
    /// </summary>
    /// <param name="password">Contraseña en texto plano.</param>
    /// <param name="hash">Hash almacenado.</param>
    /// <returns>true si la contraseña coincide con el hash; false en caso contrario.</returns>
    bool VerifyPassword(string password, string hash);
}
