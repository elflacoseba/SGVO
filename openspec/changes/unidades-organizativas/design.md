# Design: UnidadesOrganizativas Read-Only Module

## Technical Approach

Build a read-only API module following the established Cargos/Vacantes patterns: domain entity with factory constructor, EF Core entity + configuration, three query handlers (paginated list, by-id, recursive tree), and a controller with three endpoints. The tree endpoint uses a MySQL recursive CTE executed via raw SQL with Dapper-style materialization into a nested DTO structure.

## Architecture Decisions

| Decision | Option | Tradeoff | Decision |
|----------|--------|----------|----------|
| Tree query | Recursive CTE vs in-memory assembly | CTE is single DB round-trip, handles large trees efficiently | **Recursive CTE** |
| GetById response | DTO with nested Parent vs flat | Nested Parent matches spec requirement for parent info | **Nested Parent object + ChildrenCount** |
| Controller base | BaseReadOnlyController vs ControllerBase | BaseController only supports GetAll; we need 3 endpoints | **Full ControllerBase** |
| Tree materialization | EF projection vs raw SQL + manual build | EF cannot project recursive structures; raw SQL with flat result then build tree in C# | **Raw SQL + in-memory tree assembly** |
| Entity Descripcion | Keep vs remove | Column does not exist in DB; causes runtime errors | **Remove from entity and config** |

## Data Flow

```
Client ──→ UnidadesOrganizativasController
                │
                ├── GetAll ──→ GetAllUnidadesOrganizativasQueryHandler ──→ EF LINQ ──→ MySQL
                │
                ├── GetById ──→ GetUnidadOrganizativaByIdQueryHandler ──→ EF LINQ ──→ MySQL
                │
                └── GetTree ──→ GetUnidadesOrganizativasTreeQueryHandler ──→ Raw SQL CTE ──→ MySQL
                                      │
                                      └── Flat rows → in-memory tree assembly → nested DTOs
```

## File Changes

| File | Action | Description |
|------|--------|-------------|
| `src/SGVO.Domain/Entities/UnidadOrganizativa.cs` | Create | Domain entity with factory, `Actualizar()`, `Eliminar()`, `Reactivar()` |
| `src/SGVO.Infrastructure/Persistence/Entities/UnidadesOrganizativaEntity.cs` | Modify | Remove `Descripcion` property |
| `src/SGVO.Infrastructure/Persistence/Configurations/UnidadesOrganizativaConfiguration.cs` | Modify | Remove `Descripcion` configuration |
| `src/SGVO.Application/Features/UnidadesOrganizativas/Queries/UnidadOrganizativaDto.cs` | Create | Flat list DTO |
| `src/SGVO.Application/Features/UnidadesOrganizativas/Queries/UnidadOrganizativaDetailDto.cs` | Create | Detail DTO with optional Parent + ChildrenCount |
| `src/SGVO.Application/Features/UnidadesOrganizativas/Queries/UnidadOrganizativaTreeDto.cs` | Create | Recursive tree DTO with `Children` collection |
| `src/SGVO.Application/Features/UnidadesOrganizativas/Queries/GetAllUnidadesOrganizativasQuery.cs` | Create | Query record with optional `Tipo` filter |
| `src/SGVO.Application/Features/UnidadesOrganizativas/Queries/GetUnidadOrganizativaByIdQuery.cs` | Create | Query record by ID |
| `src/SGVO.Application/Features/UnidadesOrganizativas/Queries/GetUnidadesOrganizativasTreeQuery.cs` | Create | Tree query record |
| `src/SGVO.Infrastructure/Queries/GetAllUnidadesOrganizativasQueryHandler.cs` | Create | Paginated list handler with optional tipo filter |
| `src/SGVO.Infrastructure/Queries/GetUnidadOrganizativaByIdQueryHandler.cs` | Create | Single unit handler with parent join + children count |
| `src/SGVO.Infrastructure/Queries/GetUnidadesOrganizativasTreeQueryHandler.cs` | Create | Recursive CTE handler with in-memory tree assembly |
| `src/SGVO.Api/Controllers/UnidadesOrganizativasController.cs` | Create | Controller with GetAll, GetById, GetTree endpoints |
| `src/SGVO.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs` | Modify | Register 3 query handlers |
| `tests/SGVO.UnitTests/Queries/GetAllUnidadesOrganizativasQueryHandlerTests.cs` | Create | 3 unit tests |
| `tests/SGVO.UnitTests/Queries/GetUnidadOrganizativaByIdQueryHandlerTests.cs` | Create | 3 unit tests |

## Interfaces / Contracts

### Domain Entity — `UnidadOrganizativa.cs`

```csharp
public class UnidadOrganizativa
{
    public ulong Id { get; private set; }
    public string Nombre { get; private set; } = null!;
    public string Tipo { get; private set; } = null!;
    public int? NivelJerarquico { get; private set; }
    public ulong? PadreId { get; private set; }
    public bool Activo { get; private set; }
    public DateTime CreadoEn { get; private set; }
    public DateTime? ModificadoEn { get; private set; }
    public DateTime? EliminadoEn { get; private set; }
    public ulong? EliminadoPor { get; private set; }

    private UnidadOrganizativa() { }

    public UnidadOrganizativa(string nombre, string tipo, ulong? padreId = null, int? nivelJerarquico = 1)
    {
        // validation: nombre required/max 150, tipo required/max 50
        Nombre = nombre.Trim();
        Tipo = tipo.Trim();
        PadreId = padreId;
        NivelJerarquico = nivelJerarquico ?? 1;
        Activo = true;
        CreadoEn = DateTime.UtcNow;
    }

    public void Actualizar(string nombre, string tipo, ulong? padreId, int? nivelJerarquico) { ... }
    public void Eliminar(ulong eliminadoPor) { ... }
    public void Reactivar() { ... }
}
```

### DTOs

```csharp
// Flat list DTO
public class UnidadOrganizativaDto
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public int? NivelJerarquico { get; set; }
    public long? PadreId { get; set; }
    public bool Activo { get; set; }
}

// Detail DTO with parent info
public class UnidadOrganizativaDetailDto
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public int? NivelJerarquico { get; set; }
    public long? PadreId { get; set; }
    public bool Activo { get; set; }
    public ParentInfo? Parent { get; set; }
    public int ChildrenCount { get; set; }

    public record ParentInfo(long Id, string Nombre);
}

// Recursive tree DTO
public class UnidadOrganizativaTreeDto
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public int? NivelJerarquico { get; set; }
    public bool Activo { get; set; }
    public List<UnidadOrganizativaTreeDto> Children { get; set; } = new();
}
```

### Queries

```csharp
public sealed record GetAllUnidadesOrganizativasQuery(PageParameters Pagination, string? Tipo = null)
    : IQuery<PagedResult<UnidadOrganizativaDto>>;

public sealed record GetUnidadOrganizativaByIdQuery(long Id)
    : IQuery<UnidadOrganizativaDetailDto?>;

public sealed record GetUnidadesOrganizativasTreeQuery()
    : IQuery<IReadOnlyList<UnidadOrganizativaTreeDto>>;
```

### CTE Implementation — `GetUnidadesOrganizativasTreeQueryHandler.cs`

The tree handler uses raw SQL because EF Core cannot project recursive structures. The approach:
1. Execute `WITH RECURSIVE` CTE to get flat rows with depth ordering
2. Materialize into flat list of intermediate objects
3. Build nested tree in C# using a dictionary lookup

```csharp
const string sql = """
    WITH RECURSIVE OrgTree AS (
        SELECT Id, Nombre, Tipo, NivelJerarquico, PadreId, Activo, 1 AS Depth
        FROM UnidadesOrganizativas
        WHERE PadreId IS NULL AND EliminadoEn IS NULL AND EliminadoPor IS NULL AND Activo = 1
        UNION ALL
        SELECT u.Id, u.Nombre, u.Tipo, u.NivelJerarquico, u.PadreId, u.Activo, ot.Depth + 1
        FROM UnidadesOrganizativas u
        INNER JOIN OrgTree ot ON ot.Id = u.PadreId
        WHERE u.EliminadoEn IS NULL AND u.EliminadoPor IS NULL AND u.Activo = 1
    )
    SELECT Id, Nombre, Tipo, NivelJerarquico, PadreId, Depth
    FROM OrgTree
    ORDER BY Depth, Id
    """;

// After getting flat rows, build tree:
// 1. Create all nodes with empty Children lists
// 2. Dictionary keyed by Id
// 3. Iterate: if node has PadreId, add to parent's Children
// 4. Return root nodes (PadreId == null)
```

### Controller — `UnidadesOrganizativasController.cs`

```csharp
[ApiController]
[Route("api/v1/unidades-organizativas")]
public class UnidadesOrganizativasController : ControllerBase
{
    private readonly IQueryHandler<GetAllUnidadesOrganizativasQuery, PagedResult<UnidadOrganizativaDto>> _getAll;
    private readonly IQueryHandler<GetUnidadOrganizativaByIdQuery, UnidadOrganizativaDetailDto?> _getById;
    private readonly IQueryHandler<GetUnidadesOrganizativasTreeQuery, IReadOnlyList<UnidadOrganizativaTreeDto>> _getTree;

    // Constructor injection

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? tipo = null,
        CancellationToken ct = default) { ... }

    [HttpGet("{id:ulong}")]
    public async Task<IActionResult> GetById(ulong id, CancellationToken ct) { ... }

    [HttpGet("tree")]
    [EndpointName("GetUnidadesOrganizativasTree")]
    public async Task<IActionResult> GetTree(CancellationToken ct) { ... }
}
```

**Note:** The `tree` route must be defined **before** any `{id}` parameterized route to avoid path conflict. In ASP.NET routing, literal segments take precedence, but placing `tree` first is a safety convention.

### DI Registration

Add to `ServiceCollectionExtensions.AddInfrastructure()`:

```csharp
services.AddScoped<
    IQueryHandler<GetAllUnidadesOrganizativasQuery, PagedResult<UnidadOrganizativaDto>>,
    GetAllUnidadesOrganizativasQueryHandler>();
services.AddScoped<
    IQueryHandler<GetUnidadOrganizativaByIdQuery, UnidadOrganizativaDetailDto?>,
    GetUnidadOrganizativaByIdQueryHandler>();
services.AddScoped<
    IQueryHandler<GetUnidadesOrganizativasTreeQuery, IReadOnlyList<UnidadOrganizativaTreeDto>>,
    GetUnidadesOrganizativasTreeQueryHandler>();
```

## Testing Strategy

| Layer | What to Test | Approach |
|-------|-------------|----------|
| Unit | GetAll handler — active items only, soft-delete exclusion, pagination | EF InMemory, arrange-act-assert |
| Unit | GetById handler — found unit with parent/children, null for missing, null for soft-deleted | EF InMemory with seeded parent-child relationships |
| Unit | Tree handler — not unit tested; tree assembly logic is internal. Integration test would be better suited. | Defer to integration tests |

Test files follow the existing pattern in `tests/SGVO.UnitTests/Queries/` using `InMemoryDbContextFactory`.

## Migration / Rollout

No migration required. The `Descripcion` column never existed in the database — the entity property was a mismatch. Removing it is a pure code correction.

## Open Questions

- [ ] Should the tree endpoint support an optional `tipo` filter? (Not in spec v1, but useful for future)
- [ ] Should `GetById` route use `ulong` constraint (`{id:ulong}`) or `long` (`{id:long}`) to match existing VacantesController pattern? **Decision: use `ulong`** since IDs are `BIGINT UNSIGNED` in DB and `ulong` in entities — more accurate than the existing `long` convention in Cargos/Vacantes DTOs.
