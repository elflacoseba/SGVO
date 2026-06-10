using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGVO.Infrastructure.Persistence.Entities;

namespace SGVO.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración Fluent API para PersonaEntity.
/// </summary>
public class PersonaConfiguration : IEntityTypeConfiguration<PersonaEntity>
{
    public void Configure(EntityTypeBuilder<PersonaEntity> builder)
    {
        builder.ToTable("Personas");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Nombre)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(e => e.Apellido)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(e => e.Email)
            .HasMaxLength(200);

        builder.Property(e => e.Telefono)
            .HasMaxLength(50);

        builder.Property(e => e.Documento)
            .HasMaxLength(50);

        builder.Property(e => e.Activo)
            .HasDefaultValue(true);

        builder.Property(e => e.CreadoEn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.ModificadoEn)
            .ValueGeneratedOnAddOrUpdate();

        // Relationships
        builder.HasOne(e => e.EliminadoPorNavigation)
            .WithMany(u => u.Personas)
            .HasForeignKey(e => e.EliminadoPor)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Personas_EliminadoPor");

        builder.HasMany(e => e.Ocupaciones)
            .WithOne(o => o.Persona)
            .HasForeignKey(o => o.PersonaId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Ocupaciones_Persona");

        builder.HasMany(e => e.PersonaSkills)
            .WithOne(ps => ps.Persona)
            .HasForeignKey(ps => ps.PersonaId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PersonaSkills_Persona");

        builder.HasMany(e => e.Postulantes)
            .WithOne(p => p.Persona)
            .HasForeignKey(p => p.PersonaId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Postulantes_Persona");
    }
}
