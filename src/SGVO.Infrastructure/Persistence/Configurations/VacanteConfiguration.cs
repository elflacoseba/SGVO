using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGVO.Infrastructure.Persistence.Entities;

namespace SGVO.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración Fluent API para VacanteEntity.
/// </summary>
public class VacanteConfiguration : IEntityTypeConfiguration<VacanteEntity>
{
    public void Configure(EntityTypeBuilder<VacanteEntity> builder)
    {
        builder.ToTable("Vacantes");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Motivo)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.Estado)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Abierta")
            .HasComment("Abierta, EnSeleccion, Cubierta, Cancelada");

        builder.Property(e => e.Observaciones)
            .HasMaxLength(1000);

        builder.Property(e => e.FechaApertura)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.Activo)
            .HasDefaultValue(true);

        builder.Property(e => e.CreadoEn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.ModificadoEn)
            .ValueGeneratedOnAddOrUpdate();

        // Indexes
        builder.HasIndex(e => e.Estado)
            .HasDatabaseName("IX_Vacantes_Estado");

        builder.HasIndex(e => new { e.PuestoId, e.Estado })
            .HasDatabaseName("IX_Vacantes_PuestoEstado");

        builder.HasIndex(e => e.ResponsableId)
            .HasDatabaseName("FK_Vacantes_Responsable");

        // Relationships
        builder.HasOne(e => e.EliminadoPorNavigation)
            .WithMany(u => u.Vacantes)
            .HasForeignKey(e => e.EliminadoPor)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Vacantes_EliminadoPor");

        builder.HasOne(e => e.Puesto)
            .WithMany(p => p.Vacantes)
            .HasForeignKey(e => e.PuestoId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Vacantes_Puesto");

        builder.HasOne(e => e.Responsable)
            .WithMany()
            .HasForeignKey(e => e.ResponsableId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Vacantes_Responsable");

        builder.HasMany(e => e.Postulaciones)
            .WithOne(p => p.Vacante)
            .HasForeignKey(p => p.VacanteId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Postulaciones_Vacante");
    }
}
