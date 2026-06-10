using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGVO.Infrastructure.Persistence.Entities;

namespace SGVO.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración Fluent API para PostulanteEntity.
/// </summary>
public class PostulanteConfiguration : IEntityTypeConfiguration<PostulanteEntity>
{
    public void Configure(EntityTypeBuilder<PostulanteEntity> builder)
    {
        builder.ToTable("Postulantes");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Nombre)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(e => e.Apellido)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Telefono)
            .HasMaxLength(50);

        builder.Property(e => e.Origen)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Externo")
            .HasComment("Interno, Externo, Recomendado");

        builder.Property(e => e.CurriculumUrl)
            .HasMaxLength(500);

        builder.Property(e => e.PersonaId)
            .HasComment("NULL si es externo");

        builder.Property(e => e.Activo)
            .HasDefaultValue(true);

        builder.Property(e => e.CreadoEn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Indexes
        builder.HasIndex(e => e.PersonaId)
            .HasDatabaseName("IX_Postulantes_PersonaId");

        // Relationships
        builder.HasOne(e => e.EliminadoPorNavigation)
            .WithMany(u => u.Postulantes)
            .HasForeignKey(e => e.EliminadoPor)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Postulantes_EliminadoPor");

        builder.HasOne(e => e.Persona)
            .WithMany(p => p.Postulantes)
            .HasForeignKey(e => e.PersonaId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Postulantes_Persona");

        builder.HasMany(e => e.Postulaciones)
            .WithOne(p => p.Postulante)
            .HasForeignKey(p => p.PostulanteId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Postulaciones_Postulante");
    }
}
