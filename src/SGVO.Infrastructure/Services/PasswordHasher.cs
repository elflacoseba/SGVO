using SGVO.Domain.Interfaces;

namespace SGVO.Infrastructure.Services;

/// <summary>
/// Servicio para hashear y verificar contraseñas usando BCrypt (work factor 11).
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 11);
    }

    public bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
