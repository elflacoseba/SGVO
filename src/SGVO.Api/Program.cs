using Microsoft.EntityFrameworkCore;
using SGVO.Api.Middleware;
using SGVO.Infrastructure.DependencyInjection;
using SGVO.Infrastructure.Persistence.Generated.Context;
using SGVO.Infrastructure.Persistence.Generated.Entities;

var builder = WebApplication.CreateBuilder(args);

// ─────────────────────────────────────────────────────────────
// Servicios
// ─────────────────────────────────────────────────────────────

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "SGVO API",
        Version = "v1",
        Description = "Sistema de Gestión de Vacantes Organizacionales"
    });
});

// Health checks
builder.Services.AddHealthChecks();

// ProblemDetails para manejo de errores estandarizado
builder.Services.AddProblemDetails();

// Logging
builder.Services.AddLogging();

// Capas de aplicación e infraestructura
builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddPersistence(builder.Configuration);

var app = builder.Build();

// ─────────────────────────────────────────────────────────────
// Middleware
// ─────────────────────────────────────────────────────────────

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseStatusCodePages();

// Middleware personalizado de manejo de excepciones
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

// ─────────────────────────────────────────────────────────────
// Endpoints
// ─────────────────────────────────────────────────────────────

var api = app.MapGroup("/api");

// Health check
api.MapGet("/health", () =>
    Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck");

// Version 1 endpoints
var v1 = api.MapGroup("/v1");

// ── Vacantes ──
v1.MapGet("/vacantes", async (
    SgvoDbContext db,
    CancellationToken ct) =>
{
    var vacantes = await db.Vacantes
        .AsNoTracking()
        .Select(v => new
        {
            v.Id,
            v.PuestoId,
            v.FechaApertura,
            v.FechaCierre,
            v.Motivo,
            v.Estado
        })
        .ToListAsync(ct);

    return Results.Ok(vacantes);
})
.WithName("GetVacantes");

v1.MapGet("/vacantes/{id:long}", async (
    long id,
    SgvoDbContext db,
    CancellationToken ct) =>
{
    var vacante = await db.Vacantes
        .AsNoTracking()
        .FirstOrDefaultAsync(v => v.Id == (ulong)id, ct);

    return vacante is null
        ? Results.NotFound(new { error = $"Vacante con id {id} no encontrada." })
        : Results.Ok(new
        {
            vacante.Id,
            vacante.PuestoId,
            vacante.FechaApertura,
            vacante.FechaCierre,
            vacante.Motivo,
            vacante.Estado
        });
})
.WithName("GetVacanteById");

// ── Cargos ──
v1.MapGet("/cargos", async (
    SgvoDbContext db,
    CancellationToken ct) =>
{
    var cargos = await db.Cargos
        .AsNoTracking()
        .Select(c => new
        {
            c.Id,
            c.Nombre,
            c.Descripcion
        })
        .ToListAsync(ct);

    return Results.Ok(cargos);
})
.WithName("GetCargos");

// ── Postulantes ──
v1.MapGet("/postulantes", async (
    SgvoDbContext db,
    CancellationToken ct) =>
{
    var postulantes = await db.Postulantes
        .AsNoTracking()
        .Select(p => new
        {
            p.Id,
            p.Nombre,
            p.Apellido,
            p.Email,
            p.Origen
        })
        .ToListAsync(ct);

    return Results.Ok(postulantes);
})
.WithName("GetPostulantes");

// ── Skills ──
v1.MapGet("/skills", async (
    SgvoDbContext db,
    CancellationToken ct) =>
{
    var skills = await db.Skills
        .AsNoTracking()
        .Select(s => new
        {
            s.Id,
            s.Nombre,
            s.Categoria
        })
        .ToListAsync(ct);

    return Results.Ok(skills);
})
.WithName("GetSkills");

app.Run();

// Requerido para WebApplicationFactory en tests de integración
public partial class Program { }
