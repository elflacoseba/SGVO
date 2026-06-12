# Design: Cargo ABM (CRUD + Reactivate)

## Technical Approach

Implement full write operations for `Cargo` following the existing CQRS + FluentValidation + Result-pattern architecture, but with a **deliberate deviation from the current project norm**: command handlers will use the domain entity `Cargo` (from `SGVO.Domain`) instead of the infrastructure `CargoEntity`. This aligns with the approved proposal and moves the module toward true domain-driven design. Query handlers continue using `DbContext` + `AsNoTracking()` projections (no change to read pattern).

## Architecture Decisions

| Decision | Option | Tradeoff | Chosen |
|----------|--------|----------|--------|
| **Entity mapping** | A) Keep `CargoEntity` for EF, map manually in handlers | Minimal change, no domain logic | **B) Map `Cargo` domain entity directly in EF Core** — use `Cargo` in DbContext, update all navigation references. Slightly more refactoring, but domain methods (`Actualizar`, `Eliminar`, `Reactivar`) are enforced and the codebase moves toward DDD. |
| **Write data access** | A) Use `DbContext` directly + `CargoEntity` (current project pattern) | Consistent with existing handlers, but bypasses domain invariants | **B) Use `IRepository<Cargo>` for Add/Update, `DbContext.SaveChangesAsync()` for persistence** — repository is already registered, keeps persistence logic abstracted, and domain entity methods enforce invariants. |
| **Query data access** | A) Use `IRepository<Cargo>` | Repository is generic, no projection support | **B) Use `DbContext.Set<Cargo>().AsNoTracking()` with projection to DTOs** — lightweight, no change from existing read pattern. |
| **Validation layering** | A) Single layer (FluentValidation only) | Domain constructor may throw before validation runs | **B) Two-layer: FluentValidation for structural rules (length, required), domain constructor/methods for business invariants** — API returns 400 via validators; domain methods are defensive for non-API paths. |
| **Unique name check** | A) DB unique constraint | Schema change required | **B) Application-level check in handlers** — query for existing active name before create/update, return `CONFLICT`. No migration needed. |

## Data Flow

```
API Request
    ↓
CargosController → ValidationBehavior.HandleAsync()
    ↓
FluentValidation (CrearCargoValidator / ActualizarCargoValidator / etc.)
    ↓
Command Handler (CrearCargoCommandHandler)
    ↓
Domain entity: new Cargo(nombre, descripcion) OR cargo.Actualizar() / Eliminar() / Reactivar()
    ↓
IRepository<Cargo>.Add(cargo) / .Update(cargo)
    ↓
DbContext.SaveChangesAsync()
    ↓
SaveChangesInterceptor → Audit record (Auditorias table)
    ↓
Result<CargoDetailDto> → HTTP response (200/201/204/400/404/409)
```

## File Changes

| File | Action | Description |
|------|--------|-------------|
| `src/SGVO.Domain/Entities/Cargo.cs` | Modify | Add `IEntity` interface; ensure EF Core private constructor is compatible with `DbContext` |
| `src/SGVO.Shared/DomainConstants.cs` | Modify | Add `CargoNombreMaxLength = 150`, `CargoDescripcionMaxLength = 500` |
| `src/SGVO.Infrastructure/Persistence/SgvoDbContext.cs` | Modify | Change `DbSet<CargoEntity>` → `DbSet<Cargo>`; remove `CargoEntity` using |
| `src/SGVO.Infrastructure/Persistence/Configurations/CargoConfiguration.cs` | Modify | Change `IEntityTypeConfiguration<CargoEntity>` → `IEntityTypeConfiguration<Cargo>`; adjust property mappings (nullable `Activo` → `bool` in domain) |
| `src/SGVO.Infrastructure/Persistence/Entities/CargoEntity.cs` | Delete | Replaced by domain entity `Cargo` |
| `src/SGVO.Infrastructure/Persistence/Entities/CargoSkillEntity.cs` | Modify | Change navigation `CargoEntity` → `Cargo` |
| `src/SGVO.Infrastructure/Persistence/Entities/PuestoEntity.cs` | Modify | Change navigation `CargoEntity` → `Cargo` |
| `src/SGVO.Infrastructure/Persistence/Entities/UsuarioEntity.cs` | Modify | Change navigation `CargoEntity` → `Cargo` |
| `src/SGVO.Infrastructure/Persistence/Configurations/CargoSkillConfiguration.cs` | Modify | Change `.HasOne(e => e.Cargo)` generic type from `CargoEntity` to `Cargo` |
| `src/SGVO.Infrastructure/Persistence/Configurations/PuestoConfiguration.cs` | Modify | Change `.HasOne(e => e.Cargo)` generic type from `CargoEntity` to `Cargo` |
| `src/SGVO.Application/Features/Cargos/Queries/CargoDetailDto.cs` | Create | DTO for single cargo detail (Id, Nombre, Descripcion, Activo, CreadoEn, ModificadoEn) |
| `src/SGVO.Application/Features/Cargos/Queries/GetCargoByIdQuery.cs` | Create | `IQuery<CargoDetailDto?>` |
| `src/SGVO.Application/Features/Cargos/Commands/CrearCargoCommand.cs` | Create | `ICommand<CargoDetailDto>` |
| `src/SGVO.Application/Features/Cargos/Commands/ActualizarCargoCommand.cs` | Create | `ICommand<CargoDetailDto>` |
| `src/SGVO.Application/Features/Cargos/Commands/EliminarCargoCommand.cs` | Create | `ICommand<Unit>` |
| `src/SGVO.Application/Features/Cargos/Commands/ReactivarCargoCommand.cs` | Create | `ICommand<CargoDetailDto>` |
| `src/SGVO.Application/Features/Cargos/Commands/CrearCargoValidator.cs` | Create | `NotEmpty`, `MaxLength(150)`, `MaxLength(500)` |
| `src/SGVO.Application/Features/Cargos/Commands/ActualizarCargoValidator.cs` | Create | Same as Crear + `Id > 0` |
| `src/SGVO.Application/Features/Cargos/Commands/EliminarCargoValidator.cs` | Create | `Id > 0`, `EliminadoPor > 0` |
| `src/SGVO.Application/Features/Cargos/Commands/ReactivarCargoValidator.cs` | Create | `Id > 0` |
| `src/SGVO.Application/Features/Cargos/Dtos/CreateCargoRequest.cs` | Create | `Nombre`, `Descripcion` |
| `src/SGVO.Application/Features/Cargos/Dtos/UpdateCargoRequest.cs` | Create | `Nombre`, `Descripcion` |
| `src/SGVO.Infrastructure/Queries/GetCargoByIdQueryHandler.cs` | Create | `DbContext` + `AsNoTracking` + projection to `CargoDetailDto` |
| `src/SGVO.Infrastructure/Commands/CrearCargoCommandHandler.cs` | Create | `IRepository<Cargo>.Add()` + unique-name guard + `SaveChangesAsync()` |
| `src/SGVO.Infrastructure/Commands/ActualizarCargoCommandHandler.cs` | Create | `IRepository<Cargo>.GetByIdAsync()` + `cargo.Actualizar()` + unique-name guard + `.Update()` + `SaveChangesAsync()` |
| `src/SGVO.Infrastructure/Commands/EliminarCargoCommandHandler.cs` | Create | `IRepository<Cargo>.GetByIdAsync()` + `cargo.Eliminar()` + Puestos guard + `.Update()` + `SaveChangesAsync()` |
| `src/SGVO.Infrastructure/Commands/ReactivarCargoCommandHandler.cs` | Create | `IRepository<Cargo>.GetByIdAsync()` + `cargo.Reactivar()` + `.Update()` + `SaveChangesAsync()` |
| `src/SGVO.Infrastructure/Queries/GetAllCargosQueryHandler.cs` | Modify | Change `DbSet<CargoEntity>` → `DbSet<Cargo>`; adjust `Activo` access (now non-nullable `bool`) |
| `src/SGVO.Api/Controllers/CargosController.cs` | Modify | Extend from `ControllerBase`; add `GetById`, `POST`, `PUT`, `DELETE`, `POST /{id}/reactivate` |
| `src/SGVO.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs` | Modify | Register new query handlers, command handlers, and validators |
| `tests/SGVO.UnitTests/Commands/Cargos/CrearCargoCommandHandlerTests.cs` | Create | Unit tests for handler: valid, duplicate name, already deleted |
| `tests/SGVO.UnitTests/Commands/Cargos/ActualizarCargoCommandHandlerTests.cs` | Create | Unit tests for handler: valid, not found, already deleted, duplicate name, Puestos guard |
| `tests/SGVO.UnitTests/Commands/Cargos/EliminarCargoCommandHandlerTests.cs` | Create | Unit tests for handler: valid soft delete, not found, already deleted, active Puestos guard |
| `tests/SGVO.UnitTests/Commands/Cargos/ReactivarCargoCommandHandlerTests.cs` | Create | Unit tests for handler: valid reactivation, not found, already active |
| `tests/SGVO.UnitTests/Commands/Cargos/CrearCargoValidatorTests.cs` | Create | FluentValidation tests |
| `tests/SGVO.UnitTests/Commands/Cargos/ActualizarCargoValidatorTests.cs` | Create | FluentValidation tests |
| `tests/SGVO.UnitTests/Commands/Cargos/EliminarCargoValidatorTests.cs` | Create | FluentValidation tests |
| `tests/SGVO.UnitTests/Commands/Cargos/ReactivarCargoValidatorTests.cs` | Create | FluentValidation tests |
| `tests/SGVO.UnitTests/Queries/GetCargoByIdQueryHandlerTests.cs` | Create | Unit tests: existing, not found, soft-deleted |
| `tests/SGVO.IntegrationTests/Api/CargosTests.cs` | Create | Integration tests for all 5 endpoints (GET by id, POST, PUT, DELETE, reactivate) |

## Interfaces / Contracts

### DTOs

```csharp
public class CargoDetailDto
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
    public DateTime CreadoEn { get; set; }
    public DateTime? ModificadoEn { get; set; }
}

public class CreateCargoRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}

public class UpdateCargoRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}
```

### Commands

```csharp
public sealed record CrearCargoCommand(string Nombre, string? Descripcion) : ICommand<CargoDetailDto>;
public sealed record ActualizarCargoCommand(long Id, string Nombre, string? Descripcion) : ICommand<CargoDetailDto>;
public sealed record EliminarCargoCommand(long Id, long EliminadoPor) : ICommand<Unit>;
public sealed record ReactivarCargoCommand(long Id) : ICommand<CargoDetailDto>;
```

### Domain Entity Mapping

The `Cargo` domain entity already has a private parameterless constructor for EF Core, and public constructor/methods that enforce invariants:

```csharp
public class Cargo
{
    public long Id { get; private set; }
    public string Nombre { get; private set; } = null!;
    public string? Descripcion { get; private set; }
    public bool Activo { get; private set; }
    public DateTime CreadoEn { get; private set; }
    public DateTime? ModificadoEn { get; private set; }
    public DateTime? EliminadoEn { get; private set; }
    public long? EliminadoPor { get; private set; }

    private Cargo() { } // EF Core
    public Cargo(string nombre, string? descripcion = null) { /* validation */ }
    public void Actualizar(string nombre, string? descripcion) { /* validation */ }
    public void Eliminar(long eliminadoPor) { /* soft delete */ }
    public void Reactivar() { /* clear soft delete */ }
}
```

EF Core will map this via `CargoConfiguration` (updated to `IEntityTypeConfiguration<Cargo>`). The private constructor satisfies EF Core's requirement for parameterless construction, while the public constructor and methods enforce domain rules.

## Testing Strategy

| Layer | What to Test | Approach |
|-------|-------------|----------|
| Unit — Validators | `CrearCargoValidator`, `ActualizarCargoValidator`, `EliminarCargoValidator`, `ReactivarCargoValidator` | `FluentValidation.TestHelper` with `ShouldHaveValidationErrorFor` / `ShouldNotHaveAnyValidationErrors` |
| Unit — Query Handlers | `GetCargoByIdQueryHandler` | In-memory `SgvoDbContext` via `InMemoryDbContextFactory` |
| Unit — Command Handlers | `Crear`, `Actualizar`, `Eliminar`, `Reactivar` handlers | In-memory `SgvoDbContext` + `Repository<Cargo>`; assert `Result.IsSuccess/IsFailure` and side effects on `DbContext` |
| Integration | All 5 API endpoints | `WebApplicationFactory<Program>` with `TestAuthHandler` bypass; real MySQL DB via `DbContextOptionsBuilder.UseMySql` |

### Unit Test Patterns (following existing project)

- Handlers use `InMemoryDbContextFactory.Create()` and directly instantiate the handler under test
- Validators use `FluentValidation.TestHelper`
- Integration tests use `Collection("IntegrationTests")` and `IClassFixture<WebApplicationFactory<Program>>`
- Integration tests seed and clean up `TEST_*` prefixed data to avoid affecting real data

## DI Registration

In `ServiceCollectionExtensions.cs`:

```csharp
// Query handlers
services.AddScoped<IQueryHandler<GetCargoByIdQuery, CargoDetailDto?>, GetCargoByIdQueryHandler>();

// Command handlers
services.AddScoped<ICommandHandler<CrearCargoCommand, CargoDetailDto>, CrearCargoCommandHandler>();
services.AddScoped<ICommandHandler<ActualizarCargoCommand, CargoDetailDto>, ActualizarCargoCommandHandler>();
services.AddScoped<ICommandHandler<EliminarCargoCommand, Unit>, EliminarCargoCommandHandler>();
services.AddScoped<ICommandHandler<ReactivarCargoCommand, CargoDetailDto>, ReactivarCargoCommandHandler>();
```

Validators are auto-discovered by `AddValidatorsFromAssemblyContaining<Application.IAssemblyMarker>()` — no explicit registration needed.

## Comparison with UnidadOrganizativa

| Aspect | UnidadOrganizativa | Cargo (this change) |
|--------|-------------------|---------------------|
| **Entity type** | `UnidadesOrganizativaEntity` (infrastructure) | `Cargo` (domain) — **deviation** |
| **Hierarchy** | Self-referencing `PadreId` + tree endpoint | None — flat model |
| **FK validation** | `TipoUnidadOrganizativaId` + `PadreId` existence checks | None — no foreign keys |
| **Delete guards** | Active `Puestos` | Active `Puestos` + active `CargoSkills` |
| **Reactivate guards** | Parent must be active, Tipo must be active | None — no parent/ FK |
| **Unique name** | Not enforced | Enforced in application layer |
| **Read pattern** | `DbContext` + `AsNoTracking` + projection | Same |
| **Write pattern** | `DbContext` directly + mutate entity properties | `IRepository<Cargo>` + domain methods |
| **Controller** | Full `ControllerBase` with 7 endpoints | Full `ControllerBase` with 5 endpoints (no tree) |
| **DTO** | `UnidadOrganizativaDetailDto` with parent/children | `CargoDetailDto` — flat, no navigation |

## Migration / Rollout

No database migration required. The schema already exists (`Cargos` table). The change is purely in the EF Core mapping (switching from `CargoEntity` to `Cargo` domain entity) and application layer. Rollback plan is documented in the proposal.

## Open Questions

1. **Uniqueness enforcement**: Should we enforce unique `Nombre` at the database level with a unique index? Currently decided: application-level check only.
2. **CargoSkills delete guard**: The proposal originally only mentioned `Puestos`. The approved spec says "delete guards (Puestos + CargoSkills)". Should we also block deletion if active `CargoSkills` exist? **Decision: Yes** — check both `Puestos` and `CargoSkills` for `EliminadoEn == null`.
3. **Activo property mismatch**: `CargoEntity.Activo` is `bool?` (nullable) in the database; `Cargo.Activo` is `bool` (non-nullable). EF Core mapping must handle this — `HasDefaultValue(true)` covers the nullable in SQL, but the domain property should always be non-null. Verify this does not cause migration issues.

## Risks

| Risk | Mitigation |
|------|------------|
| `CargoEntity` removal breaks other modules | Update all navigation references in `CargoSkillEntity`, `PuestoEntity`, `UsuarioEntity` and their configurations |
| `Cargo.Activo` (`bool`) vs `CargoEntity.Activo` (`bool?`) mapping mismatch | Ensure EF Core mapping handles the nullable DB column with `.HasDefaultValue(true)`; domain property is always non-null |
| Domain constructor throws on invalid data before FluentValidation runs | Keep validators in sync with domain rules; API path always validates first |
| In-memory EF Core tests fail with `Cargo` domain entity | Private constructor must work; test via `InMemoryDbContextFactory` |
