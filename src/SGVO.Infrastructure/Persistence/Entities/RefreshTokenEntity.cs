namespace SGVO.Infrastructure.Persistence.Entities;

/// <summary>
/// Entidad de persistencia para RefreshToken. Mapea a la tabla 'RefreshTokens' en MySQL.
/// </summary>
public class RefreshTokenEntity
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

    // Navigation properties
    public virtual UsuarioEntity Usuario { get; set; } = null!;
    public virtual RefreshTokenEntity? ReemplazadoPor { get; set; }
    public virtual ICollection<RefreshTokenEntity> InverseReemplazadoPor { get; set; } = new List<RefreshTokenEntity>();
}
