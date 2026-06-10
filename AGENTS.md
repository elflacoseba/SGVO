# AGENTS.md — SGVO

Sistema de Gestión de Vacantes Organizacionales. Greenfield .NET 10 project. Currently no solution, no projects, no CI.

## Tech Stack (Decided)
- .NET 10
- Entity Framework Core 9 + Pomelo.EntityFrameworkCore.MySql
- MySQL 8.0+
- Clean Architecture (Domain / Application / Infrastructure / API)
- xUnit for testing

## State of the Repo
- **No .NET code yet.** The database is already created and populated in local MySQL.
- SQL migration scripts live in `sql/` and have been executed against the local `sgvo` database.
- No `.csproj`, no `.sln`, no CI workflows, no test projects.
- Any agent implementing the .NET layer should scaffold the Clean Architecture solution from scratch.

## Database (Already Deployed)
- **Host:** localhost (socket `/tmp/mysql.sock`)
- **User:** root (no password, local dev only)
- **Database:** `sgvo`
- **Charset:** `utf8mb4` / `utf8mb4_unicode_ci`
- **Migrations executed:** `sql/01` through `sql/06`

## Project-Specific Skills
Custom skills are installed under `.agents/skills/` and tracked in `skills-lock.json`:
- `database-designer` — schema design, migrations, indexing
- `dotnet-backend-patterns` — EF Core, Dapper, DI, repository/service patterns
- `dotnet-best-practices` — code quality checks
- `github-issues` — issue management
- `mysql` — MySQL-specific operations and tuning
- `pr-review-dotnet` — PR review for .NET projects

The skill registry at `.atl/skill-registry.md` indexes both project-local and user-global skills. When delegating to subagents, pass the relevant `SKILL.md` paths from the registry.

## Conventions Already Established
- **Table names:** PascalCase, plural, Spanish singular (`Personas`, `Ocupaciones`, `UnidadesOrganizativas`)
- **Column names:** PascalCase, no prefixes (`Nombre`, `FechaInicio`)
- **PKs:** `Id` (`BIGINT UNSIGNED` → `long` in C#)
- **FKs:** `[Entity]Id` (`PersonaId`, `PuestoId`)
- **Timestamps:** `CreadoEn`, `ModificadoEn`
- **Soft delete:** `Activo` (boolean)
- **Enums in C#:** mapped to `VARCHAR` in MySQL (no magic numbers)

## Known Decisions
- **Auditoría:** single table (`Auditorias`) with JSON columns for old/new values. Implement via `SaveChangesInterceptor`.
- **Skill match:** `PuntajeMatch` stored in `Postulaciones` and recalculated at evaluation time.
- **Postulantes:** `PersonaId` is nullable to support external candidates.
- **Hierarchy:** self-referencing FKs on `UnidadesOrganizativas` (PadreId) and `Puestos` (SuperiorId).
- **Auth:** `Usuarios` table with SHA-256 hashed passwords (dev only, upgrade to BCrypt/Argon2 in prod). M:N with `Roles` via `UsuarioRoles`. `PersonaId` nullable — admin users may not be employees.
- **Auditorías → Usuarios:** `Auditorias.UsuarioId` has FK to `Usuarios(Id)`.

## What Does Not Exist Yet (Do Not Assume)
- `opencode.json`
- `README.md`
- `.editorconfig`
- CI / GitHub Actions
- Dockerfile / compose
- Any test runner or test config

## What to Do First
When starting implementation, create:
1. `SGVO.sln` with Clean Architecture projects
2. `SGVO.Domain` — entities and interfaces
3. `SGVO.Infrastructure` — EF Core DbContext, configurations, migrations
4. `SGVO.API` — minimal API or controllers
5. `SGVO.Tests` — xUnit project
6. `opencode.json` if needed for workspace config
7. `README.md` with setup instructions
