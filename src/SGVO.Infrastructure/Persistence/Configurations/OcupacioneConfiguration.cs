using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGVO.Infrastructure.Persistence.Entities;

namespace SGVO.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración Fluent API para OcupacioneEntity.
/// </summary>
public class OcupacioneConfiguration : IEntityTypeConfiguration<OcupacioneEntity>
{
    public void Configure(EntityTypeBuilder<OcupacioneEntity> builder)
    {
        builder.ToTable("Ocupaciones");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.TipoOcupacion)
            .IsRequired()
            .HasMaxLength(50)
            .HasComment("Permanente, Interina, Suplente");

        builder.Property(e => e.FechaFin)
            .HasComment("NULL = vigente");

        builder.Property(e => e.Activo)
            .HasDefaultValue(true);

        builder.Property(e => e.CreadoEn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Relationships
        builder.HasOne(e => e.EliminadoPorNavigation)
            .WithMany(u => u.Ocupaciones)
            .HasForeignKey(e => e.EliminadoPor)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Ocupaciones_EliminadoPor");

        builder.HasOne(e => e.Persona)
            .WithMany(p => p.Ocupaciones)
            .HasForeignKey(e => e.PersonaId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Ocupaciones_Persona");

        builder.HasOne(e => e.Puesto)
            .WithMany(p => p.Ocupaciones)
            .HasForeignKey(e => e.PuestoId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Ocupaciones_Puesto");
    }
}
