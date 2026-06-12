# Tasks: Cargo Module

## Review Workload Forecast

| Field | Value |
|-------|-------|
| Estimated changed lines | ~1200–1500 |
| 400-line budget risk | High |
| Chained PRs recommended | Yes |
| Suggested split | PR 1 (Domain+Infra mapping) → PR 2 (Queries+Commands+API) → PR 3 (Tests) |
| Delivery strategy | ask-on-risk |
| Chain strategy | stacked-to-main |

Decision needed before apply: Yes
Chained PRs recommended: Yes
Chain strategy: stacked-to-main
400-line budget risk: High

### Suggested Work Units

| Unit | Goal | Likely PR | Notes |
|------|------|-----------|-------|
| 1 | Replace CargoEntity → Cargo domain entity; update all navigation refs and EF configs | PR 1 | Base: main; pure infra refactor, no behavior change |
| 2 | Implement queries, commands, validators, handlers, and API controller | PR 2 | Base: PR 1 branch; new application logic |
| 3 | Unit tests + integration tests for all endpoints | PR 3 | Base: PR 2 branch; test coverage |

## Phase 1: Domain + Infrastructure Mapping

- [x] 1.1 Add `IEntity` interface to `Cargo` domain entity in `src/SGVO.Domain/Entities/Cargo.cs`
- [x] 1.2 Add `CargoNombreMaxLength = 150` and `CargoDescripcionMaxLength = 500` to `src/SGVO.Shared/DomainConstants.cs`
- [x] 1.3 Change `SgvoDbContext.Cargos` from `DbSet<CargoEntity>` to `DbSet<Cargo>` in `src/SGVO.Infrastructure/Persistence/SgvoDbContext.cs`
- [x] 1.4 Update `CargoConfiguration` to `IEntityTypeConfiguration<Cargo>` in `src/SGVO.Infrastructure/Persistence/Configurations/CargoConfiguration.cs`; map `Activo` as non-nullable `bool` with `.HasDefaultValue(true)`
- [x] 1.5 Update `CargoSkillEntity.Cargo` navigation from `CargoEntity` to `Cargo` in `src/SGVO.Infrastructure/Persistence/Entities/CargoSkillEntity.cs`
- [x] 1.6 Update `PuestoEntity.Cargo` navigation from `CargoEntity` to `Cargo` in `src/SGVO.Infrastructure/Persistence/Entities/PuestoEntity.cs`
- [x] 1.7 Update `UsuarioEntity.Cargos` navigation from `ICollection<CargoEntity>` to `ICollection<Cargo>` in `src/SGVO.Infrastructure/Persistence/Entities/UsuarioEntity.cs`
- [x] 1.8 Update `CargoSkillConfiguration` relationship `.HasOne(e => e.Cargo)` generic type to `Cargo` in `src/SGVO.Infrastructure/Persistence/Configurations/CargoSkillConfiguration.cs`
- [x] 1.9 Update `PuestoConfiguration` relationship `.HasOne(e => e.Cargo)` generic type to `Cargo` in `src/SGVO.Infrastructure/Persistence/Configurations/PuestoConfiguration.cs`
- [x] 1.10 Delete `src/SGVO.Infrastructure/Persistence/Entities/CargoEntity.cs`

## Phase 2: Queries (GetById)

- [x] 2.1 Create `CargoDetailDto` in `src/SGVO.Application/Features/Cargos/Queries/CargoDetailDto.cs` with properties: Id, Nombre, Descripcion, Activo, CreadoEn, ModificadoEn
- [x] 2.2 Create `GetCargoByIdQuery` record in `src/SGVO.Application/Features/Cargos/Queries/GetCargoByIdQuery.cs` implementing `IQuery<CargoDetailDto?>`
- [x] 2.3 Create `GetCargoByIdQueryHandler` in `src/SGVO.Infrastructure/Queries/GetCargoByIdQueryHandler.cs` using `DbContext.Set<Cargo>().AsNoTracking()` projection; return failure if soft-deleted or not found
- [x] 2.4 Update `GetAllCargosQueryHandler` to use `DbSet<Cargo>` instead of `DbSet<CargoEntity>`; adjust `Activo` access (now `bool` not `bool?`) in `src/SGVO.Infrastructure/Queries/GetAllCargosQueryHandler.cs`
- [x] 2.5 Register `GetCargoByIdQueryHandler` in `src/SGVO.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs`

## Phase 3: Commands (Create, Update, Delete, Reactivate)

- [x] 3.1 Create `CreateCargoRequest` and `UpdateCargoRequest` records in `src/SGVO.Application/Features/Cargos/Dtos/`
- [x] 3.2 Create `CrearCargoCommand`, `ActualizarCargoCommand`, `EliminarCargoCommand`, `ReactivarCargoCommand` records in `src/SGVO.Application/Features/Cargos/Commands/`
- [x] 3.3 Create `CrearCargoValidator` with `NotEmpty` on Nombre, `MaxLength(150)` on Nombre, `MaxLength(500)` on Descripcion
- [x] 3.4 Create `ActualizarCargoValidator` with same rules as Crear + `Id > 0`
- [x] 3.5 Create `EliminarCargoValidator` with `Id > 0` and `EliminadoPor > 0`
- [x] 3.6 Create `ReactivarCargoValidator` with `Id > 0`
- [x] 3.7 Create `CrearCargoCommandHandler` in `src/SGVO.Infrastructure/Commands/` using `IRepository<Cargo>.Add()`; unique-name guard via `DbContext.Cargos.AnyAsync()`; return `CargoDetailDto`
- [x] 3.8 Create `ActualizarCargoCommandHandler` using `IRepository<Cargo>.GetByIdAsync()` + `cargo.Actualizar()` + unique-name guard (exclude self) + `.Update()` + `SaveChangesAsync()`
- [x] 3.9 Create `EliminarCargoCommandHandler` using `IRepository<Cargo>.GetByIdAsync()` + not-found check + already-deleted check + Puestos guard + CargoSkills guard + `cargo.Eliminar()` + `.Update()` + `SaveChangesAsync()`
- [x] 3.10 Create `ReactivarCargoCommandHandler` using `IRepository<Cargo>.GetByIdAsync()` + not-found check + `cargo.Reactivar()` (idempotent if already active) + `.Update()` + `SaveChangesAsync()`
- [x] 3.11 Register all 4 command handlers in `ServiceCollectionExtensions.cs`

## Phase 4: API Controller

- [x] 4.1 Refactor `CargosController` from `BaseReadOnlyController` to `ControllerBase` in `src/SGVO.Api/Controllers/CargosController.cs`
- [x] 4.2 Add `GET /{id}` endpoint returning `CargoDetailDto` (200/404)
- [x] 4.3 Add `POST /` endpoint with `CreateCargoRequest` → `CrearCargoCommand` (201/400/409)
- [x] 4.4 Add `PUT /{id}` endpoint with `UpdateCargoRequest` → `ActualizarCargoCommand` (200/400/404/409)
- [x] 4.5 Add `DELETE /{id}` endpoint → `EliminarCargoCommand` with `User.GetUserId()` (204/404/409)
- [x] 4.6 Add `POST /{id}/reactivate` endpoint → `ReactivarCargoCommand` (200/404)

## Phase 5: Tests

- [x] 5.1 Create `CrearCargoCommandHandlerTests` — valid create, duplicate name, deleted name reuse
- [x] 5.2 Create `ActualizarCargoCommandHandlerTests` — valid update, not found, duplicate other name, not found (deleted)
- [x] 5.3 Create `EliminarCargoCommandHandlerTests` — valid soft delete, not found, already deleted, blocked by Puestos, blocked by CargoSkills
- [x] 5.4 Create `ReactivarCargoCommandHandlerTests` — valid reactivation, idempotent on active, not found
- [x] 5.5 Create `GetCargoByIdQueryHandlerTests` — found active, not found, soft-deleted returns failure
- [x] 5.6 Create `CrearCargoValidatorTests` — valid, empty nombre, nombre > 150, descripcion > 500
- [x] 5.7 Create `ActualizarCargoValidatorTests` — valid, empty nombre, id <= 0
- [x] 5.8 Create `EliminarCargoValidatorTests` — valid, id <= 0, eliminadoPor <= 0
- [x] 5.9 Create `ReactivarCargoValidatorTests` — valid, id <= 0
- [x] 5.10 Update `GetAllCargosQueryHandlerTests` to use `Cargo` domain entity instead of `CargoEntity`
- [x] 5.11 Create `CargosTests` integration tests — POST 201/400/409, GET by id 200/404, PUT 200/404/409, DELETE 204/404/409, reactivate 200/404

## Phase 6: DI + Cleanup

- [x] 6.1 Verify all handler registrations in `ServiceCollectionExtensions.cs` are correct and complete
- [x] 6.2 Run `dotnet build` and fix any compilation errors from the CargoEntity → Cargo migration
- [x] 6.3 Run `dotnet test` and verify all existing tests still pass after entity migration
