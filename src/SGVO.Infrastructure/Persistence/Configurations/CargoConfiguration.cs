using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGVO.Infrastructure.Persistence.Entities;

namespace SGVO.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración Fluent API para CargoEntity.
/// </summary>
public class CargoConfiguration : IEntityTypeConfiguration<CargoEntity>
{
    public void Configure(EntityTypeBuilder<CargoEntity> builder)
    {
        builder.ToTable("Cargos");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Nombre)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(e => e.Descripcion)
            .HasMaxLength(500);

        builder.Property(e => e.Activo)
            .HasDefaultValue(true);

        builder.Property(e => e.CreadoEn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.ModificadoEn)
            .ValueGeneratedOnAddOrUpdate();

        // Relationships
        builder.HasOne(e => e.EliminadoPorNavigation)
            .WithMany(u => u.Cargos)
            .HasForeignKey(e => e.EliminadoPor)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Cargos_EliminadoPor");

        builder.HasMany(e => e.CargoSkills)
            .WithOne(cs => cs.Cargo)
            .HasForeignKey(cs => cs.CargoId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_CargoSkills_Cargo");

        builder.HasMany(e => e.Puestos)
            .WithOne(p => p.Cargo)
            .HasForeignKey(p => p.CargoId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Puestos_Cargo");
    }
}
