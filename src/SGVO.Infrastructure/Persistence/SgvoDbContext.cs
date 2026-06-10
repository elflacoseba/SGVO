using Microsoft.EntityFrameworkCore;
using SGVO.Infrastructure.Persistence.Entities;

namespace SGVO.Infrastructure.Persistence;

/// <summary>
/// DbContext para SGVO. Utiliza Fluent API mediante configuraciones en la carpeta Configurations.
/// </summary>
public class SgvoDbContext : DbContext
{
    public SgvoDbContext(DbContextOptions<SgvoDbContext> options)
        : base(options)
    {
    }

    // DbSets para todas las entidades
    public DbSet<AuditoriaEntity> Auditorias { get; set; }
    public DbSet<CargoEntity> Cargos { get; set; }
    public DbSet<CargoSkillEntity> CargoSkills { get; set; }
    public DbSet<OcupacioneEntity> Ocupaciones { get; set; }
    public DbSet<PersonaEntity> Personas { get; set; }
    public DbSet<PersonaSkillEntity> PersonaSkills { get; set; }
    public DbSet<PostulacioneEntity> Postulaciones { get; set; }
    public DbSet<PostulanteEntity> Postulantes { get; set; }
    public DbSet<PuestoEntity> Puestos { get; set; }
    public DbSet<RoleEntity> Roles { get; set; }
    public DbSet<SkillEntity> Skills { get; set; }
    public DbSet<UnidadesOrganizativaEntity> UnidadesOrganizativas { get; set; }
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
    public DbSet<UsuarioEntity> Usuarios { get; set; }
    public DbSet<UsuarioRoleEntity> UsuarioRoles { get; set; }
    public DbSet<VacanteEntity> Vacantes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurar charset y collation globales
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        // Aplicar todas las configuraciones Fluent API desde el assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SgvoDbContext).Assembly);
    }
}
