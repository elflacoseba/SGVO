namespace SGVO.Domain.Entities;

/// <summary>
/// Domain entity representing a refresh token.
/// Mapped to RefreshTokenEntity in the Infrastructure layer.
/// </summary>
public class RefreshToken
{
    public ulong Id { get; set; }
    public ulong UsuarioId { get; set; }
    public string TokenHash { get; set; } = null!;
    public string FamilyId { get; set; } = null!;
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaExpiracion { get; set; }
    public DateTime? FechaUso { get; set; }
    public ulong? ReemplazadoPorId { get; set; }
    public bool Revocado { get; set; }
    public DateTime? FechaRevocacion { get; set; }
    public string? MotivoRevocacion { get; set; }
}
