using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGVO.Infrastructure.Persistence.Entities;

namespace SGVO.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración Fluent API para PersonaSkillEntity.
/// </summary>
public class PersonaSkillConfiguration : IEntityTypeConfiguration<PersonaSkillEntity>
{
    public void Configure(EntityTypeBuilder<PersonaSkillEntity> builder)
    {
        builder.ToTable("PersonaSkills");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.NivelDominio)
            .HasComment("1=Básico, 2=Intermedio, 3=Avanzado, 4=Experto");

        builder.Property(e => e.Activo)
            .HasDefaultValue(true);

        // Relationships
        builder.HasOne(e => e.Persona)
            .WithMany(p => p.PersonaSkills)
            .HasForeignKey(e => e.PersonaId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PersonaSkills_Persona");

        builder.HasOne(e => e.Skill)
            .WithMany(s => s.PersonaSkills)
            .HasForeignKey(e => e.SkillId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PersonaSkills_Skill");

        builder.HasOne(e => e.EliminadoPorNavigation)
            .WithMany(u => u.PersonaSkills)
            .HasForeignKey(e => e.EliminadoPor)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PersonaSkills_EliminadoPor");
    }
}
