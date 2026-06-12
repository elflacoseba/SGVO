# Tasks: Skills ABM Full CRUD

## Review Workload Forecast

| Field | Value |
|-------|-------|
| Estimated changed lines | ~950 (16 new + 4 modified, tests included) |
| 400-line budget risk | High |
| Chained PRs recommended | Yes |
| Suggested split | PR 1 (Domain + Application) → PR 2 (Infrastructure + API) → PR 3 (Tests) |
| Delivery strategy | ask-always |
| Chain strategy | pending |

Decision needed before apply: Yes
Chained PRs recommended: Yes
Chain strategy: pending
400-line budget risk: High

### Suggested Work Units

| Unit | Goal | Likely PR | Notes |
|------|------|-----------|-------|
| 1 | Domain fixes + Application layer | PR 1 | ~330 lines; base: main |
| 2 | Handlers + Controller + DI | PR 2 | ~340 lines; depends on PR 1 |
| 3 | Unit tests | PR 3 | ~280 lines; depends on PR 2 |

## Phase 1: Foundation

- [x] 1.1 Modify `src/SGVO.Domain/Entities/Skill.cs`: add `: IEntity`, add `DateTime? ModificadoEn`, update mutation methods to set it
- [x] 1.2 Modify `src/SGVO.Shared/DomainConstants.cs`: add `SkillNombreMaxLength=150`, `SkillCategoriaMaxLength=100`, `SkillDescripcionMaxLength=500`
- [x] 1.3 Create `src/SGVO.Application/Features/Skills/Commands/CrearSkillCommand.cs` with 4 command records (Crear, Actualizar, Eliminar, Reactivar)
- [x] 1.4 Create `src/SGVO.Application/Features/Skills/Dtos/CreateSkillRequest.cs` and `UpdateSkillRequest.cs`
- [x] 1.5 Create `src/SGVO.Application/Features/Skills/Queries/GetSkillByIdQuery.cs` and `SkillDetailDto.cs`

## Phase 2: Core Implementation

- [x] 2.1 Create `CrearSkillValidator.cs` — Nombre required+max 150, Categoria max 100, Descripcion max 500
- [x] 2.2 Create `ActualizarSkillValidator.cs`, `EliminarSkillValidator.cs`, `ReactivarSkillValidator.cs`
- [x] 2.3 Create `CrearSkillCommandHandler.cs` — new entity, duplicate name check, return DTO
- [x] 2.4 Create `ActualizarSkillCommandHandler.cs` — load, check not deleted, call `Actualizar()`, save
- [x] 2.5 Create `EliminarSkillCommandHandler.cs` — guard `CargoSkills`+`PersonaSkills` refs, call `Eliminar()`
- [x] 2.6 Create `ReactivarSkillCommandHandler.cs` — load, idempotent if active, call `Reactivar()`
- [x] 2.7 Create `GetSkillByIdQueryHandler.cs` — `AsNoTracking`, exclude soft-deleted, map to DTO
- [x] 2.8 Modify `ServiceCollectionExtensions.cs` — register 5 new handlers
- [x] 2.9 Rewrite `SkillsController.cs` from `BaseReadOnlyController` to full `ControllerBase` with 5 endpoints

## Phase 3: Testing

- [x] 3.1 Create `CrearSkillCommandHandlerTests.cs` — valid data, duplicate name conflict
- [x] 3.2 Create `CrearSkillValidatorTests.cs` — valid, empty name, name>150, desc>500, null optional
- [x] 3.3 Create `EliminarSkillCommandHandlerTests.cs` — valid, not found, already deleted, blocked by CargoSkills, blocked by PersonaSkills
- [x] 3.4 Create `ActualizarSkillCommandHandlerTests.cs` — valid, not found, already deleted
- [x] 3.5 Create `ReactivarSkillCommandHandlerTests.cs` — valid, not found, idempotent already active
- [x] 3.6 Create `GetSkillByIdQueryHandlerTests.cs` — found, not found, soft-deleted returns null

## Phase 4: Cleanup

- [x] 4.1 Run `dotnet build` (zero warnings) and `dotnet test` (all pass)
- [x] 4.2 Verify no regression on existing `GET /api/v1/skills`
