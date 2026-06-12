using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGVO.Domain.Entities;
using SGVO.Infrastructure.Persistence.Entities;

namespace SGVO.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración Fluent API para Skill domain entity.
/// </summary>
public class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        builder.ToTable("Skills");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Nombre)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(e => e.Categoria)
            .HasMaxLength(100)
            .HasComment("Técnica, Blanda, Gerencial, etc.");

        builder.Property(e => e.Descripcion)
            .HasMaxLength(500);

        builder.Property(e => e.Activo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.CreadoEn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.ModificadoEn)
            .ValueGeneratedOnAddOrUpdate();

        // Relationships — Skill has no navigation collections, so we configure
        // the inverse side from the other entity configurations.
        // FK for EliminadoPor is configured here since Skill owns the FK property.
        builder.HasOne<UsuarioEntity>()
            .WithMany(u => u.Skills)
            .HasForeignKey(e => e.EliminadoPor)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Skills_EliminadoPor");
    }
}
