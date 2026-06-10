using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGVO.Infrastructure.Persistence.Entities;

namespace SGVO.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración Fluent API para RefreshTokenEntity.
/// </summary>
public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshTokenEntity>
{
    public void Configure(EntityTypeBuilder<RefreshTokenEntity> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.UsuarioId)
            .IsRequired();

        builder.Property(e => e.TokenHash)
            .IsRequired()
            .HasMaxLength(64)
            .IsFixedLength();

        builder.Property(e => e.FamilyId)
            .IsRequired()
            .HasMaxLength(36)
            .IsFixedLength();

        builder.Property(e => e.FechaCreacion)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.FechaExpiracion)
            .IsRequired();

        builder.Property(e => e.Revocado)
            .HasDefaultValue(false);

        builder.Property(e => e.MotivoRevocacion)
            .HasMaxLength(100);

        // Unique index on TokenHash
        builder.HasIndex(e => e.TokenHash)
            .IsUnique()
            .HasDatabaseName("IX_RefreshTokens_TokenHash");

        // Relationships
        builder.HasOne(e => e.Usuario)
            .WithMany()
            .HasForeignKey(e => e.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_RefreshTokens_Usuario");

        builder.HasOne(e => e.ReemplazadoPor)
            .WithMany(e => e.InverseReemplazadoPor)
            .HasForeignKey(e => e.ReemplazadoPorId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_RefreshTokens_ReemplazadoPor");
    }
}
