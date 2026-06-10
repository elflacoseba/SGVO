using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGVO.Infrastructure.Persistence.Entities;

namespace SGVO.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración Fluent API para UsuarioEntity.
/// </summary>
public class UsuarioConfiguration : IEntityTypeConfiguration<UsuarioEntity>
{
    public void Configure(EntityTypeBuilder<UsuarioEntity> builder)
    {
        builder.ToTable("Usuarios");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.NombreUsuario)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("Username");

        builder.Property(e => e.Email)
            .HasMaxLength(200)
            .HasColumnName("Email");

        builder.Property(e => e.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.PersonaId)
            .HasComment("Vincula con Personas si el usuario es empleado");

        builder.Property(e => e.Activo)
            .HasDefaultValue(true);

        builder.Property(e => e.CreadoEn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.ModificadoEn)
            .ValueGeneratedOnAddOrUpdate();

        // Relationships
        builder.HasOne(e => e.EliminadoPorNavigation)
            .WithMany(u => u.InverseEliminadoPorNavigation)
            .HasForeignKey(e => e.EliminadoPor)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Usuarios_EliminadoPor");

        builder.HasOne(e => e.Persona)
            .WithMany(p => p.Usuarios)
            .HasForeignKey(e => e.PersonaId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Usuarios_Persona");

        builder.HasMany(e => e.Auditoria)
            .WithOne(a => a.Usuario)
            .HasForeignKey(a => a.UsuarioId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Auditorias_Usuario");
    }
}
