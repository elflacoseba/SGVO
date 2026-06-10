# SGVO — Sistema de Gestión de Vacantes Organizacionales

Proyecto .NET 10 con arquitectura limpia (Clean Architecture) para la gestión de vacantes, puestos, cargos, personas y postulaciones dentro de una organización universitaria.

---

## Stack Tecnológico

| Capa | Tecnología |
|------|-----------|
| Framework | .NET 10 |
| API | ASP.NET Core Web API (Controllers) |
| ORM | Entity Framework Core 9 |
| Base de datos | MySQL 9.6.0 (Pomelo.EntityFrameworkCore.MySql) |
| Validación | FluentValidation |
| Documentación | Swashbuckle.AspNetCore |
| Autenticación | JWT Bearer (Microsoft.AspNetCore.Authentication.JwtBearer) |
| Tests unitarios | xUnit + Moq + FluentAssertions |
| Tests integración | xUnit + WebApplicationFactory + EF InMemory |

---

## Arquitectura

El proyecto sigue **Clean Architecture** con las siguientes capas:

```
SGVO.sln
├── src/
│   ├── SGVO.Api              ← Entry point (Controllers, middleware, DI)
│   ├── SGVO.Application      ← Casos de uso, DTOs, validadores, abstracciones
│   ├── SGVO.Domain           ← Entidades, interfaces de dominio, eventos, value objects
│   ├── SGVO.Infrastructure   ← EF Core, repositorios, servicios externos
│   └── SGVO.Shared           ← Primitivas compartidas (Result, marcadores de ensamblado)
└── tests/
    ├── SGVO.UnitTests        ← Tests de lógica de negocio y validadores
    └── SGVO.IntegrationTests ← Tests de endpoints HTTP
```

### Dependencias entre capas

- **Api** → Application, Infrastructure, Shared
- **Application** → Domain, Shared
- **Infrastructure** → Application, Domain, Shared
- **Shared** → (sin dependencias)

---

## Estructura de Carpetas

```
src/
├── SGVO.Api/
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── HealthController.cs
│   │   ├── VacantesController.cs
│   │   ├── CargosController.cs
│   │   ├── PostulantesController.cs
│   │   └── SkillsController.cs
│   ├── Middleware/
│   │   └── ExceptionHandlingMiddleware.cs
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   └── Program.cs
├── SGVO.Application/
│   ├── Abstractions/
│   ├── Behaviors/
│   │   └── ValidationBehavior.cs
│   ├── Common/
│   │   ├── ICommand.cs / ICommandHandler.cs
│   │   └── IQuery.cs / IQueryHandler.cs
│   ├── Features/
│   │   ├── Auth/
│   │   │   ├── Commands/
│   │   │   │   ├── LoginCommand.cs
│   │   │   │   ├── RefreshTokenCommand.cs
│   │   │   │   └── LogoutCommand.cs
│   │   │   ├── Dtos/
│   │   │   │   ├── LoginRequestDto.cs
│   │   │   │   ├── LoginResponseDto.cs
│   │   │   │   ├── RefreshTokenRequestDto.cs
│   │   │   │   └── UserDto.cs
│   │   │   └── Queries/
│   │   │       └── GetCurrentUserQuery.cs
│   │   ├── Cargos/Queries/CargoDto.cs
│   │   ├── Postulantes/Queries/PostulanteDto.cs
│   │   ├── Skills/Queries/SkillDto.cs
│   │   └── Vacantes/Queries/
│   │       ├── GetVacantesQuery.cs
│   │       └── VacanteDto.cs
│   └── Validators/
│       └── BaseValidator.cs
├── SGVO.Domain/
│   ├── Entities/
│   │   └── IEntity.cs
│   ├── Enums/
│   ├── Events/
│   │   ├── DomainEvent.cs
│   │   └── IDomainEvent.cs
│   ├── Interfaces/
│   │   ├── IAuthService.cs
│   │   ├── IDateTimeProvider.cs
│   │   ├── IPasswordHasher.cs
│   │   ├── IRefreshTokenRepository.cs
│   │   ├── IRepository{T}.cs
│   │   ├── ITokenService.cs
│   │   └── IUnitOfWork.cs
│   └── ValueObjects/
│       └── ValueObject.cs
├── SGVO.Infrastructure/
│   ├── DependencyInjection/
│   │   └── ServiceCollectionExtensions.cs
│   ├── Persistence/
│   │   ├── SgvoDbContext.cs
│   │   ├── Entities/
│   │   │   ├── Vacante.cs, Cargo.cs, Postulante.cs, ...
│   │   │   └── RefreshTokenEntity.cs
│   │   └── Repositories/
│   │       ├── Repository{T}.cs
│   │       └── RefreshTokenRepository.cs
│   └── Services/
│       ├── AuthService.cs
│       ├── DateTimeProvider.cs
│       ├── JwtTokenService.cs
│       └── PasswordHasher.cs
└── SGVO.Shared/
    ├── IAssemblyMarker.cs
    ├── Result.cs
    └── Result{T}.cs

tests/
├── SGVO.UnitTests/
│   ├── Shared/
│   │   └── ResultTests.cs
│   └── Validators/
│       └── ValidatorTests.cs
└── SGVO.IntegrationTests/
    └── Api/
        └── HealthCheckTests.cs
```

---

## Base de Datos

La base de datos `sgvo` ya existe en MySQL local (`localhost:3306`).

Las entidades y el DbContext se regeneran mediante **scaffolding inverso**:

```bash
dotnet ef dbcontext scaffold \
  "Server=localhost;Port=3306;Database=sgvo;Uid=root;Pwd=;" \
  Pomelo.EntityFrameworkCore.MySql \
  --output-dir src/SGVO.Infrastructure/Persistence/Entities \
  --context-dir src/SGVO.Infrastructure/Persistence \
  --context SgvoDbContext \
  --data-annotations \
  --use-database-names \
  --no-onconfiguring \
  --force \
  --project src/SGVO.Infrastructure/SGVO.Infrastructure.csproj
```

---

## Cómo Ejecutar la API

### Requisitos

- .NET 10 SDK
- MySQL 9.6.0 corriendo en `localhost:3306` con la base de datos `sgvo` creada

### Ejecutar

```bash
dotnet run --project src/SGVO.Api
```

La API se expone en:
- `https://localhost:5001` (por defecto)
- `http://localhost:5000`

### Endpoints disponibles

| Método | Endpoint | Descripción | Autenticación |
|--------|----------|-------------|---------------|
| POST | `/api/v1/auth/login` | Iniciar sesión | No requiere |
| POST | `/api/v1/auth/refresh` | Refrescar token de acceso | No requiere |
| POST | `/api/v1/auth/logout` | Cerrar sesión | Requiere |
| GET | `/api/v1/auth/me` | Obtener usuario actual | Requiere |
| GET | `/api/health` | Health check | No requiere |
| GET | `/api/v1/vacantes` | Listar vacantes | Requiere |
| GET | `/api/v1/vacantes/{id}` | Obtener vacante por ID | Requiere |
| GET | `/api/v1/cargos` | Listar cargos | Requiere |
| GET | `/api/v1/postulantes` | Listar postulantes | Requiere |
| GET | `/api/v1/skills` | Listar skills | Requiere |

### Swagger

En modo desarrollo, la documentación interactiva está disponible en:
- `/swagger`
- `/swagger/v1/swagger.json`

Haz clic en **Authorize** y usa `Bearer {tu_token}` para probar endpoints protegidos.

---

## Cómo Autenticarse

### 1. Iniciar sesión

```bash
curl -X POST https://localhost:5001/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username": "admin", "password": "password"}'
```

Respuesta:
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "dGhpcyBpcyBhIHJlZnJlc2g...",
  "expiresAt": "2025-01-01T12:00:00Z",
  "tokenType": "Bearer"
}
```

### 2. Usar el token en endpoints protegidos

Incluye el header en cada petición:

```bash
curl -X GET https://localhost:5001/api/v1/vacantes \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIs..."
```

### 3. Refrescar el token

```bash
curl -X POST https://localhost:5001/api/v1/auth/refresh \
  -H "Content-Type: application/json" \
  -d '{"refreshToken": "dGhpcyBpcyBhIHJlZnJlc2g..."}'
```

### 4. Cerrar sesión

```bash
curl -X POST https://localhost:5001/api/v1/auth/logout \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIs..." \
  -H "Content-Type: application/json" \
  -d '{"refreshToken": "dGhpcyBpcyBhIHJlZnJlc2g..."}'
```

---

## Cómo Agregar Nuevas Funcionalidades

Seguir el principio de **Clean Architecture** y el flujo de dependencias:

1. **Dominio**: si se necesita una nueva entidad, interfaz o evento, agregarlo en `SGVO.Domain`.
2. **Aplicación**: definir el comando/consulta (CQRS manual) y el DTO en `SGVO.Application`.
3. **Validación**: agregar el validador FluentValidation en `SGVO.Application.Validators`.
4. **Infraestructura**: si se necesita un nuevo servicio externo o repositorio especializado, implementarlo en `SGVO.Infrastructure`.
5. **API**: exponer el endpoint en un controlador dentro de `SGVO.Api/Controllers`.

> No usar MediatR a menos que se solicite explícitamente. Utilizar inyección directa de dependencias.

---

## Tests

### Ejecutar todos los tests

```bash
dotnet test
```

### Tests unitarios

```bash
dotnet test tests/SGVO.UnitTests
```

### Tests de integración

```bash
dotnet test tests/SGVO.IntegrationTests
```

---

## Convenciones del Proyecto

- **Tablas**: PascalCase, plural, español singular (`Personas`, `Ocupaciones`, `Vacantes`).
- **Columnas**: PascalCase, sin prefijos (`Nombre`, `FechaInicio`).
- **PKs**: `Id` (`BIGINT UNSIGNED` → `ulong` en C# por scaffold).
- **FKs**: `[Entidad]Id` (`PersonaId`, `PuestoId`).
- **Timestamps**: `CreadoEn`, `ModificadoEn`.
- **Soft delete**: `Activo` (boolean) + `EliminadoEn` / `EliminadoPor`.
- **Enums**: mapeados a `VARCHAR` en MySQL (no números mágicos).
- **Comentarios de código**: en español (neutral/profesional).
- **Namespaces**: `SGVO.Domain`, `SGVO.Application`, `SGVO.Infrastructure`, `SGVO.Api`.

---

## Estado del Proyecto

- [x] Solución y proyectos creados
- [x] Arquitectura Clean Architecture implementada
- [x] Scaffolding de la base de datos MySQL
- [x] Endpoints de lectura funcionales (Vacantes, Cargos, Postulantes, Skills)
- [x] Health check y Swagger configurados
- [x] Tests unitarios y de integración
- [x] Autenticación y autorización (JWT Bearer)
- [ ] Endpoints de escritura (POST, PUT, DELETE)
- [ ] Pipeline de validación con FluentValidation
- [ ] Interceptor de auditoría (`SaveChangesInterceptor`)
- [ ] CI/CD (GitHub Actions)
