using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SGVO.Infrastructure.Persistence.Entities;

namespace SGVO.Infrastructure.Persistence;

public partial class SgvoDbContext : DbContext
{
    public SgvoDbContext(DbContextOptions<SgvoDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Auditoria> Auditorias { get; set; }

    public virtual DbSet<Cargo> Cargos { get; set; }

    public virtual DbSet<CargoSkill> CargoSkills { get; set; }

    public virtual DbSet<Ocupacione> Ocupaciones { get; set; }

    public virtual DbSet<Persona> Personas { get; set; }

    public virtual DbSet<PersonaSkill> PersonaSkills { get; set; }

    public virtual DbSet<Postulacione> Postulaciones { get; set; }

    public virtual DbSet<Postulante> Postulantes { get; set; }

    public virtual DbSet<Puesto> Puestos { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Skill> Skills { get; set; }

    public virtual DbSet<UnidadesOrganizativa> UnidadesOrganizativas { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<UsuarioRole> UsuarioRoles { get; set; }

    public virtual DbSet<Vacante> Vacantes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Auditoria>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.EntidadId).HasComment("PK de la entidad modificada");
            entity.Property(e => e.FechaHora).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Operacion).HasComment("CREATE, UPDATE, DELETE");
            entity.Property(e => e.Tabla).HasComment("Nombre de la tabla afectada");
            entity.Property(e => e.ValoresAnterior).HasComment("Estado anterior en JSON");
            entity.Property(e => e.ValoresNuevo).HasComment("Estado nuevo en JSON");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Auditoria).HasConstraintName("FK_Auditorias_Usuario");
        });

        modelBuilder.Entity<Cargo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.CreadoEn).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.ModificadoEn).ValueGeneratedOnAddOrUpdate();

            entity.HasOne(d => d.EliminadoPorNavigation).WithMany(p => p.Cargos).HasConstraintName("FK_Cargos_EliminadoPor");
        });

        modelBuilder.Entity<CargoSkill>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.NivelImportancia).HasComment("1=Crítico, 2=Importante, 3=Deseable, 4=Secundario");

            entity.HasOne(d => d.Cargo).WithMany(p => p.CargoSkills)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CargoSkills_Cargo");

            entity.HasOne(d => d.EliminadoPorNavigation).WithMany(p => p.CargoSkills).HasConstraintName("FK_CargoSkills_EliminadoPor");

            entity.HasOne(d => d.Skill).WithMany(p => p.CargoSkills)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CargoSkills_Skill");
        });

        modelBuilder.Entity<Ocupacione>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.CreadoEn).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.FechaFin).HasComment("NULL = vigente");
            entity.Property(e => e.TipoOcupacion).HasComment("Permanente, Interina, Suplente");

            entity.HasOne(d => d.EliminadoPorNavigation).WithMany(p => p.Ocupaciones).HasConstraintName("FK_Ocupaciones_EliminadoPor");

            entity.HasOne(d => d.Persona).WithMany(p => p.Ocupaciones)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ocupaciones_Persona");

            entity.HasOne(d => d.Puesto).WithMany(p => p.Ocupaciones)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ocupaciones_Puesto");
        });

        modelBuilder.Entity<Persona>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.CreadoEn).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.ModificadoEn).ValueGeneratedOnAddOrUpdate();

            entity.HasOne(d => d.EliminadoPorNavigation).WithMany(p => p.Personas).HasConstraintName("FK_Personas_EliminadoPor");
        });

        modelBuilder.Entity<PersonaSkill>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.NivelDominio).HasComment("1=Básico, 2=Intermedio, 3=Avanzado, 4=Experto");

            entity.HasOne(d => d.EliminadoPorNavigation).WithMany(p => p.PersonaSkills).HasConstraintName("FK_PersonaSkills_EliminadoPor");

            entity.HasOne(d => d.Persona).WithMany(p => p.PersonaSkills)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PersonaSkills_Persona");

            entity.HasOne(d => d.Skill).WithMany(p => p.PersonaSkills)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PersonaSkills_Skill");
        });

        modelBuilder.Entity<Postulacione>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.CreadoEn).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Estado)
                .HasDefaultValueSql("'Postulado'")
                .HasComment("Postulado, Preseleccionado, Entrevistado, Aprobado, Rechazado, Contratado");
            entity.Property(e => e.FechaPostulacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.ModificadoEn).ValueGeneratedOnAddOrUpdate();
            entity.Property(e => e.PuntajeMatch).HasComment("0.00 - 100.00");

            entity.HasOne(d => d.EliminadoPorNavigation).WithMany(p => p.Postulaciones).HasConstraintName("FK_Postulaciones_EliminadoPor");

            entity.HasOne(d => d.Evaluador).WithMany(p => p.Postulaciones).HasConstraintName("FK_Postulaciones_Evaluador");

            entity.HasOne(d => d.Postulante).WithMany(p => p.Postulaciones)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Postulaciones_Postulante");

            entity.HasOne(d => d.Vacante).WithMany(p => p.Postulaciones)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Postulaciones_Vacante");
        });

        modelBuilder.Entity<Postulante>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.CreadoEn).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Origen)
                .HasDefaultValueSql("'Externo'")
                .HasComment("Interno, Externo, Recomendado");
            entity.Property(e => e.PersonaId).HasComment("NULL si es externo");

            entity.HasOne(d => d.EliminadoPorNavigation).WithMany(p => p.Postulantes).HasConstraintName("FK_Postulantes_EliminadoPor");

            entity.HasOne(d => d.Persona).WithMany(p => p.Postulantes).HasConstraintName("FK_Postulantes_Persona");
        });

        modelBuilder.Entity<Puesto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.CreadoEn).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.ModificadoEn).ValueGeneratedOnAddOrUpdate();

            entity.HasOne(d => d.Cargo).WithMany(p => p.Puestos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Puestos_Cargo");

            entity.HasOne(d => d.EliminadoPorNavigation).WithMany(p => p.Puestos).HasConstraintName("FK_Puestos_EliminadoPor");

            entity.HasOne(d => d.Superior).WithMany(p => p.InverseSuperior).HasConstraintName("FK_Puestos_Superior");

            entity.HasOne(d => d.UnidadOrganizativa).WithMany(p => p.Puestos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Puestos_Unidad");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.CreadoEn).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.EliminadoPorNavigation).WithMany(p => p.Roles).HasConstraintName("FK_Roles_EliminadoPor");
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.Categoria).HasComment("Técnica, Blanda, Gerencial, etc.");
            entity.Property(e => e.CreadoEn).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.EliminadoPorNavigation).WithMany(p => p.Skills).HasConstraintName("FK_Skills_EliminadoPor");
        });

        modelBuilder.Entity<UnidadesOrganizativa>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.CreadoEn).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.ModificadoEn).ValueGeneratedOnAddOrUpdate();
            entity.Property(e => e.NivelJerarquico).HasDefaultValueSql("'1'");
            entity.Property(e => e.Tipo).HasComment("Facultad, Secretaría, Dirección, Departamento, División, Área");

            entity.HasOne(d => d.EliminadoPorNavigation).WithMany(p => p.UnidadesOrganizativas).HasConstraintName("FK_UnidadesOrganizativas_EliminadoPor");

            entity.HasOne(d => d.Padre).WithMany(p => p.InversePadre).HasConstraintName("FK_UnidadesOrganizativas_Padre");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.CreadoEn).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.ModificadoEn).ValueGeneratedOnAddOrUpdate();
            entity.Property(e => e.PersonaId).HasComment("Vincula con Personas si el usuario es empleado");

            entity.HasOne(d => d.EliminadoPorNavigation).WithMany(p => p.InverseEliminadoPorNavigation).HasConstraintName("FK_Usuarios_EliminadoPor");

            entity.HasOne(d => d.Persona).WithMany(p => p.Usuarios).HasConstraintName("FK_Usuarios_Persona");
        });

        modelBuilder.Entity<UsuarioRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Activo).HasDefaultValueSql("'1'");

            entity.HasOne(d => d.EliminadoPorNavigation).WithMany(p => p.UsuarioRoleEliminadoPorNavigations).HasConstraintName("FK_UsuarioRoles_EliminadoPor");

            entity.HasOne(d => d.Rol).WithMany(p => p.UsuarioRoles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UsuarioRoles_Rol");

            entity.HasOne(d => d.Usuario).WithMany(p => p.UsuarioRoleUsuarios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UsuarioRoles_Usuario");
        });

        modelBuilder.Entity<Vacante>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.CreadoEn).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Estado)
                .HasDefaultValueSql("'Abierta'")
                .HasComment("Abierta, EnSeleccion, Cubierta, Cancelada");
            entity.Property(e => e.FechaApertura).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.ModificadoEn).ValueGeneratedOnAddOrUpdate();

            entity.HasOne(d => d.EliminadoPorNavigation).WithMany(p => p.Vacantes).HasConstraintName("FK_Vacantes_EliminadoPor");

            entity.HasOne(d => d.Puesto).WithMany(p => p.Vacantes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vacantes_Puesto");

            entity.HasOne(d => d.Responsable).WithMany(p => p.Vacantes).HasConstraintName("FK_Vacantes_Responsable");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
