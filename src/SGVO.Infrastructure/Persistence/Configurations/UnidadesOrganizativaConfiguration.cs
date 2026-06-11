using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGVO.Infrastructure.Persistence.Entities;
using SGVO.Shared;

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
            .HasMaxLength(DomainConstants.UnidadOrganizativaNombreMaxLength);

        builder.Property(e => e.TipoUnidadOrganizativaId)
            .IsRequired();

        builder.Property(e => e.NivelJerarquico)
            .HasDefaultValue(1);

        builder.Property(e => e.Activo)
            .HasDefaultValue(true);

        builder.Property(e => e.CreadoEn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.ModificadoEn)
            .ValueGeneratedOnAddOrUpdate();

        // Indexes
        builder.HasIndex(e => e.PadreId).HasDatabaseName("IX_UnidadesOrganizativas_PadreId");
        builder.HasIndex(e => e.NivelJerarquico).HasDatabaseName("IX_UnidadesOrganizativas_NivelJerarquico");

        // Relationships
        builder.HasOne(e => e.TipoUnidadOrganizativa)
            .WithMany(t => t.UnidadesOrganizativas)
            .HasForeignKey(e => e.TipoUnidadOrganizativaId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_UnidadesOrganizativas_Tipo");

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
