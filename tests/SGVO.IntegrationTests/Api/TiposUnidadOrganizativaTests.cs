using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SGVO.Application.Features.TiposUnidadOrganizativa.Dtos;
using SGVO.Infrastructure.Persistence;
using SGVO.Infrastructure.Persistence.Entities;

namespace SGVO.IntegrationTests.Api;

/// <summary>
/// Integration tests for TiposUnidadOrganizativa API endpoints.
/// Uses the real local MySQL database with test auth.
/// Tests clean up after themselves.
/// </summary>
public class TiposUnidadOrganizativaTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public TiposUnidadOrganizativaTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // Replace authentication with test handler
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
            });
        });

        _client = _factory.CreateClient();
    }

    private static async Task<(SgvoDbContext Context, List<TipoUnidadOrganizativaEntity> Seeded)> SeedDataAsync()
    {
        var context = CreateDbContext();
        var seeded = new List<TipoUnidadOrganizativaEntity>();

        // Clean any leftover test data
        var existing = await context.TiposUnidadOrganizativa
            .Where(t => t.Nombre.StartsWith("TEST_"))
            .ToListAsync();
        context.TiposUnidadOrganizativa.RemoveRange(existing);

        var tipos = new[]
        {
            new TipoUnidadOrganizativaEntity { Nombre = "TEST_Facultad", Activo = true, CreadoEn = DateTime.UtcNow },
            new TipoUnidadOrganizativaEntity { Nombre = "TEST_Secretaría", Activo = true, CreadoEn = DateTime.UtcNow },
            new TipoUnidadOrganizativaEntity { Nombre = "TEST_Eliminada", Activo = false, EliminadoEn = DateTime.UtcNow, EliminadoPor = 1, CreadoEn = DateTime.UtcNow }
        };

        context.TiposUnidadOrganizativa.AddRange(tipos);
        await context.SaveChangesAsync();
        seeded.AddRange(tipos);

        return (context, seeded);
    }

    private static SgvoDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<SgvoDbContext>()
            .UseMySql(
                "Server=localhost;Database=sgvo;User=root;Password=;",
                ServerVersion.AutoDetect("Server=localhost;Database=sgvo;User=root;Password=;"))
            .Options;
        return new SgvoDbContext(options);
    }

    private static async Task CleanupAsync(SgvoDbContext context, List<TipoUnidadOrganizativaEntity> seeded)
    {
        // Remove seeded test data and any units referencing them
        var seededIds = seeded.Select(t => t.Id).ToList();
        var units = await context.UnidadesOrganizativas
            .Where(u => seededIds.Contains(u.TipoUnidadOrganizativaId))
            .ToListAsync();
        context.UnidadesOrganizativas.RemoveRange(units);

        var tipos = await context.TiposUnidadOrganizativa
            .Where(t => seededIds.Contains(t.Id))
            .ToListAsync();
        context.TiposUnidadOrganizativa.RemoveRange(tipos);
        await context.SaveChangesAsync();
        await context.DisposeAsync();
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        // Arrange
        var (context, seeded) = await SeedDataAsync();

        try
        {
            // Act
            var response = await _client.GetAsync("/api/v1/tipos-unidad-organizativa");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("totalCount", content);
        }
        finally
        {
            await CleanupAsync(context, seeded);
        }
    }

    [Fact]
    public async Task GetById_ExistingActiveType_ReturnsOk()
    {
        // Arrange
        var (context, seeded) = await SeedDataAsync();
        var targetId = seeded[0].Id;

        try
        {
            // Act
            var response = await _client.GetAsync($"/api/v1/tipos-unidad-organizativa/{targetId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var tipo = await response.Content.ReadFromJsonAsync<TipoUnidadOrganizativaDto>();
            Assert.NotNull(tipo);
            Assert.StartsWith("TEST_", tipo.Nombre);
        }
        finally
        {
            await CleanupAsync(context, seeded);
        }
    }

    [Fact]
    public async Task GetById_SoftDeletedType_Returns404()
    {
        // Arrange
        var (context, seeded) = await SeedDataAsync();
        var deletedId = seeded[2].Id; // TEST_Eliminada

        try
        {
            // Act
            var response = await _client.GetAsync($"/api/v1/tipos-unidad-organizativa/{deletedId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        finally
        {
            await CleanupAsync(context, seeded);
        }
    }

    [Fact]
    public async Task GetById_NonExistent_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/tipos-unidad-organizativa/999999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_ValidRequest_Returns201()
    {
        // Arrange
        var request = new CreateTipoUnidadOrganizativaRequest { Nombre = "TEST_NuevoTipo" };

        try
        {
            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/tipos-unidad-organizativa", request);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var tipo = await response.Content.ReadFromJsonAsync<TipoUnidadOrganizativaDto>();
            Assert.NotNull(tipo);
            Assert.Equal("TEST_NuevoTipo", tipo.Nombre);
            Assert.True(tipo.Activo);
        }
        finally
        {
            // Cleanup
            using var ctx = CreateDbContext();
            var entity = await ctx.TiposUnidadOrganizativa.FirstOrDefaultAsync(t => t.Nombre == "TEST_NuevoTipo");
            if (entity is not null)
            {
                ctx.TiposUnidadOrganizativa.Remove(entity);
                await ctx.SaveChangesAsync();
            }
        }
    }

    [Fact]
    public async Task Create_DuplicateNombre_Returns409()
    {
        // Arrange
        var (context, seeded) = await SeedDataAsync();

        try
        {
            var request = new CreateTipoUnidadOrganizativaRequest { Nombre = "TEST_Facultad" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/tipos-unidad-organizativa", request);

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }
        finally
        {
            await CleanupAsync(context, seeded);
        }
    }

    [Fact]
    public async Task Create_EmptyNombre_Returns400()
    {
        // Arrange
        var request = new CreateTipoUnidadOrganizativaRequest { Nombre = "" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/tipos-unidad-organizativa", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_ValidRequest_ReturnsOk()
    {
        // Arrange
        var (context, seeded) = await SeedDataAsync();
        var targetId = seeded[0].Id;

        try
        {
            var request = new UpdateTipoUnidadOrganizativaRequest { Nombre = "TEST_FacultadUpdated" };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/v1/tipos-unidad-organizativa/{targetId}", request);

            // Assert
            response.EnsureSuccessStatusCode();
            var tipo = await response.Content.ReadFromJsonAsync<TipoUnidadOrganizativaDto>();
            Assert.NotNull(tipo);
            Assert.Equal("TEST_FacultadUpdated", tipo.Nombre);
        }
        finally
        {
            await CleanupAsync(context, seeded);
        }
    }

    [Fact]
    public async Task Update_NonExistent_Returns404()
    {
        // Arrange
        var request = new UpdateTipoUnidadOrganizativaRequest { Nombre = "TEST_Whatever" };

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/tipos-unidad-organizativa/999999999", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_DuplicateNombre_Returns409()
    {
        // Arrange
        var (context, seeded) = await SeedDataAsync();
        var targetId = seeded[0].Id;

        try
        {
            var request = new UpdateTipoUnidadOrganizativaRequest { Nombre = "TEST_Secretaría" };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/v1/tipos-unidad-organizativa/{targetId}", request);

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }
        finally
        {
            await CleanupAsync(context, seeded);
        }
    }

    [Fact]
    public async Task Delete_ActiveTypeNoReferences_Returns204()
    {
        // Arrange
        var (context, seeded) = await SeedDataAsync();
        var targetId = seeded[0].Id;

        try
        {
            // Act
            var response = await _client.DeleteAsync($"/api/v1/tipos-unidad-organizativa/{targetId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
        finally
        {
            await CleanupAsync(context, seeded);
        }
    }

    [Fact]
    public async Task Delete_WithActiveReferences_Returns409()
    {
        // Arrange
        var (context, seeded) = await SeedDataAsync();
        var targetId = seeded[0].Id;

        // Add a unit referencing this type
        var unit = new UnidadesOrganizativaEntity
        {
            Nombre = "TEST_Unidad",
            TipoUnidadOrganizativaId = targetId,
            Activo = true,
            CreadoEn = DateTime.UtcNow
        };
        context.UnidadesOrganizativas.Add(unit);
        await context.SaveChangesAsync();

        try
        {
            // Act
            var response = await _client.DeleteAsync($"/api/v1/tipos-unidad-organizativa/{targetId}");

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }
        finally
        {
            // Cleanup unit first
            context.UnidadesOrganizativas.Remove(unit);
            await context.SaveChangesAsync();
            await CleanupAsync(context, seeded);
        }
    }

    [Fact]
    public async Task Delete_AlreadyDeleted_Returns404()
    {
        // Arrange
        var (context, seeded) = await SeedDataAsync();
        var deletedId = seeded[2].Id; // Already soft-deleted

        try
        {
            // Act
            var response = await _client.DeleteAsync($"/api/v1/tipos-unidad-organizativa/{deletedId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        finally
        {
            await CleanupAsync(context, seeded);
        }
    }

    [Fact]
    public async Task Reactivate_DeletedType_ReturnsOk()
    {
        // Arrange
        var (context, seeded) = await SeedDataAsync();
        var deletedId = seeded[2].Id;

        try
        {
            // Act
            var response = await _client.PostAsync($"/api/v1/tipos-unidad-organizativa/{deletedId}/reactivate", null);

            // Assert
            response.EnsureSuccessStatusCode();
            var tipo = await response.Content.ReadFromJsonAsync<TipoUnidadOrganizativaDto>();
            Assert.NotNull(tipo);
            Assert.True(tipo.Activo);
        }
        finally
        {
            await CleanupAsync(context, seeded);
        }
    }

    [Fact]
    public async Task Reactivate_ActiveType_Returns409()
    {
        // Arrange
        var (context, seeded) = await SeedDataAsync();
        var activeId = seeded[0].Id;

        try
        {
            // Act
            var response = await _client.PostAsync($"/api/v1/tipos-unidad-organizativa/{activeId}/reactivate", null);

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }
        finally
        {
            await CleanupAsync(context, seeded);
        }
    }

    [Fact]
    public async Task Create_WithoutAuth_Returns401()
    {
        // Arrange — create a client without test auth
        var unauthFactory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove test auth, revert to JWT
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
                });
            });
        });
        var unauthClient = unauthFactory.CreateClient();

        var request = new CreateTipoUnidadOrganizativaRequest { Nombre = "TEST_Unauth" };

        // Act
        var response = await unauthClient.PostAsJsonAsync("/api/v1/tipos-unidad-organizativa", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
