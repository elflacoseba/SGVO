# Proposal: UnidadesOrganizativas Read-Only API Module

## Meta
- **Change**: unidades-organizativas
- **Phase**: spec
- **Date**: 2026-06-10
- **Author**: SDD executor

## Business Problem
Database table exists but no API layer to query org hierarchy. Needed to browse structure, assign vacancies, build tree UIs.

## Target Users
HR Administrators, System Integrators, Auditors, Future Module Developers

## Capabilities (v1 — Read-Only)

### New Capabilities
1. **List Organizational Units** — `GET /api/v1/unidades-organizativas` paginated, filterable by tipo
2. **Get Single Organizational Unit** — `GET /api/v1/unidades-organizativas/{id}` with parent/children
3. **Get Organizational Tree** — `GET /api/v1/unidades-organizativas/tree` nested via recursive CTE
4. **Fix Entity Mismatch** — Remove `Descripcion` property from `UnidadesOrganizativaEntity.cs`
5. **DI Registration** — Register query handlers in `ServiceCollectionExtensions.AddInfrastructure()`
6. **Unit Tests** — xUnit tests for query handlers

### Out of Scope
Create, update, delete, reparent, circular reference validation, inactive unit inclusion.

## Business Rules
- 6 Tipo values: `Facultad`, `Secretaría`, `Dirección`, `Departamento`, `División`, `Área`
- Hierarchy: self-referencing via `PadreId`, max 5 levels, no self-reference, no circular refs
- OnDelete(ClientSetNull): deleted parent → children become roots
- Active/Inactive: `Activo` bool, soft-delete via `EliminadoEn`/`EliminadoPor`
- Query filter: `EliminadoEn IS NULL AND EliminadoPor IS NULL`

## Tradeoffs Decided
- Recursive CTE for tree endpoint
- Dual response: flat list + nested tree
- `Descripcion` → remove from entity (no DB migration)

## Assumptions
1. 5 hierarchy levels are sufficient
2. Tipo values (6) are complete
3. `Descripcion` removal is correct (column never existed in DB)
4. OnDelete(ClientSetNull) is correct behavior

## Delivery Strategy
`ask-on-risk` — seek confirmation before oversized changes.

## Review Budget
400 lines