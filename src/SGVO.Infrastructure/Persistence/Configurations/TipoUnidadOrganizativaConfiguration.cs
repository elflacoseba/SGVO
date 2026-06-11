using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGVO.Infrastructure.Persistence.Entities;
using SGVO.Shared;

namespace SGVO.Infrastructure.Persistence.Configurations;

/// <summary>
/// Fluent API configuration for TipoUnidadOrganizativaEntity.
/// </summary>
public class TipoUnidadOrganizativaConfiguration : IEntityTypeConfiguration<TipoUnidadOrganizativaEntity>
{
    public void Configure(EntityTypeBuilder<TipoUnidadOrganizativaEntity> builder)
    {
        builder.ToTable("TiposUnidadOrganizativa");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Nombre)
            .IsRequired()
            .HasMaxLength(DomainConstants.TipoUnidadOrganizativaNombreMaxLength);

        builder.HasIndex(e => e.Nombre)
            .IsUnique()
            .HasDatabaseName("IX_TiposUnidadOrganizativa_Nombre");

        builder.Property(e => e.Activo)
            .HasDefaultValue(true);

        builder.Property(e => e.CreadoEn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.ModificadoEn)
            .ValueGeneratedOnAddOrUpdate();

        // Relationships
        builder.HasOne(e => e.EliminadoPorNavigation)
            .WithMany()
            .HasForeignKey(e => e.EliminadoPor)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_TiposUnidadOrganizativa_EliminadoPor");
    }
}
