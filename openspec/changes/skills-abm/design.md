# Design: Skills ABM Full CRUD

## Technical Approach

Mirror the proven **Cargos ABM pattern** exactly. The `Skill` domain entity already has `Actualizar()`, `Eliminar()`, and `Reactivar()` methods — the only missing pieces are the Application/Infrastructure/Api wiring and two small domain fixes (`IEntity` + `ModificadoEn`).

All new files follow the established structure: command records in Application, handlers in Infrastructure, validators registered via `AddValidatorsFromAssembly`, and controller endpoints using `ValidationBehavior.HandleAsync` for the validator→handler pipeline.

## Architecture Decisions

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Pattern | Mirror Cargos exactly | Established convention, zero abstraction risk, consistent codebase |
| `IEntity` on `Skill` | Add it | Required for `IRepository<Skill>` generic registration; no migration needed |
| `ModificadoEn` | Add to domain entity | DB schema already has it (`06-soft-delete-fields.sql`), `Cargo` sets it on every mutation; `Skill` should too |
| Deletion guard | Check `CargoSkills` + `PersonaSkills` | Same pattern as `EliminarCargoCommandHandler` (checks `Puestos` + `CargoSkills`), adapted for Skill's M:N relationships |
| Query return entity | Use `SkillEntity` directly | Existing `GetAllSkillsQueryHandler` reads persistence entities; keeping consistency |

## Data Flow

```
Create / Update / Delete / Reactivate
  │
  ▼
Controller ──→ ValidationBehavior ──→ Command Handler ──→ Domain Entity
  │                (FluentValidation)         (IRepository<T>)    (business rules)
  │                                                          │
  │                                                          ▼
  │                                                    DbContext.SaveChanges
  │                                                          │
  ▼                                                          ▼
HTTP Response ◀── Result<T> ◀── DTO mapping ◀── Persisted entity

GetById / GetAll
  │
  ▼
Controller ──→ Query Handler ──→ SgvoDbContext (AsNoTracking) ──→ DTO projection ──→ Result<T>
```

**Delete-specific flow**: Handler checks `cargoSkills.Any(cs => cs.SkillId == id && cs.EliminadoEn == null)` and `personaSkills.Any(ps => ps.SkillId == id && ps.EliminadoEn == null)` before calling `skill.Eliminar(eliminadoPor)`.

## File Changes

| File | Action | Description |
|------|--------|-------------|
| `src/SGVO.Domain/Entities/Skill.cs` | Modify | Add `: IEntity`, add `DateTime? ModificadoEn` property, update `Actualizar`/`Eliminar`/`Reactivar` to set it |
| `src/SGVO.Shared/DomainConstants.cs` | Modify | Add `SkillNombreMaxLength=150`, `SkillCategoriaMaxLength=100`, `SkillDescripcionMaxLength=500` |
| `src/SGVO.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs` | Modify | Register 5 new handlers (`GetSkillById`, `Crear`, `Actualizar`, `Eliminar`, `Reactivar`) |
| `src/SGVO.Api/Controllers/SkillsController.cs` | Modify | Rewrite from `BaseReadOnlyController` to full `ControllerBase` with 5 endpoints |
| `src/SGVO.Application/Features/Skills/Commands/CrearSkillCommand.cs` | Create | 4 command records: `Crear`, `Actualizar`, `Eliminar`, `Reactivar` |
| `src/SGVO.Application/Features/Skills/Commands/CrearSkillValidator.cs` | Create | FluentValidation for `CrearSkillCommand` |
| `src/SGVO.Application/Features/Skills/Commands/ActualizarSkillValidator.cs` | Create | FluentValidation for `ActualizarSkillCommand` |
| `src/SGVO.Application/Features/Skills/Commands/EliminarSkillValidator.cs` | Create | FluentValidation for `EliminarSkillCommand` |
| `src/SGVO.Application/Features/Skills/Commands/ReactivarSkillValidator.cs` | Create | FluentValidation for `ReactivarSkillCommand` |
| `src/SGVO.Application/Features/Skills/Dtos/CreateSkillRequest.cs` | Create | `record CreateSkillRequest(string Nombre, string? Categoria, string? Descripcion)` |
| `src/SGVO.Application/Features/Skills/Dtos/UpdateSkillRequest.cs` | Create | `record UpdateSkillRequest(string Nombre, string? Categoria, string? Descripcion)` |
| `src/SGVO.Application/Features/Skills/Queries/GetSkillByIdQuery.cs` | Create | `record GetSkillByIdQuery(long Id) : IQuery<SkillDetailDto?>` |
| `src/SGVO.Application/Features/Skills/Queries/SkillDetailDto.cs` | Create | DTO with `Id`, `Nombre`, `Categoria`, `Descripcion`, `Activo`, `CreadoEn`, `ModificadoEn` |
| `src/SGVO.Infrastructure/Commands/CrearSkillCommandHandler.cs` | Create | `ICommandHandler<CrearSkillCommand, SkillDetailDto>` — new domain entity, save, return DTO |
| `src/SGVO.Infrastructure/Commands/ActualizarSkillCommandHandler.cs` | Create | Load, validate not deleted, call `Actualizar()`, save |
| `src/SGVO.Infrastructure/Commands/EliminarSkillCommandHandler.cs` | Create | Load, validate not deleted, guard M:N refs, call `Eliminar()`, save |
| `src/SGVO.Infrastructure/Commands/ReactivarSkillCommandHandler.cs` | Create | Load, idempotent if already active, call `Reactivar()`, save |
| `src/SGVO.Infrastructure/Queries/GetSkillByIdQueryHandler.cs` | Create | `AsNoTracking` query, exclude `EliminadoEn != null`, map to `SkillDetailDto` |
| `tests/SGVO.UnitTests/Commands/Skills/CrearSkillCommandHandlerTests.cs` | Create | 2+ tests: valid data, null optional fields |
| `tests/SGVO.UnitTests/Commands/Skills/CrearSkillValidatorTests.cs` | Create | 5 tests: valid, empty name, name too long, desc too long, null desc |
| `tests/SGVO.UnitTests/Commands/Skills/EliminarSkillCommandHandlerTests.cs` | Create | 5 tests: valid, not found, already deleted, blocked by CargoSkills, blocked by PersonaSkills |
| `tests/SGVO.UnitTests/Commands/Skills/ActualizarSkillCommandHandlerTests.cs` | Create | 3+ tests: valid, not found, already deleted |
| `tests/SGVO.UnitTests/Commands/Skills/ReactivarSkillCommandHandlerTests.cs` | Create | 3+ tests: valid, not found, idempotent already active |
| `tests/SGVO.UnitTests/Queries/GetSkillByIdQueryHandlerTests.cs` | Create | 2+ tests: found, not found (soft-deleted) |

## Testing Strategy

| Layer | What | Approach |
|-------|------|----------|
| Unit — Handlers | Each command handler logic | `InMemoryDbContextFactory` + `TestRepository<Skill>` (no mocks) |
| Unit — Validators | FluentValidation rules | `TestValidate` from `FluentValidation.TestHelper` |
| Unit — Queries | `GetSkillById` | Same in-memory DB pattern |
| Integration | Controller routing + DI | Existing integration test suite (if present) — out of scope for this change |

## Migration / Rollout

No database migration required. `ModificadoEn` already exists in the schema (`06-soft-delete-fields.sql`). Adding the property to the domain entity is purely a mapping alignment.

**Rollback plan**: Revert the 4 modified files and delete the 16 new files. The existing `GET /api/v1/skills` endpoint will continue working because `GetAllSkillsQueryHandler` is untouched.

## Open Questions

- None — all patterns are established and the exploration is complete.
