using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGVO.Infrastructure.Persistence.Entities;

namespace SGVO.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración Fluent API para PostulacioneEntity.
/// </summary>
public class PostulacioneConfiguration : IEntityTypeConfiguration<PostulacioneEntity>
{
    public void Configure(EntityTypeBuilder<PostulacioneEntity> builder)
    {
        builder.ToTable("Postulaciones");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Estado)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Postulado")
            .HasComment("Postulado, Preseleccionado, Entrevistado, Aprobado, Rechazado, Contratado");

        builder.Property(e => e.PuntajeMatch)
            .HasColumnType("decimal(5,2)")
            .HasComment("0.00 - 100.00");

        builder.Property(e => e.Observaciones)
            .HasMaxLength(1000);

        builder.Property(e => e.FechaPostulacion)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.Activo)
            .HasDefaultValue(true);

        builder.Property(e => e.CreadoEn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.ModificadoEn)
            .ValueGeneratedOnAddOrUpdate();

        // Relationships
        builder.HasOne(e => e.EliminadoPorNavigation)
            .WithMany(u => u.Postulaciones)
            .HasForeignKey(e => e.EliminadoPor)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Postulaciones_EliminadoPor");

        builder.HasOne(e => e.Evaluador)
            .WithMany()
            .HasForeignKey(e => e.EvaluadorId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Postulaciones_Evaluador");

        builder.HasOne(e => e.Postulante)
            .WithMany(p => p.Postulaciones)
            .HasForeignKey(e => e.PostulanteId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Postulaciones_Postulante");

        builder.HasOne(e => e.Vacante)
            .WithMany(v => v.Postulaciones)
            .HasForeignKey(e => e.VacanteId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Postulaciones_Vacante");
    }
}
