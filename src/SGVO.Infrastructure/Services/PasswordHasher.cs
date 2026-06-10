using System.Security.Cryptography;
using System.Text;
using SGVO.Domain.Interfaces;

namespace SGVO.Infrastructure.Services;

/// <summary>
/// Servicio para hashear y verificar contraseñas usando SHA-256.
/// Nota: Esta implementación es para desarrollo. En producción usar BCrypt o Argon2.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }

    public bool VerifyPassword(string password, string hash)
    {
        var computedHash = HashPassword(password);
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(computedHash),
            Encoding.UTF8.GetBytes(hash));
    }
}
