using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGVO.Infrastructure.Persistence.Entities;

namespace SGVO.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración Fluent API para RoleEntity.
/// </summary>
public class RoleConfiguration : IEntityTypeConfiguration<RoleEntity>
{
    public void Configure(EntityTypeBuilder<RoleEntity> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Descripcion)
            .HasMaxLength(500);

        builder.Property(e => e.Activo)
            .HasDefaultValue(true);

        builder.Property(e => e.CreadoEn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Relationships
        builder.HasOne(e => e.EliminadoPorNavigation)
            .WithMany(u => u.Roles)
            .HasForeignKey(e => e.EliminadoPor)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Roles_EliminadoPor");

        builder.HasMany(e => e.UsuarioRoles)
            .WithOne(ur => ur.Rol)
            .HasForeignKey(ur => ur.RolId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_UsuarioRoles_Rol");
    }
}
