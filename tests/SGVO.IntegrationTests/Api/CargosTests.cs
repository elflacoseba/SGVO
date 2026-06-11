using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SGVO.Application.Features.Cargos.Dtos;
using SGVO.Application.Features.Cargos.Queries;
using SGVO.Domain.Entities;
using SGVO.Infrastructure.Persistence;
using SGVO.Infrastructure.Persistence.Entities;

namespace SGVO.IntegrationTests.Api;

/// <summary>
/// Integration tests for Cargos API endpoints.
/// Uses the real local MySQL database with test auth.
/// Tests clean up after themselves.
/// </summary>
[Collection("IntegrationTests")]
public class CargosTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public CargosTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
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

    private static async Task<(SgvoDbContext Context, List<Cargo> Seeded)> SeedDataAsync()
    {
        var context = CreateDbContext();
        var seeded = new List<Cargo>();

        // Clean any leftover test data
        var existingCargos = await context.Cargos
            .Where(c => c.Nombre.StartsWith("TEST_"))
            .ToListAsync();
        context.Cargos.RemoveRange(existingCargos);
        await context.SaveChangesAsync();

        var cargos = new[]
        {
            new Cargo("TEST_Analista Senior", "Análisis de datos"),
            new Cargo("TEST_Gerente", "Gerencia general"),
        };
        var deleted = new Cargo("TEST_Eliminado", null);
        deleted.Eliminar(1);

        context.Cargos.AddRange(cargos);
        context.Cargos.Add(deleted);
        await context.SaveChangesAsync();
        seeded.AddRange(cargos);
        seeded.Add(deleted);

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

    private static async Task CleanupAsync(SgvoDbContext context, List<Cargo> seeded)
    {
        var seededIds = seeded.Select(c => c.Id).ToList();

        // Remove related CargoSkills first (FK constraint)
        var cargoSkills = await context.CargoSkills
            .Where(cs => seededIds.Contains(cs.CargoId))
            .ToListAsync();
        context.CargoSkills.RemoveRange(cargoSkills);
        await context.SaveChangesAsync();

        // Remove related Puestos (FK constraint)
        var puestos = await context.Puestos
            .Where(p => p.CargoId.HasValue && seededIds.Contains(p.CargoId.Value))
            .ToListAsync();
        context.Puestos.RemoveRange(puestos);
        await context.SaveChangesAsync();

        var cargos = await context.Cargos
            .Where(c => seededIds.Contains(c.Id))
            .ToListAsync();
        context.Cargos.RemoveRange(cargos);
        await context.SaveChangesAsync();
        await context.DisposeAsync();
    }

    // GET All
    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var (context, seeded) = await SeedDataAsync();
        try
        {
            var response = await _client.GetAsync("/api/v1/cargos");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("totalCount", content);
        }
        finally { await CleanupAsync(context, seeded); }
    }

    // GET By Id - active
    [Fact]
    public async Task GetById_ExistingActiveCargo_ReturnsOk()
    {
        var (context, seeded) = await SeedDataAsync();
        var targetId = seeded[0].Id;
        try
        {
            var response = await _client.GetAsync($"/api/v1/cargos/{targetId}");
            response.EnsureSuccessStatusCode();
            var cargo = await response.Content.ReadFromJsonAsync<CargoDetailDto>();
            Assert.NotNull(cargo);
            Assert.StartsWith("TEST_", cargo.Nombre);
            Assert.True(cargo.Activo);
        }
        finally { await CleanupAsync(context, seeded); }
    }

    // GET By Id - soft-deleted returns 404
    [Fact]
    public async Task GetById_SoftDeletedCargo_Returns404()
    {
        var (context, seeded) = await SeedDataAsync();
        var deletedId = seeded[2].Id;
        try
        {
            var response = await _client.GetAsync($"/api/v1/cargos/{deletedId}");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        finally { await CleanupAsync(context, seeded); }
    }

    // GET By Id - non-existent returns 404
    [Fact]
    public async Task GetById_NonExistent_Returns404()
    {
        var response = await _client.GetAsync("/api/v1/cargos/999999999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // POST - valid returns 201
    [Fact]
    public async Task Create_ValidRequest_Returns201()
    {
        var request = new CreateCargoRequest("TEST_NuevoCargo", "Descripción de prueba");
        try
        {
            var response = await _client.PostAsJsonAsync("/api/v1/cargos", request);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var cargo = await response.Content.ReadFromJsonAsync<CargoDetailDto>();
            Assert.NotNull(cargo);
            Assert.Equal("TEST_NuevoCargo", cargo.Nombre);
            Assert.True(cargo.Activo);
        }
        finally
        {
            using var ctx = CreateDbContext();
            var entity = await ctx.Cargos.FirstOrDefaultAsync(c => c.Nombre == "TEST_NuevoCargo");
            if (entity is not null)
            {
                ctx.Cargos.Remove(entity);
                await ctx.SaveChangesAsync();
            }
        }
    }

    // POST - empty nombre returns 400
    [Fact]
    public async Task Create_EmptyNombre_Returns400()
    {
        var request = new CreateCargoRequest("", null);
        var response = await _client.PostAsJsonAsync("/api/v1/cargos", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // PUT - valid returns 200
    [Fact]
    public async Task Update_ValidRequest_ReturnsOk()
    {
        var (context, seeded) = await SeedDataAsync();
        var targetId = seeded[0].Id;
        try
        {
            var request = new UpdateCargoRequest("TEST_Analista Senior II", "Updated description");
            var response = await _client.PutAsJsonAsync($"/api/v1/cargos/{targetId}", request);
            response.EnsureSuccessStatusCode();
            var cargo = await response.Content.ReadFromJsonAsync<CargoDetailDto>();
            Assert.NotNull(cargo);
            Assert.Equal("TEST_Analista Senior II", cargo.Nombre);
        }
        finally { await CleanupAsync(context, seeded); }
    }

    // PUT - non-existent returns 404
    [Fact]
    public async Task Update_NonExistent_Returns404()
    {
        var request = new UpdateCargoRequest("Whatever", null);
        var response = await _client.PutAsJsonAsync("/api/v1/cargos/999999999", request);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // DELETE - active cargo with no references returns 204
    [Fact]
    public async Task Delete_ActiveCargoNoReferences_Returns204()
    {
        var (context, seeded) = await SeedDataAsync();
        var targetId = seeded[0].Id;
        try
        {
            var response = await _client.DeleteAsync($"/api/v1/cargos/{targetId}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
        finally { await CleanupAsync(context, seeded); }
    }

    // DELETE - non-existent returns 404
    [Fact]
    public async Task Delete_NonExistent_Returns404()
    {
        var response = await _client.DeleteAsync("/api/v1/cargos/999999999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // DELETE - already deleted returns 404
    [Fact]
    public async Task Delete_AlreadyDeleted_Returns404()
    {
        var (context, seeded) = await SeedDataAsync();
        var deletedId = seeded[2].Id;
        try
        {
            var response = await _client.DeleteAsync($"/api/v1/cargos/{deletedId}");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        finally { await CleanupAsync(context, seeded); }
    }

    // DELETE - with active Puestos returns 409
    [Fact]
    public async Task Delete_WithActivePuestos_Returns409()
    {
        var (context, seeded) = await SeedDataAsync();
        var targetId = seeded[0].Id;

        var puesto = new PuestoEntity
        {
            Nombre = "TEST_Puesto",
            CargoId = targetId,
            Activo = true,
            CreadoEn = DateTime.UtcNow
        };
        context.Puestos.Add(puesto);
        await context.SaveChangesAsync();

        try
        {
            var response = await _client.DeleteAsync($"/api/v1/cargos/{targetId}");
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }
        finally
        {
            context.Puestos.Remove(puesto);
            await context.SaveChangesAsync();
            await CleanupAsync(context, seeded);
        }
    }

    // DELETE - with active CargoSkills returns 409
    [Fact]
    public async Task Delete_WithActiveCargoSkills_Returns409()
    {
        var (context, seeded) = await SeedDataAsync();
        var targetId = seeded[0].Id;

        var skill = new SkillEntity
        {
            Nombre = "TEST_Skill",
            Activo = true,
            CreadoEn = DateTime.UtcNow
        };
        context.Skills.Add(skill);
        await context.SaveChangesAsync();

        var cargoSkill = new CargoSkillEntity
        {
            CargoId = targetId,
            SkillId = skill.Id,
            NivelImportancia = 1,
            Activo = true
        };
        context.CargoSkills.Add(cargoSkill);
        await context.SaveChangesAsync();

        try
        {
            var response = await _client.DeleteAsync($"/api/v1/cargos/{targetId}");
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }
        finally
        {
            context.CargoSkills.Remove(cargoSkill);
            await context.SaveChangesAsync();
            context.Skills.Remove(skill);
            await context.SaveChangesAsync();
            await CleanupAsync(context, seeded);
        }
    }

    // Reactivate - deleted cargo returns 200
    [Fact]
    public async Task Reactivate_DeletedCargo_ReturnsOk()
    {
        var (context, seeded) = await SeedDataAsync();
        var deletedId = seeded[2].Id;
        try
        {
            var response = await _client.PostAsync($"/api/v1/cargos/{deletedId}/reactivate", null);
            response.EnsureSuccessStatusCode();
            var cargo = await response.Content.ReadFromJsonAsync<CargoDetailDto>();
            Assert.NotNull(cargo);
            Assert.True(cargo.Activo);
        }
        finally { await CleanupAsync(context, seeded); }
    }

    // Reactivate - non-existent returns 404
    [Fact]
    public async Task Reactivate_NonExistent_Returns404()
    {
        var response = await _client.PostAsync("/api/v1/cargos/999999999/reactivate", null);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
