using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGVO.Domain.Entities;
using SGVO.Infrastructure.Persistence.Entities;

namespace SGVO.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración Fluent API para Cargo domain entity.
/// </summary>
public class CargoConfiguration : IEntityTypeConfiguration<Cargo>
{
    public void Configure(EntityTypeBuilder<Cargo> builder)
    {
        builder.ToTable("Cargos");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Nombre)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(e => e.Descripcion)
            .HasMaxLength(500);

        builder.Property(e => e.Activo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.CreadoEn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.ModificadoEn)
            .ValueGeneratedOnAddOrUpdate();

        // Relationships — Cargo has no navigation collections, so we configure
        // the inverse side from the other entity configurations.
        // FK for EliminadoPor is configured here since Cargo owns the FK property.
        builder.HasOne<UsuarioEntity>()
            .WithMany(u => u.Cargos)
            .HasForeignKey(e => e.EliminadoPor)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Cargos_EliminadoPor");
    }
}
