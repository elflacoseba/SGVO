using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGVO.Infrastructure.Persistence.Entities;

namespace SGVO.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración Fluent API para PuestoEntity.
/// </summary>
public class PuestoConfiguration : IEntityTypeConfiguration<PuestoEntity>
{
    public void Configure(EntityTypeBuilder<PuestoEntity> builder)
    {
        builder.ToTable("Puestos");

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
            .WithMany(u => u.Puestos)
            .HasForeignKey(e => e.EliminadoPor)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Puestos_EliminadoPor");

        builder.HasOne(e => e.Cargo)
            .WithMany(c => c.Puestos)
            .HasForeignKey(e => e.CargoId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Puestos_Cargo");

        builder.HasOne(e => e.UnidadOrganizativa)
            .WithMany(u => u.Puestos)
            .HasForeignKey(e => e.UnidadOrganizativaId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Puestos_Unidad");

        builder.HasOne(e => e.Superior)
            .WithMany(p => p.InverseSuperior)
            .HasForeignKey(e => e.SuperiorId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Puestos_Superior");

        builder.HasMany(e => e.Ocupaciones)
            .WithOne(o => o.Puesto)
            .HasForeignKey(o => o.PuestoId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Ocupaciones_Puesto");

        builder.HasMany(e => e.Vacantes)
            .WithOne(v => v.Puesto)
            .HasForeignKey(v => v.PuestoId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Vacantes_Puesto");
    }
}
