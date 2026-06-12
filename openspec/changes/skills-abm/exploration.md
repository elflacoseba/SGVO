# Exploration: Skills ABM (Alta/Baja/Modificación)

## Current State

The Skills module exists in a **partially implemented** state — read-only query endpoint with domain entity that already contains full write logic, but no Application/Infrastructure/Controller wiring for write operations.

### What EXISTS today

| Layer | File | Status |
|-------|------|--------|
| Domain Entity | `src/SGVO.Domain/Entities/Skill.cs` | ✅ Complete — has `Actualizar()`, `Eliminar()`, `Reactivar()` methods with validation |
| Persistence Entity | `src/SGVO.Infrastructure/Persistence/Entities/SkillEntity.cs` | ✅ Complete — includes M:N nav props (`CargoSkills`, `PersonaSkills`) |
| EF Configuration | `src/SGVO.Infrastructure/Persistence/Configurations/SkillConfiguration.cs` | ✅ Complete — table mapping, relationships, max lengths |
| Query (GetAll) | `src/SGVO.Application/Features/Skills/Queries/GetAllSkillsQuery.cs` | ✅ Exists |
| Query Handler | `src/SGVO.Infrastructure/Queries/GetAllSkillsQueryHandler.cs` | ✅ Exists — paginated, excludes soft-deleted |
| DTO (list) | `src/SGVO.Application/Features/Skills/Queries/SkillDto.cs` | ✅ Exists — `Id`, `Nombre`, `Categoria` |
| Controller | `src/SGVO.Api/Controllers/SkillsController.cs` | ⚠️ Read-only — extends `BaseReadOnlyController`, only `GET /api/v1/skills` |
| DI Registration | `src/SGVO.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs` | ⚠️ Only `GetAllSkillsQueryHandler` registered |
| DB Schema | `sql/02-create-tables.sql` | ⚠️ Missing `ModificadoEn` column (exists in entity via soft-delete migration) |
| Unit Tests | `tests/SGVO.UnitTests/Queries/GetAllSkillsQueryHandlerTests.cs` | ✅ 2 tests for GetAll |

### What is MISSING for full ABM

| Layer | What's Missing | Reference Pattern (from Cargos) |
|-------|----------------|--------------------------------|
| Domain | `Skill` does NOT implement `IEntity` interface | `Cargo : IEntity` |
| Application | No `Commands/` folder under `Features/Skills/` | `Features/Cargos/Commands/` has 5 files |
| Application | No `Dtos/` folder (CreateSkillRequest, UpdateSkillRequest) | `Features/Cargos/Dtos/` has 2 files |
| Application | No `GetSkillByIdQuery` or `SkillDetailDto` | `GetCargoByIdQuery` + `CargoDetailDto` |
| Application | No validators (Crear, Actualizar, Eliminar, Reactivar) | 4 validator files in Cargos |
| Infrastructure | No command handlers (Crear, Actualizar, Eliminar, Reactivar) | 4 handler files in `Commands/` |
| Infrastructure | No `GetSkillByIdQueryHandler` | `GetCargoByIdQueryHandler` |
| Infrastructure | DI missing 5 handler registrations | Cargos has 6 registrations |
| Controller | No GetById, Create, Update, Delete, Reactivate endpoints | `CargosController` has 5 endpoints |
| Shared | No Skill constants in `DomainConstants.cs` | `CargoNombreMaxLength`, `CargoDescripcionMaxLength` |
| Tests | No command handler tests, no validator tests | 8 test files in `Commands/Cargos/` |

## Affected Areas

- `src/SGVO.Domain/Entities/Skill.cs` — Add `: IEntity` interface
- `src/SGVO.Shared/DomainConstants.cs` — Add Skill max length constants
- `src/SGVO.Application/Features/Skills/` — Create `Commands/` and `Dtos/` subdirectories with all files
- `src/SGVO.Application/Features/Skills/Queries/` — Add `GetSkillByIdQuery.cs`, `SkillDetailDto.cs`
- `src/SGVO.Infrastructure/Commands/` — Add 4 command handler files
- `src/SGVO.Infrastructure/Queries/` — Add `GetSkillByIdQueryHandler.cs`
- `src/SGVO.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs` — Register 5 new handlers
- `src/SGVO.Api/Controllers/SkillsController.cs` — Rewrite from `BaseReadOnlyController` to full CRUD `ControllerBase`
- `tests/SGVO.UnitTests/Commands/Skills/` — Create directory with 8 test files
- `tests/SGVO.UnitTests/Queries/` — Add `GetSkillByIdQueryHandlerTests.cs`

## Approaches

### 1. Mirror Cargos Pattern (Recommended)

Copy the exact Cargos ABM structure, adapting field names for Skills (Nombre, Categoria, Descripcion).

- **Pros**: Proven pattern, consistent codebase, low risk, fast implementation
- **Cons**: None significant — this IS the project convention
- **Effort**: Low — ~15 files, all following established templates

### 2. Generic/Abstract Base ABM

Create a generic base controller/service that handles CRUD for any entity, then configure per-entity.

- **Pros**: Less code duplication for future modules
- **Cons**: Over-engineering for 5-6 entities, breaks existing convention, adds abstraction complexity
- **Effort**: Medium — requires refactoring existing modules for consistency

### 3. Minimal ABM (No Reactivate, No GetById)

Only implement Create + Update + Delete, skip GetById and Reactivate.

- **Pros**: Fewer files
- **Cons**: Inconsistent with Cargos/TiposUnidadOrganizativa which have full 5-endpoint ABM, missing business need (reactivation is a real requirement)
- **Effort**: Low — but incomplete

## Key Implementation Details

### Skill Domain Entity Gap

`Skill.cs` does NOT implement `IEntity` (unlike `Cargo.cs`). This must be fixed because `IRepository<T>` and command handlers depend on it for the generic repository pattern.

```csharp
// Current
public class Skill { ... }

// Required
public class Skill : IEntity { ... }
```

### Deletion Guard: M:N Relationships

The `EliminarSkillCommandHandler` must check for active references in TWO join tables:
- `CargoSkills` — skills required by cargos
- `PersonaSkills` — skills possessed by personas

This mirrors how `EliminarCargoCommandHandler` checks `Puestos` and `CargoSkills`.

### Missing `ModificadoEn` in DB Schema

The SQL `02-create-tables.sql` creates `Skills` without `ModificadoEn`. The entity has it via `06-soft-delete-fields.sql`. The domain entity `Skill.cs` does NOT have `ModificadoEn` property — this is a discrepancy. The `Actualizar()` method should set `ModificadoEn = DateTime.UtcNow` but the property doesn't exist on the domain entity.

**Decision needed**: Add `ModificadoEn` to `Skill.cs` domain entity (matching `Cargo.cs` pattern) or leave it out. Recommendation: ADD it for consistency.

### DomainConstants Gap

`DomainConstants.cs` has constants for `Cargo`, `UnidadOrganizativa`, `TipoUnidadOrganizativa` but NOT for `Skill`. Need to add:
- `SkillNombreMaxLength = 150`
- `SkillCategoriaMaxLength = 100`
- `SkillDescripcionMaxLength = 500`

### Request DTOs

Following Cargos pattern:
- `CreateSkillRequest(string Nombre, string? Categoria, string? Descripcion)`
- `UpdateSkillRequest(string Nombre, string? Categoria, string? Descripcion)`

### SkillDetailDto

Should include all readable fields:
- `Id`, `Nombre`, `Categoria`, `Descripcion`, `Activo`, `CreadoEn`, `ModificadoEn`

## Recommendation

**Approach 1: Mirror Cargos Pattern** — This is the clear winner. The project has an established, working convention. Deviating adds complexity without value at this stage.

The implementation follows this exact file list (16 new files, 3 modified):

### New Files (16)

**Application Layer (7 files):**
1. `src/SGVO.Application/Features/Skills/Commands/CrearSkillCommand.cs` — 4 command records
2. `src/SGVO.Application/Features/Skills/Commands/CrearSkillValidator.cs`
3. `src/SGVO.Application/Features/Skills/Commands/ActualizarSkillValidator.cs`
4. `src/SGVO.Application/Features/Skills/Commands/EliminarSkillValidator.cs`
5. `src/SGVO.Application/Features/Skills/Commands/ReactivarSkillValidator.cs`
6. `src/SGVO.Application/Features/Skills/Dtos/CreateSkillRequest.cs`
7. `src/SGVO.Application/Features/Skills/Dtos/UpdateSkillRequest.cs`

**Queries (2 files):**
8. `src/SGVO.Application/Features/Skills/Queries/GetSkillByIdQuery.cs`
9. `src/SGVO.Application/Features/Skills/Queries/SkillDetailDto.cs`

**Infrastructure Layer (5 files):**
10. `src/SGVO.Infrastructure/Commands/CrearSkillCommandHandler.cs`
11. `src/SGVO.Infrastructure/Commands/ActualizarSkillCommandHandler.cs`
12. `src/SGVO.Infrastructure/Commands/EliminarSkillCommandHandler.cs`
13. `src/SGVO.Infrastructure/Commands/ReactivarSkillCommandHandler.cs`
14. `src/SGVO.Infrastructure/Queries/GetSkillByIdQueryHandler.cs`

**Tests (2+ files):**
15. `tests/SGVO.UnitTests/Commands/Skills/CrearSkillCommandHandlerTests.cs`
16. `tests/SGVO.UnitTests/Commands/Skills/CrearSkillValidatorTests.cs`
+ Additional test files for each handler/validator

### Modified Files (3)

1. `src/SGVO.Domain/Entities/Skill.cs` — Add `: IEntity`, add `ModificadoEn` property
2. `src/SGVO.Shared/DomainConstants.cs` — Add Skill constants
3. `src/SGVO.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs` — Register 5 handlers
4. `src/SGVO.Api/Controllers/SkillsController.cs` — Rewrite to full CRUD

## Risks

- **Low**: `Skill` entity missing `IEntity` — simple fix, no migration needed
- **Low**: `ModificadoEn` missing from domain entity — add property, EF already maps it
- **Medium**: Deletion guard needs to check 2 M:N tables (`CargoSkills`, `PersonaSkills`) — more complex than Cargos but follows same pattern
- **Low**: Existing `GetAllSkillsQueryHandler` uses `SkillEntity` directly instead of domain `Skill` — this is the established pattern (queries use persistence entities, commands use domain entities)

## Ready for Proposal

**Yes** — The exploration is complete. All affected files are identified, the reference pattern is well-understood, and the implementation path is clear. The change is low-risk and follows established conventions exactly.

**Recommended next step**: `sdd-propose` to formalize the scope, then `sdd-spec` for requirements, then `sdd-tasks` to break into implementation units.
