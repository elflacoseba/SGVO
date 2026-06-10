using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGVO.Infrastructure.Persistence.Entities;

namespace SGVO.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración Fluent API para AuditoriaEntity.
/// </summary>
public class AuditoriaConfiguration : IEntityTypeConfiguration<AuditoriaEntity>
{
    public void Configure(EntityTypeBuilder<AuditoriaEntity> builder)
    {
        builder.ToTable("Auditorias");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Tabla)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("Nombre de la tabla afectada");

        builder.Property(e => e.EntidadId)
            .HasComment("PK de la entidad modificada");

        builder.Property(e => e.Operacion)
            .IsRequired()
            .HasMaxLength(50)
            .HasComment("CREATE, UPDATE, DELETE");

        builder.Property(e => e.FechaHora)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.ValoresAnterior)
            .HasColumnType("json")
            .HasComment("Estado anterior en JSON");

        builder.Property(e => e.ValoresNuevo)
            .HasColumnType("json")
            .HasComment("Estado nuevo en JSON");

        // Relationships
        builder.HasOne(e => e.Usuario)
            .WithMany(u => u.Auditoria)
            .HasForeignKey(e => e.UsuarioId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Auditorias_Usuario");
    }
}
