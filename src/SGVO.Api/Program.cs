using SGVO.Api.Middleware;
using SGVO.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// ─────────────────────────────────────────────────────────────
// Servicios
// ─────────────────────────────────────────────────────────────

// Controllers
builder.Services.AddControllers();

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
// Endpoints (Controllers)
// ─────────────────────────────────────────────────────────────

app.MapControllers();

app.Run();

// Requerido para WebApplicationFactory en tests de integración
public partial class Program { }
