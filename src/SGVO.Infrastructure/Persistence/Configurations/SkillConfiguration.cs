using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGVO.Infrastructure.Persistence.Entities;

namespace SGVO.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración Fluent API para SkillEntity.
/// </summary>
public class SkillConfiguration : IEntityTypeConfiguration<SkillEntity>
{
    public void Configure(EntityTypeBuilder<SkillEntity> builder)
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
            .HasDefaultValue(true);

        builder.Property(e => e.CreadoEn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Relationships
        builder.HasOne(e => e.EliminadoPorNavigation)
            .WithMany(u => u.Skills)
            .HasForeignKey(e => e.EliminadoPor)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Skills_EliminadoPor");

        builder.HasMany(e => e.CargoSkills)
            .WithOne(cs => cs.Skill)
            .HasForeignKey(cs => cs.SkillId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_CargoSkills_Skill");

        builder.HasMany(e => e.PersonaSkills)
            .WithOne(ps => ps.Skill)
            .HasForeignKey(ps => ps.SkillId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PersonaSkills_Skill");
    }
}
