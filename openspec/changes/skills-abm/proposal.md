# Proposal: Implementar módulo Skills con ABM completo

## Intent

Wire up full Create/Read/Update/Delete/Reactivate operations on the existing `Skill` domain entity, mirroring the proven Cargos pattern exactly.

## Current State

- Domain entity `Skill.cs` has write methods (`Actualizar`, `Eliminar`, `Reactivar`) but is **not wired** to any command, handler, or controller endpoint
- Only `GET /api/v1/skills` (paginated list) exists
- `Skill` is missing `IEntity` interface and `ModificadoEn` property (unlike `Cargo`)

## Scope

### New Files (16)

| Layer | Files |
|-------|-------|
| Application Commands | `CrearSkillCommand.cs`, `CrearSkillValidator.cs`, `ActualizarSkillValidator.cs`, `EliminarSkillValidator.cs`, `ReactivarSkillValidator.cs` |
| Application DTOs | `CreateSkillRequest.cs`, `UpdateSkillRequest.cs` |
| Application Queries | `GetSkillByIdQuery.cs`, `SkillDetailDto.cs` |
| Infrastructure Commands | `CrearSkillCommandHandler.cs`, `ActualizarSkillCommandHandler.cs`, `EliminarSkillCommandHandler.cs`, `ReactivarSkillCommandHandler.cs` |
| Infrastructure Queries | `GetSkillByIdQueryHandler.cs` |
| Tests | `CrearSkillCommandHandlerTests.cs`, `CrearSkillValidatorTests.cs` (+ additional handler/validator tests) |

### Modified Files (4)

| File | Change |
|------|--------|
| `Domain/Entities/Skill.cs` | Add `: IEntity`, add `ModificadoEn` property |
| `Shared/DomainConstants.cs` | Add `SkillNombreMaxLength=150`, `SkillCategoriaMaxLength=100`, `SkillDescripcionMaxLength=500` |
| `Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs` | Register 5 new handlers |
| `Api/Controllers/SkillsController.cs` | Rewrite from `BaseReadOnlyController` to full CRUD `ControllerBase` |

## API Endpoints

| Verb | Route | Action |
|------|-------|--------|
| GET | `/api/v1/skills` | List (existing) |
| GET | `/api/v1/skills/{id}` | Get by ID |
| POST | `/api/v1/skills` | Create |
| PUT | `/api/v1/skills/{id}` | Update |
| DELETE | `/api/v1/skills/{id}` | Soft delete |
| POST | `/api/v1/skills/{id}/reactivate` | Reactivate |

## Key Business Rules

- **Deletion guard**: Cannot delete a Skill referenced by active `CargoSkills` or `PersonaSkills` records
- **Validation**: Nombre required (max 150), Categoria optional (max 100), Descripcion optional (max 500)
- **Soft delete**: `Activo=false`, sets `EliminadoEn`/`EliminadoPor`; reactivation clears them

## Approach

**Mirror Cargos pattern** — copy established structure, adapt field names. No new abstractions.

## Risks

- **Low**: `IEntity` addition is a one-line change, no migration needed
- **Low**: `ModificadoEn` already exists in DB schema (`06-soft-delete-fields.sql`), just needs domain property
- **Low**: Deletion guard checks 2 M:N tables (same pattern as Cargos → Puestos/CargoSkills)

## Out of Scope

- Auth/JWT (separate change)
- Audit interceptor (separate change)
- CI/CD pipeline
