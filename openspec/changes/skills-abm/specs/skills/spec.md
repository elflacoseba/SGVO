# Skills Specification

## Purpose

Manage skills assignable to positions (`CargoSkills`) and persons (`PersonaSkills`).

## Requirements

### Requirement: Create Skill

The system MUST allow creating a skill with `Nombre` (required, max 150), `Categoria` (optional, max 100), and `Descripcion` (optional, max 500). Duplicate active names MUST be rejected.

#### Scenario: Create with valid data

- GIVEN no skill named "Python" exists
- WHEN a create request with `Nombre="Python"`, `Categoria="Técnica"`, `Descripcion="Backend"` is submitted
- THEN the skill is persisted with `Activo=true` and `CreadoEn` populated
- AND the response returns the created skill ID

#### Scenario: Create with duplicate name

- GIVEN an active skill named "Python" exists
- WHEN a create request with `Nombre="Python"` is submitted
- THEN the operation MUST fail with a conflict error

#### Scenario: Create with invalid data

- GIVEN a request with empty `Nombre` and `Categoria` > 100 chars
- WHEN validated
- THEN the operation MUST fail with validation errors

### Requirement: Get Skill by ID

The system MUST return an active skill by its ID.

#### Scenario: Get existing skill

- GIVEN a skill with `Id=1` exists and `Activo=true`
- WHEN a get-by-id request for `Id=1` is submitted
- THEN the response MUST contain the skill

#### Scenario: Get non-existent skill

- GIVEN no skill exists with `Id=999`
- WHEN a get-by-id request for `Id=999` is submitted
- THEN the response MUST return 404 Not Found

### Requirement: List Skills

The system MUST return a paginated list of active skills, excluding soft-deleted records.

#### Scenario: List with pagination

- GIVEN 25 active skills exist
- WHEN a list request with `page=1`, `pageSize=10` is submitted
- THEN the response MUST contain 10 skills and total 25

#### Scenario: Soft-deleted excluded

- GIVEN 5 active and 3 soft-deleted skills exist
- WHEN a list request is submitted
- THEN only the 5 active skills are returned

### Requirement: Update Skill

The system MUST allow updating a skill's `Nombre`, `Categoria`, and `Descripcion`.

#### Scenario: Update existing skill

- GIVEN a skill with `Id=1` exists
- WHEN an update request with `Nombre="Python Avanzado"` is submitted
- THEN the skill is updated and `ModificadoEn` is set

#### Scenario: Update non-existent skill

- GIVEN no skill exists with `Id=999`
- WHEN an update request for `Id=999` is submitted
- THEN the response MUST return 404 Not Found

#### Scenario: Update with invalid data

- GIVEN a skill with `Id=1` exists
- WHEN an update request with empty `Nombre` is submitted
- THEN the operation MUST fail with validation errors

### Requirement: Delete Skill

The system MUST soft-delete a skill. The system MUST NOT delete a skill referenced by active `CargoSkills` or `PersonaSkills`.

#### Scenario: Delete unreferenced skill

- GIVEN skill `Id=1` exists and is not referenced by active `CargoSkills` or `PersonaSkills`
- WHEN a delete request for `Id=1` by user `Id=5` is submitted
- THEN `Activo` becomes `false`, `EliminadoEn` and `EliminadoPor` are set

#### Scenario: Delete referenced skill

- GIVEN skill `Id=1` is referenced by an active `CargoSkills` record
- WHEN a delete request for `Id=1` is submitted
- THEN the operation MUST fail with a conflict error

#### Scenario: Delete already deleted skill

- GIVEN skill `Id=1` has `Activo=false`
- WHEN a delete request for `Id=1` is submitted
- THEN the operation MUST fail with a conflict error

### Requirement: Reactivate Skill

The system MUST allow reactivating a soft-deleted skill.

#### Scenario: Reactivate deleted skill

- GIVEN skill `Id=1` has `Activo=false`
- WHEN a reactivate request for `Id=1` is submitted
- THEN `Activo` becomes `true` and `EliminadoEn`/`EliminadoPor` are cleared

#### Scenario: Reactivate non-existent skill

- GIVEN no skill exists with `Id=999`
- WHEN a reactivate request for `Id=999` is submitted
- THEN the response MUST return 404 Not Found

#### Scenario: Reactivate already active skill

- GIVEN skill `Id=1` has `Activo=true`
- WHEN a reactivate request for `Id=1` is submitted
- THEN the skill remains active and the response returns the skill (idempotent)
