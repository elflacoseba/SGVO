using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGVO.Domain.Entities;
using SGVO.Infrastructure.Persistence.Entities;

namespace SGVO.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración Fluent API para CargoSkillEntity.
/// </summary>
public class CargoSkillConfiguration : IEntityTypeConfiguration<CargoSkillEntity>
{
    public void Configure(EntityTypeBuilder<CargoSkillEntity> builder)
    {
        builder.ToTable("CargoSkills");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.NivelImportancia)
            .HasComment("1=Crítico, 2=Importante, 3=Deseable, 4=Secundario");

        builder.Property(e => e.Activo)
            .HasDefaultValue(true);

        // Relationships — Cargo is a domain entity without navigation collections,
        // so we configure the relationship from the CargoSkill side only.
        builder.HasOne(e => e.Cargo)
            .WithMany()
            .HasForeignKey(e => e.CargoId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_CargoSkills_Cargo");

        builder.HasOne(e => e.Skill)
            .WithMany()
            .HasForeignKey(e => e.SkillId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_CargoSkills_Skill");

        builder.HasOne(e => e.EliminadoPorNavigation)
            .WithMany(u => u.CargoSkills)
            .HasForeignKey(e => e.EliminadoPor)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_CargoSkills_EliminadoPor");
    }
}
