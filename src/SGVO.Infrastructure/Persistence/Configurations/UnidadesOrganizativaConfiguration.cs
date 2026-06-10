using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGVO.Infrastructure.Persistence.Entities;

namespace SGVO.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración Fluent API para UnidadesOrganizativaEntity.
/// </summary>
public class UnidadesOrganizativaConfiguration : IEntityTypeConfiguration<UnidadesOrganizativaEntity>
{
    public void Configure(EntityTypeBuilder<UnidadesOrganizativaEntity> builder)
    {
        builder.ToTable("UnidadesOrganizativas");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Nombre)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(e => e.Descripcion)
            .HasMaxLength(500);

        builder.Property(e => e.Tipo)
            .IsRequired()
            .HasMaxLength(50)
            .HasComment("Facultad, Secretaría, Dirección, Departamento, División, Área");

        builder.Property(e => e.NivelJerarquico)
            .HasDefaultValue(1);

        builder.Property(e => e.Activo)
            .HasDefaultValue(true);

        builder.Property(e => e.CreadoEn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.ModificadoEn)
            .ValueGeneratedOnAddOrUpdate();

        // Relationships
        builder.HasOne(e => e.EliminadoPorNavigation)
            .WithMany(u => u.UnidadesOrganizativas)
            .HasForeignKey(e => e.EliminadoPor)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_UnidadesOrganizativas_EliminadoPor");

        builder.HasOne(e => e.Padre)
            .WithMany(p => p.InversePadre)
            .HasForeignKey(e => e.PadreId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_UnidadesOrganizativas_Padre");

        builder.HasMany(e => e.Puestos)
            .WithOne(p => p.UnidadOrganizativa)
            .HasForeignKey(p => p.UnidadOrganizativaId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Puestos_Unidad");
    }
}
