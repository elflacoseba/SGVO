# Delta for UnidadesOrganizativas

## ADDED Requirements

### Requirement: List Organizational Units

The system SHALL return a paginated, flat list of active organizational units from `GET /api/v1/unidades-organizativas`. The endpoint MUST accept `pageNumber` (default 1), `pageSize` (default 20), and an optional `tipo` query parameter for filtering by organizational type. Only units where `EliminadoEn IS NULL AND EliminadoPor IS NULL AND Activo = true` SHALL be returned. The response MUST be a `PagedResult<UnidadOrganizativaDto>` containing `Items`, `TotalCount`, `Page`, `PageSize`, `TotalPages`, `HasNextPage`, and `HasPreviousPage`. Each item SHALL include `Id`, `Nombre`, `Tipo`, `NivelJerarquico`, `PadreId`, and `Activo`. Pagination headers (`X-Pagination-*`) SHOULD be included in the response. A `tipo` filter, when provided, SHALL map to `WHERE Tipo = @tipo` in addition to the active-filter.

#### Scenario: List all active organizational units (no filter)

- GIVEN the request `GET /api/v1/unidades-organizativas?pageNumber=1&pageSize=20`
- WHEN the database contains active and soft-deleted units
- THEN the response status is `200 OK`
- AND the body is a `PagedResult<UnidadOrganizativaDto>` with only active units
- AND soft-deleted units are excluded

#### Scenario: Filter by organizational type

- GIVEN the request `GET /api/v1/unidades-organizativas?tipo=Departamento`
- WHEN units of type `Departamento` exist
- THEN the response contains only units where `Tipo = 'Departamento'`
- AND other types are excluded

#### Scenario: Pagination with pageSize 5, page 2

- GIVEN the request `GET /api/v1/unidades-organizativas?pageNumber=2&pageSize=5`
- THEN `PageSize` is 5 and `Page` is 2 in the response
- AND `HasNextPage` reflects whether more pages exist
- AND `HasPreviousPage` is `true`

#### Scenario: Default pagination

- GIVEN the request `GET /api/v1/unidades-organizativas` with no query params
- THEN `pageNumber` defaults to 1 and `pageSize` defaults to 20
- AND `PageSize` is capped at 100 (PageParameters.Normalize)

### Requirement: Get Single Organizational Unit

The system SHALL return a single organizational unit from `GET /api/v1/unidades-organizativas/{id}` where `id` is an unsigned 64-bit integer. The response MUST be an `UnidadOrganizativaDetailDto` containing `Id`, `Nombre`, `Tipo`, `NivelJerarquico`, `PadreId`, `Activo`, an optional `Parent` object (with `Id` and `Nombre`), and `ChildrenCount`. If the unit is not found or is soft-deleted, the system SHALL return `404 Not Found`. If `id` is not a valid unsigned integer, the system SHALL return `400 Bad Request`.

#### Scenario: Get existing active unit with parent and children

- GIVEN the database contains a unit with `Id=5` that has a parent and two children
- WHEN the request is `GET /api/v1/unidades-organizativas/5`
- THEN the response status is `200 OK`
- AND `Parent` contains the parent's `Id` and `Nombre`
- AND `ChildrenCount` equals 2

#### Scenario: Get root unit (no parent)

- GIVEN the database contains a root unit (no `PadreId`)
- WHEN the request is `GET /api/v1/unidades-organizativas/{rootId}`
- THEN `Parent` is `null` in the response

#### Scenario: Unit not found

- GIVEN no unit exists with the requested `id`
- WHEN the request is `GET /api/v1/unidades-organizativas/9999`
- THEN the response status is `404 Not Found`

#### Scenario: Soft-deleted unit returns 404

- GIVEN a unit exists but has `EliminadoEn` and `EliminadoPor` set
- WHEN the request is `GET /api/v1/unidades-organizativas/{deletedId}`
- THEN the response status is `404 Not Found`

### Requirement: Get Organizational Tree

The system SHALL return a nested tree of active organizational units from `GET /api/v1/unidades-organizativas/tree`. The tree MUST be rooted at units where `PadreId IS NULL`. Each node SHALL include `Id`, `Nombre`, `Tipo`, `NivelJerarquico`, `Activo`, and a recursive `Children` array. The tree MUST be built using a recursive CTE. Only active units SHALL be included. If no units exist, the system SHALL return `200 OK` with an empty array.

#### Scenario: Return full tree rooted at root nodes

- GIVEN the database contains a hierarchy with root at `Id=1` and nested children
- WHEN the request is `GET /api/v1/unidades-organizativas/tree`
- THEN the response is a JSON array with root nodes at level 1
- AND each node recursively contains its children
- AND `Children` is an empty array for leaf nodes

#### Scenario: Empty database returns empty array

- GIVEN the database contains no active organizational units
- WHEN the request is `GET /api/v1/unidades-organizativas/tree`
- THEN the response is `200 OK` with body `[]`

### Requirement: Fix Entity Mismatch (Remove Descripcion)

The `UnidadesOrganizativaEntity` class SHALL NOT contain a `Descripcion` property, and the `UnidadesOrganizativaConfiguration` class SHALL NOT configure a `Descripcion` property. The database column `Descripcion` does not exist and no migration is needed.

(Reason: the DB schema does not have this column; the entity maps to the actual DB structure)

(Migration: no migration needed; the column never existed in the DB)

#### Scenario: Entity has no Descripcion property

- GIVEN the compiled `UnidadesOrganizativaEntity` class
- THEN it does not declare a `Descripcion` property
- AND `UnidadesOrganizativaConfiguration` does not reference `Descripcion`

### Requirement: DI Registration

The system SHALL register the three query handlers in `ServiceCollectionExtensions.AddInfrastructure()`: `GetAllUnidadesOrganizativasQueryHandler`, `GetUnidadOrganizativaByIdQueryHandler`, and `GetUnidadesOrganizativasTreeQueryHandler`.

#### Scenario: Handlers are registered in DI

- GIVEN the application starts
- THEN `IQueryHandler<GetAllUnidadesOrganizativasQuery, PagedResult<UnidadOrganizativaDto>>` is registered
- AND `IQueryHandler<GetUnidadOrganizativaByIdQuery, UnidadOrganizativaDetailDto?>` is registered
- AND `IQueryHandler<GetUnidadesOrganizativasTreeQuery, IReadOnlyList<UnidadOrganizativaTreeDto>>` is registered

### Requirement: Unit Tests

The test project SHALL contain unit tests for the query handlers using xUnit, Moq, and EF InMemory.

#### Scenario: GetAllUnidadesOrganizativasQueryHandler — returns active items only

- GIVEN a database with active and soft-deleted organizational units
- WHEN `GetAllUnidadesOrganizativasQueryHandler.Handle` is invoked
- THEN only units where `EliminadoEn IS NULL AND EliminadoPor IS NULL` are returned

#### Scenario: GetAllUnidadesOrganizativasQueryHandler — excludes soft-deleted units

- GIVEN a database with a unit having `EliminadoEn` and `EliminadoPor` set
- WHEN `GetAllUnidadesOrganizativasQueryHandler.Handle` is invoked
- THEN that unit does not appear in the result

#### Scenario: GetAllUnidadesOrganizativasQueryHandler — pagination works

- GIVEN a database with 25 organizational units
- WHEN a query with `page=2, pageSize=10` is executed
- THEN the result contains 10 items and `TotalCount` is 25

#### Scenario: GetUnidadOrganizativaByIdQueryHandler — returns found unit

- GIVEN a database with an active organizational unit
- WHEN `GetUnidadOrganizativaByIdQueryHandler.Handle(id)` is invoked with that id
- THEN the result contains the unit with `Parent` and `ChildrenCount` populated

#### Scenario: GetUnidadOrganizativaByIdQueryHandler — returns null for non-existent id

- GIVEN no organizational unit exists with the given id
- WHEN `GetUnidadOrganizativaByIdQueryHandler.Handle(9999)` is invoked
- THEN the result is `null`

#### Scenario: GetUnidadOrganizativaByIdQueryHandler — returns null for soft-deleted unit

- GIVEN a soft-deleted organizational unit
- WHEN `GetUnidadOrganizativaByIdQueryHandler.Handle(deletedId)` is invoked
- THEN the result is `null`