# Proposal: Cargo ABM (CRUD + Reactivate)

## Intent

Add full ABM (Create, Read, Update, Delete, Reactivate) for the `Cargo` entity so HR admins can manage organizational positions through the API. Today only `GET /api/v1/cargos` (paginated list) exists; all write operations are missing. This change brings Cargo to parity with `UnidadOrganizativa` but with a simpler, non-hierarchical model.

## Scope

### In Scope
- `GetCargoByIdQuery` + Handler returning `CargoDetailDto`
- `CrearCargoCommand` + Handler + Validator
- `ActualizarCargoCommand` + Handler + Validator
- `EliminarCargoCommand` + Handler + Validator (soft delete with guard)
- `ReactivarCargoCommand` + Handler + Validator
- Request DTOs (`CreateCargoRequest`, `UpdateCargoRequest`)
- Controller endpoints: `POST`, `PUT`, `DELETE`, `POST /{id}/reactivate`
- `DomainConstants.CargoNombreMaxLength` (150) and `CargoDescripcionMaxLength` (500)
- DI registrations for all new handlers and validators
- Unit tests for handlers and validators
- Integration tests for the controller endpoints

### Out of Scope
- CargoSkills management (M:N table is read-only in this change)
- Bulk import / bulk delete
- Audit UI or custom audit rules beyond the existing `SaveChangesInterceptor`
- Authorization/role checks beyond existing `[Authorize]` attribute
- Search/filter endpoints (only GetAll pagination exists)

## Capabilities

### New Capabilities
- `cargo-abm`: Full CRUD and reactivation for organizational positions (`Cargo`).

### Modified Capabilities
- None (pure addition).

## Approach

### Architecture
- **Domain entity first**: Use `SGVO.Domain.Entities.Cargo` directly in command handlers. Call `cargo.Actualizar()`, `cargo.Eliminar()`, `cargo.Reactivar()` — domain methods already enforce invariants.
- **Refactor EF Core mapping**: Replace `CargoEntity` with `Cargo` in the DbContext so `IRepository<Cargo>` works. Update `CargoConfiguration`, `CargoSkillEntity`, `PuestoEntity`, and `UsuarioEntity` navigation references.
- **Queries stay lightweight**: Read handlers continue using `DbContext` + `AsNoTracking()` + projection to DTOs (no change to read pattern).
- **Writes use repository**: Command handlers use `IRepository<Cargo>` for `Add`/`Update`, then `DbContext.SaveChangesAsync()`.
- **Validation in two layers**: FluentValidation validators for structural checks (max length, not empty), domain constructor/methods for business invariants.

### Guard Rule
- **Delete**: Reject if `Puestos` table has active (non-soft-deleted) rows referencing the cargo.
- **Reactivate**: No additional guard (simpler than `UnidadOrganizativa` which has parent validation).

### Simplifications vs UnidadOrganizativa
- No hierarchy → no `PadreId`, no tree endpoint, no parent validation.
- No `TipoUnidadOrganizativaId` foreign key validation.

## Affected Areas

| Area | Impact | Description |
|------|--------|-------------|
| `src/SGVO.Domain/Entities/Cargo.cs` | Modified | Add `IEntity` interface if missing; ensure EF Core private constructor works |
| `src/SGVO.Shared/DomainConstants.cs` | Modified | Add `CargoNombreMaxLength`, `CargoDescripcionMaxLength` |
| `src/SGVO.Application/Features/Cargos/Queries/` | New | `GetCargoByIdQuery`, `CargoDetailDto` |
| `src/SGVO.Application/Features/Cargos/Commands/` | New | `CrearCargoCommand`, `ActualizarCargoCommand`, `EliminarCargoCommand`, `ReactivarCargoCommand` + Validators |
| `src/SGVO.Application/Features/Cargos/Dtos/` | New | `CreateCargoRequest`, `UpdateCargoRequest` |
| `src/SGVO.Infrastructure/Commands/` | New | `CrearCargoCommandHandler`, `ActualizarCargoCommandHandler`, `EliminarCargoCommandHandler`, `ReactivarCargoCommandHandler` |
| `src/SGVO.Infrastructure/Queries/` | New | `GetCargoByIdQueryHandler` |
| `src/SGVO.Infrastructure/Persistence/` | Modified | `SgvoDbContext` (swap `CargoEntity` → `Cargo`), `CargoConfiguration`, `CargoSkillEntity`, `PuestoEntity`, `UsuarioEntity` |
| `src/SGVO.Api/Controllers/CargosController.cs` | Modified | Extend from `ControllerBase`; add POST, PUT, DELETE, reactivate endpoints |
| `src/SGVO.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs` | Modified | Register new handlers and validators |
| `tests/SGVO.UnitTests/Commands/Cargos/` | New | Handler + validator unit tests |
| `tests/SGVO.IntegrationTests/Api/CargosTests.cs` | New | Integration tests for endpoints |

## Risks

| Risk | Likelihood | Mitigation |
|------|------------|------------|
| EF Core mapping `Cargo` domain entity breaks existing reads | Medium | Run full integration test suite after mapping change; keep `AsNoTracking` projections unchanged |
| `CargoEntity` removal affects other modules (Puestos, CargoSkills) | Medium | Update all `CargoEntity` references in `PuestoEntity`, `CargoSkillEntity`, `UsuarioEntity`; verify FK constraints remain |
| Domain validation (`Cargo` constructor) throws before FluentValidation | Low | Ensure validators mirror domain rules so API returns 400 instead of 500 |
| Soft-delete guard on `Puestos` causes unexpected conflicts | Low | Document in API response message; guard only checks `EliminadoEn == null` |

## Rollback Plan

1. Revert `SgvoDbContext` to use `DbSet<CargoEntity>` and restore `CargoConfiguration`.
2. Restore `CargosController` to `BaseReadOnlyController`.
3. Remove new DI registrations.
4. Database schema is unchanged (no migrations needed — soft delete is column-level).

## Dependencies

- Existing `SaveChangesInterceptor` for audit logging (no new dependency).
- No database migration required (schema already exists).

## Success Criteria

- [ ] `GET /api/v1/cargos/{id}` returns 200 with `CargoDetailDto` or 404 if not found / soft-deleted.
- [ ] `POST /api/v1/cargos` returns 201 with new cargo; 400 on invalid payload.
- [ ] `PUT /api/v1/cargos/{id}` returns 200 with updated cargo; 400/404 on invalid or missing.
- [ ] `DELETE /api/v1/cargos/{id}` returns 204 on success; 409 if active `Puestos` reference it.
- [ ] `POST /api/v1/cargos/{id}/reactivate` returns 200 with reactivated cargo.
- [ ] All handlers use `SGVO.Domain.Entities.Cargo` (not `CargoEntity`).
- [ ] `DomainConstants` includes `CargoNombreMaxLength` (150) and `CargoDescripcionMaxLength` (500).
- [ ] Unit tests cover all handlers and validators.
- [ ] Integration tests cover all new endpoints.
- [ ] `dotnet test` passes without regressions.

## Proposal Question Round

Before finalizing, the following assumptions could affect scope or behavior:

1. **Uniqueness of `Nombre`**: Should active `Cargo` names be unique? Currently no unique constraint exists.
2. **Delete guard on `CargoSkills`**: Should deletion also be blocked if the cargo has active `CargoSkills` rows, or only `Puestos`?
3. **Audit coverage**: Should all mutations be automatically audited via the existing `SaveChangesInterceptor`, or is any custom audit logic needed?

If these are already decided elsewhere, we can proceed with the defaults above.
