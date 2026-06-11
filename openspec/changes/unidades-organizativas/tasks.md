# Tasks: UnidadesOrganizativas Read-Only Module

## Review Workload Forecast

| Field | Value |
|-------|-------|
| Estimated changed lines | ~380–420 |
| 400-line budget risk | Medium |
| Chained PRs recommended | Yes |
| Suggested split | PR 1: Foundation (T1–T3, ~80 lines) → PR 2: Handlers + API (T4–T8, ~200 lines) → PR 3: DI + Tests (T9–T11, ~120 lines) |
| Delivery strategy | ask-on-risk |
| Chain strategy | feature-branch-chain |

Decision needed before apply: Yes
Chained PRs recommended: Yes
Chain strategy: feature-branch-chain
400-line budget risk: Medium

### Suggested Work Units

| Unit | Goal | Likely PR | Notes |
|------|------|-----------|-------|
| 1 | Domain entity + entity mismatch fix + DTOs | PR 1 (base: feature/unidades-organizativas) | Foundation — no handlers yet |
| 2 | Query records + 3 handlers + controller | PR 2 (base: PR 1 branch) | Core read API |
| 3 | DI registration + unit tests | PR 3 (base: PR 2 branch) | Wiring and verification |

## Phase 1: Foundation

- [x] 1.1 Remove `Descripcion` property from `src/SGVO.Infrastructure/Persistence/Entities/UnidadesOrganizativaEntity.cs` and its configuration in `src/SGVO.Infrastructure/Persistence/Configurations/UnidadesOrganizativaConfiguration.cs`. Verify: `dotnet build` compiles without errors.
- [x] 1.2 Create `src/SGVO.Domain/Entities/UnidadOrganizativa.cs` — domain entity with `ulong Id`, factory constructor, `Actualizar()`, `Eliminar()`, `Reactivar()` methods. ~45 lines.
- [x] 1.3 Create `src/SGVO.Application/Features/UnidadesOrganizativas/Queries/UnidadOrganizativaDto.cs` — flat list DTO. ~10 lines.
- [x] 1.4 Create `src/SGVO.Application/Features/UnidadesOrganizativas/Queries/UnidadOrganizativaDetailDto.cs` — detail DTO with optional `Parent` record and `ChildrenCount`. ~15 lines.
- [x] 1.5 Create `src/SGVO.Application/Features/UnidadesOrganizativas/Queries/UnidadOrganizativaTreeDto.cs` — recursive tree DTO with `Children` list. ~10 lines.

## Phase 2: Core Implementation

- [x] 2.1 Create `src/SGVO.Application/Features/UnidadesOrganizativas/Queries/GetAllUnidadesOrganizativasQuery.cs` — sealed record with `PageParameters Pagination` and `string? Tipo`. ~5 lines.
- [x] 2.2 Create `src/SGVO.Application/Features/UnidadesOrganizativas/Queries/GetUnidadOrganizativaByIdQuery.cs` — sealed record with `ulong Id`. ~5 lines.
- [x] 2.3 Create `src/SGVO.Application/Features/UnidadesOrganizativas/Queries/GetUnidadesOrganizativasTreeQuery.cs` — sealed record, no parameters. ~5 lines.
- [x] 2.4 Create `src/SGVO.Infrastructure/Queries/GetAllUnidadesOrganizativasQueryHandler.cs` — paginated list handler with optional `tipo` filter, active-only filter. Follows `GetAllCargosQueryHandler` pattern. ~50 lines.
- [x] 2.5 Create `src/SGVO.Infrastructure/Queries/GetUnidadOrganizativaByIdQueryHandler.cs` — single unit handler with parent join and children count. Returns `UnidadOrganizativaDetailDto?`. ~60 lines.
- [x] 2.6 Create `src/SGVO.Infrastructure/Queries/GetUnidadesOrganizativasTreeQueryHandler.cs` — recursive CTE via raw SQL + in-memory tree assembly. Returns `IReadOnlyList<UnidadOrganizativaTreeDto>`. ~85 lines.
- [x] 2.7 Create `src/SGVO.Api/Controllers/UnidadesOrganizativasController.cs` — `ControllerBase` with `GetAll` (paginated, optional tipo), `GetById` (`{id:ulong}`), `GetTree` (literal route before `{id}`) endpoints. ~60 lines.

## Phase 3: Wiring and Testing

- [x] 3.1 Add 3 handler DI registrations to `src/SGVO.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs`. ~6 lines.
- [x] 3.2 Create `tests/SGVO.UnitTests/Queries/GetAllUnidadesOrganizativasQueryHandlerTests.cs` — 3 tests: active items only, soft-deleted excluded, pagination works. Follow `GetAllCargosQueryHandlerTests` pattern. ~80 lines.
- [x] 3.3 Create `tests/SGVO.UnitTests/Queries/GetUnidadOrganizativaByIdQueryHandlerTests.cs` — 3 tests: found with parent/children, non-existent returns null, soft-deleted returns null. ~80 lines.