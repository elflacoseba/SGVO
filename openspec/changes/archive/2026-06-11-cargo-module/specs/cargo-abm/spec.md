# Cargo ABM Specification

## Purpose

Define the behavior for full CRUD and reactivation of the `Cargo` entity. A `Cargo` represents an organizational position that can be assigned to employees (`Puestos`) and associated with skills (`CargoSkills`). The entity is non-hierarchical and uses soft-delete.

## Requirements

### Requirement: Create Cargo

The system MUST allow creating a new `Cargo` with `Nombre` and optional `Descripcion`. `Nombre` MUST NOT exceed 150 characters. `Descripcion` MUST NOT exceed 500 characters. Names MAY be duplicated.

#### Scenario: Create with valid data

- GIVEN a payload with `Nombre = "Analista Senior"` and `Descripcion = "Análisis de datos avanzado"`
- WHEN `POST /api/v1/cargos` is called
- THEN the response is `201 Created` with `CargoDetailDto`
- AND `Cargo.Id` is assigned by the database

#### Scenario: Name exceeds max length

- GIVEN a payload with `Nombre` of 151 characters
- WHEN `POST /api/v1/cargos` is called
- THEN the response is `400 Bad Request` with validation error for `Nombre`

---

### Requirement: Get Cargo by Id

The system MUST return a single `CargoDetailDto` by its identifier. Soft-deleted cargos MUST NOT be returned.

#### Scenario: Existing active cargo

- GIVEN an active cargo with `Id = 1`
- WHEN `GET /api/v1/cargos/1` is called
- THEN the response is `200 OK` with `CargoDetailDto`

#### Scenario: Soft-deleted cargo

- GIVEN a cargo with `Id = 2` where `EliminadoEn` is not null
- WHEN `GET /api/v1/cargos/2` is called
- THEN the response is `404 Not Found`

---

### Requirement: Update Cargo

The system MUST allow updating `Nombre` and `Descripcion` of an existing active cargo.

#### Scenario: Update with valid data

- GIVEN an active cargo with `Id = 1`
- WHEN `PUT /api/v1/cargos/1` with `Nombre = "Analista Senior II"`
- THEN the response is `200 OK` with updated `CargoDetailDto`
- AND `ModificadoEn` is updated

#### Scenario: Update soft-deleted cargo

- GIVEN a cargo with `Id = 2` where `EliminadoEn` is not null
- WHEN `PUT /api/v1/cargos/2` is called
- THEN the response is `404 Not Found`

---

### Requirement: Delete Cargo

The system MUST soft-delete a cargo by setting `EliminadoEn`, `EliminadoPor`, and `Activo = false`. Deletion MUST be blocked if active `Puestos` or active `CargoSkills` reference the cargo.

#### Scenario: Successful delete

- GIVEN an active cargo with no active `Puestos` or `CargoSkills`
- WHEN `DELETE /api/v1/cargos/{id}` by an authenticated user
- THEN the response is `204 NoContent`
- AND `EliminadoEn` is set, `Activo` is false

#### Scenario: Delete with active Puestos

- GIVEN an active cargo referenced by at least one active `Puesto`
- WHEN `DELETE /api/v1/cargos/{id}` is called
- THEN the response is `409 Conflict` with error code `DELETE_BLOCKED`

#### Scenario: Delete with active CargoSkills

- GIVEN an active cargo referenced by at least one active `CargoSkill`
- WHEN `DELETE /api/v1/cargos/{id}` is called
- THEN the response is `409 Conflict` with error code `DELETE_BLOCKED`

#### Scenario: Delete already soft-deleted cargo

- GIVEN a cargo where `EliminadoEn` is not null
- WHEN `DELETE /api/v1/cargos/{id}` is called
- THEN the response is `404 Not Found`

---

### Requirement: Reactivate Cargo

The system MUST reactivate a soft-deleted cargo by clearing `EliminadoEn` and `EliminadoPor`, and setting `Activo = true`.

#### Scenario: Successful reactivation

- GIVEN a soft-deleted cargo
- WHEN `POST /api/v1/cargos/{id}/reactivate` is called
- THEN the response is `200 OK` with `CargoDetailDto`
- AND `EliminadoEn` is null, `Activo` is true

#### Scenario: Reactivate active cargo

- GIVEN an active cargo (not soft-deleted)
- WHEN `POST /api/v1/cargos/{id}/reactivate` is called
- THEN the response is `200 OK` with the same cargo (no-op idempotent)

#### Scenario: Reactivate non-existent cargo

- GIVEN a cargo id that does not exist
- WHEN `POST /api/v1/cargos/{id}/reactivate` is called
- THEN the response is `404 Not Found`

---

## Validation Rules

| Rule | Scope | Error Code |
|------|-------|------------|
| `Nombre` is required | Structural | `VALIDATION` |
| `Nombre` max 150 chars | Structural | `VALIDATION` |
| `Descripcion` max 500 chars | Structural | `VALIDATION` |
| Delete blocked by active `Puestos` | Business | `DELETE_BLOCKED` |
| Delete blocked by active `CargoSkills` | Business | `DELETE_BLOCKED` |

## API Contract

| Method | Endpoint | Request | Response | Status |
|--------|----------|---------|----------|--------|
| POST | `/api/v1/cargos` | `CreateCargoRequest` | `CargoDetailDto` | 201 |
| GET | `/api/v1/cargos/{id}` | — | `CargoDetailDto` | 200 / 404 |
| PUT | `/api/v1/cargos/{id}` | `UpdateCargoRequest` | `CargoDetailDto` | 200 / 400 / 404 |
| DELETE | `/api/v1/cargos/{id}` | — | — | 204 / 404 / 409 |
| POST | `/api/v1/cargos/{id}/reactivate` | — | `CargoDetailDto` | 200 / 404 |

### DTOs

```csharp
public record CreateCargoRequest(string Nombre, string? Descripcion);
public record UpdateCargoRequest(string Nombre, string? Descripcion);

public class CargoDetailDto
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
    public DateTime CreadoEn { get; set; }
    public DateTime? ModificadoEn { get; set; }
}
```

## Error Scenarios

| Trigger | HTTP Status | Error Code | Message |
|---------|-------------|------------|---------|
| Validation fails | 400 | `VALIDATION` | FluentValidation errors |
| Cargo not found | 404 | `NOT_FOUND` | "Cargo con id {id} no encontrado." |
| Delete blocked | 409 | `DELETE_BLOCKED` | "No se puede eliminar: tiene puestos o skills activos." |

## Test Scenarios

### Unit Tests

| Handler | Cases |
|---------|-------|
| `CrearCargoCommandHandler` | success, max length |
| `ActualizarCargoCommandHandler` | success, not found |
| `EliminarCargoCommandHandler` | success, blocked by Puestos, blocked by CargoSkills, already deleted |
| `ReactivarCargoCommandHandler` | success, idempotent on active, not found |
| `GetCargoByIdQueryHandler` | found, not found (deleted) |

### Integration Tests

| Endpoint | Cases |
|----------|-------|
| `POST /api/v1/cargos` | 201, 400 |
| `GET /api/v1/cargos/{id}` | 200, 404 |
| `PUT /api/v1/cargos/{id}` | 200, 400, 404 |
| `DELETE /api/v1/cargos/{id}` | 204, 404, 409 |
| `POST /api/v1/cargos/{id}/reactivate` | 200, 404 |
