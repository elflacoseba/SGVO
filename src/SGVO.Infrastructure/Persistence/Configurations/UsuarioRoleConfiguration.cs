using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGVO.Infrastructure.Persistence.Entities;

namespace SGVO.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración Fluent API para UsuarioRoleEntity.
/// </summary>
public class UsuarioRoleConfiguration : IEntityTypeConfiguration<UsuarioRoleEntity>
{
    public void Configure(EntityTypeBuilder<UsuarioRoleEntity> builder)
    {
        builder.ToTable("UsuarioRoles");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Activo)
            .HasDefaultValue(true);

        // Relationships
        builder.HasOne(e => e.Usuario)
            .WithMany(u => u.UsuarioRoleUsuarios)
            .HasForeignKey(e => e.UsuarioId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_UsuarioRoles_Usuario");

        builder.HasOne(e => e.Rol)
            .WithMany(r => r.UsuarioRoles)
            .HasForeignKey(e => e.RolId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_UsuarioRoles_Rol");

        builder.HasOne(e => e.EliminadoPorNavigation)
            .WithMany(u => u.UsuarioRoleEliminadoPorNavigations)
            .HasForeignKey(e => e.EliminadoPor)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_UsuarioRoles_EliminadoPor");
    }
}
