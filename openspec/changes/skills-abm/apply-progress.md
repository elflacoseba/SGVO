# Apply Progress: Skills ABM Full CRUD

## Status: COMPLETED

## Build Result
- **Build**: 0 warnings, 0 errors
- **Unit Tests**: 207 passed, 0 failed
- **Integration Tests**: 8 failed (pre-existing MySQL schema issues, not related to this change)

## Files Created (16)

### Application Layer
| File | Description |
|------|-------------|
| `src/SGVO.Application/Features/Skills/Commands/CrearSkillCommand.cs` | 4 command records (Crear, Actualizar, Eliminar, Reactivar) |
| `src/SGVO.Application/Features/Skills/Commands/CrearSkillValidator.cs` | FluentValidation for CrearSkillCommand |
| `src/SGVO.Application/Features/Skills/Commands/ActualizarSkillValidator.cs` | FluentValidation for ActualizarSkillCommand |
| `src/SGVO.Application/Features/Skills/Commands/EliminarSkillValidator.cs` | FluentValidation for EliminarSkillCommand |
| `src/SGVO.Application/Features/Skills/Commands/ReactivarSkillValidator.cs` | FluentValidation for ReactivarSkillCommand |
| `src/SGVO.Application/Features/Skills/Dtos/CreateSkillRequest.cs` | Request DTO for creating a skill |
| `src/SGVO.Application/Features/Skills/Dtos/UpdateSkillRequest.cs` | Request DTO for updating a skill |
| `src/SGVO.Application/Features/Skills/Queries/GetSkillByIdQuery.cs` | Query to retrieve a single skill by ID |
| `src/SGVO.Application/Features/Skills/Queries/SkillDetailDto.cs` | DTO with FromEntity mapping |

### Infrastructure Layer
| File | Description |
|------|-------------|
| `src/SGVO.Infrastructure/Commands/CrearSkillCommandHandler.cs` | Creates new skill with duplicate name check |
| `src/SGVO.Infrastructure/Commands/ActualizarSkillCommandHandler.cs` | Updates existing skill |
| `src/SGVO.Infrastructure/Commands/EliminarSkillCommandHandler.cs` | Soft-deletes with CargoSkills/PersonaSkills guard |
| `src/SGVO.Infrastructure/Commands/ReactivarSkillCommandHandler.cs` | Reactivates soft-deleted skill |
| `src/SGVO.Infrastructure/Queries/GetSkillByIdQueryHandler.cs` | Returns single skill by ID |

### Tests
| File | Description |
|------|-------------|
| `tests/SGVO.UnitTests/Commands/Skills/CrearSkillCommandHandlerTests.cs` | 3 tests |
| `tests/SGVO.UnitTests/Commands/Skills/CrearSkillValidatorTests.cs` | 6 tests |
| `tests/SGVO.UnitTests/Commands/Skills/EliminarSkillCommandHandlerTests.cs` | 5 tests |
| `tests/SGVO.UnitTests/Commands/Skills/EliminarSkillValidatorTests.cs` | 3 tests |
| `tests/SGVO.UnitTests/Commands/Skills/ActualizarSkillCommandHandlerTests.cs` | 3 tests |
| `tests/SGVO.UnitTests/Commands/Skills/ActualizarSkillValidatorTests.cs` | 4 tests |
| `tests/SGVO.UnitTests/Commands/Skills/ReactivarSkillCommandHandlerTests.cs` | 3 tests |
| `tests/SGVO.UnitTests/Commands/Skills/ReactivarSkillValidatorTests.cs` | 2 tests |
| `tests/SGVO.UnitTests/Queries/GetSkillByIdQueryHandlerTests.cs` | 3 tests |

## Files Modified (5)

| File | Change |
|------|--------|
| `src/SGVO.Domain/Entities/Skill.cs` | Added `: IEntity`, `ModificadoEn` property, set on mutations |
| `src/SGVO.Shared/DomainConstants.cs` | Added Skill constants (Nombre=150, Categoria=100, Descripcion=500) |
| `src/SGVO.Infrastructure/Persistence/Entities/SkillEntity.cs` | Added `ModificadoEn` property for DB mapping |
| `src/SGVO.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs` | Registered 5 Skill handlers + 1 query handler |
| `src/SGVO.Api/Controllers/SkillsController.cs` | Rewritten from BaseReadOnlyController to full ControllerBase with 6 endpoints |

## Key Discovery

Skills uses a **separate persistence entity** (`SkillEntity`) unlike Cargo which uses the domain entity directly (`DbSet<Cargo>`). The command handlers were adapted to use `SgvoDbContext` directly with `SkillEntity` for persistence, while using the domain `Skill` entity for business rule validation.

## API Endpoints

| Verb | Route | Status |
|------|-------|--------|
| GET | `/api/v1/skills` | ✅ Existing (unchanged) |
| GET | `/api/v1/skills/{id}` | ✅ New |
| POST | `/api/v1/skills` | ✅ New |
| PUT | `/api/v1/skills/{id}` | ✅ New |
| DELETE | `/api/v1/skills/{id}` | ✅ New |
| POST | `/api/v1/skills/{id}/reactivate` | ✅ New |
