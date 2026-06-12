using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SGVO.Application.Features.Skills.Dtos;
using SGVO.Application.Features.Skills.Queries;
using SGVO.Domain.Entities;
using SGVO.Infrastructure.Persistence;
using SGVO.Infrastructure.Persistence.Entities;

namespace SGVO.IntegrationTests.Api;

/// <summary>
/// Integration tests for Skills API endpoints.
/// Uses the real local MySQL database with test auth.
/// Tests clean up after themselves.
/// </summary>
[Collection("IntegrationTests")]
public class SkillsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public SkillsTests(WebApplicationFactory<Program> factory)
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

    private static async Task<(SgvoDbContext Context, List<Skill> Seeded)> SeedDataAsync()
    {
        var context = CreateDbContext();
        var seeded = new List<Skill>();

        // Clean any leftover test data
        var existingSkills = await context.Skills
            .Where(s => s.Nombre.StartsWith("TEST_"))
            .ToListAsync();
        context.Skills.RemoveRange(existingSkills);
        await context.SaveChangesAsync();

        var skills = new[]
        {
            new Skill("TEST_Python", "Técnica", "Backend avanzado"),
            new Skill("TEST_JavaScript", "Técnica", "Frontend"),
        };
        var deleted = new Skill("TEST_Eliminado", null, null);
        deleted.Eliminar(1);

        context.Skills.AddRange(skills);
        context.Skills.Add(deleted);
        await context.SaveChangesAsync();
        seeded.AddRange(skills);
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

    private static async Task CleanupAsync(SgvoDbContext context, List<Skill> seeded)
    {
        var seededIds = seeded.Select(s => s.Id).ToList();

        // Remove related CargoSkills first (FK constraint)
        var cargoSkills = await context.CargoSkills
            .Where(cs => seededIds.Contains(cs.SkillId))
            .ToListAsync();
        context.CargoSkills.RemoveRange(cargoSkills);
        await context.SaveChangesAsync();

        // Remove related PersonaSkills (FK constraint)
        var personaSkills = await context.PersonaSkills
            .Where(ps => seededIds.Contains(ps.SkillId))
            .ToListAsync();
        context.PersonaSkills.RemoveRange(personaSkills);
        await context.SaveChangesAsync();

        var skills = await context.Skills
            .Where(s => seededIds.Contains(s.Id))
            .ToListAsync();
        context.Skills.RemoveRange(skills);
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
            var response = await _client.GetAsync("/api/v1/skills");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("totalCount", content);
        }
        finally { await CleanupAsync(context, seeded); }
    }

    // GET By Id - active
    [Fact]
    public async Task GetById_ExistingActiveSkill_ReturnsOk()
    {
        var (context, seeded) = await SeedDataAsync();
        var targetId = seeded[0].Id;
        try
        {
            var response = await _client.GetAsync($"/api/v1/skills/{targetId}");
            response.EnsureSuccessStatusCode();
            var skill = await response.Content.ReadFromJsonAsync<SkillDetailDto>();
            Assert.NotNull(skill);
            Assert.StartsWith("TEST_", skill.Nombre);
            Assert.True(skill.Activo);
        }
        finally { await CleanupAsync(context, seeded); }
    }

    // GET By Id - soft-deleted returns 404
    [Fact]
    public async Task GetById_SoftDeletedSkill_Returns404()
    {
        var (context, seeded) = await SeedDataAsync();
        var deletedId = seeded[2].Id;
        try
        {
            var response = await _client.GetAsync($"/api/v1/skills/{deletedId}");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        finally { await CleanupAsync(context, seeded); }
    }

    // GET By Id - non-existent returns 404
    [Fact]
    public async Task GetById_NonExistent_Returns404()
    {
        var response = await _client.GetAsync("/api/v1/skills/999999999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // POST - valid returns 201
    [Fact]
    public async Task Create_ValidRequest_Returns201()
    {
        var request = new CreateSkillRequest("TEST_NuevoSkill", "Técnica", "Descripción de prueba");
        try
        {
            var response = await _client.PostAsJsonAsync("/api/v1/skills", request);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var skill = await response.Content.ReadFromJsonAsync<SkillDetailDto>();
            Assert.NotNull(skill);
            Assert.Equal("TEST_NuevoSkill", skill.Nombre);
            Assert.Equal("Técnica", skill.Categoria);
            Assert.True(skill.Activo);
        }
        finally
        {
            using var ctx = CreateDbContext();
            var entity = await ctx.Skills.FirstOrDefaultAsync(s => s.Nombre == "TEST_NuevoSkill");
            if (entity is not null)
            {
                ctx.Skills.Remove(entity);
                await ctx.SaveChangesAsync();
            }
        }
    }

    // POST - empty nombre returns 400
    [Fact]
    public async Task Create_EmptyNombre_Returns400()
    {
        var request = new CreateSkillRequest("", null, null);
        var response = await _client.PostAsJsonAsync("/api/v1/skills", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // POST - duplicate name returns 409
    [Fact]
    public async Task Create_DuplicateName_Returns409()
    {
        var (context, seeded) = await SeedDataAsync();
        var existingName = seeded[0].Nombre;
        try
        {
            var request = new CreateSkillRequest(existingName, "Nueva", null);
            var response = await _client.PostAsJsonAsync("/api/v1/skills", request);
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }
        finally { await CleanupAsync(context, seeded); }
    }

    // PUT - valid returns 200
    [Fact]
    public async Task Update_ValidRequest_ReturnsOk()
    {
        var (context, seeded) = await SeedDataAsync();
        var targetId = seeded[0].Id;
        try
        {
            var request = new UpdateSkillRequest("TEST_Python Avanzado", "Backend", "Updated description");
            var response = await _client.PutAsJsonAsync($"/api/v1/skills/{targetId}", request);
            response.EnsureSuccessStatusCode();
            var skill = await response.Content.ReadFromJsonAsync<SkillDetailDto>();
            Assert.NotNull(skill);
            Assert.Equal("TEST_Python Avanzado", skill.Nombre);
            Assert.Equal("Backend", skill.Categoria);
        }
        finally { await CleanupAsync(context, seeded); }
    }

    // PUT - non-existent returns 404
    [Fact]
    public async Task Update_NonExistent_Returns404()
    {
        var request = new UpdateSkillRequest("Whatever", null, null);
        var response = await _client.PutAsJsonAsync("/api/v1/skills/999999999", request);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // PUT - duplicate name returns 409
    [Fact]
    public async Task Update_DuplicateName_Returns409()
    {
        var (context, seeded) = await SeedDataAsync();
        var targetId = seeded[0].Id;
        var otherSkillName = seeded[1].Nombre;
        try
        {
            var request = new UpdateSkillRequest(otherSkillName, "Técnica", null);
            var response = await _client.PutAsJsonAsync($"/api/v1/skills/{targetId}", request);
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }
        finally { await CleanupAsync(context, seeded); }
    }

    // DELETE - active skill with no references returns 204
    [Fact]
    public async Task Delete_ActiveSkillNoReferences_Returns204()
    {
        var (context, seeded) = await SeedDataAsync();
        var targetId = seeded[0].Id;
        try
        {
            var response = await _client.DeleteAsync($"/api/v1/skills/{targetId}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
        finally { await CleanupAsync(context, seeded); }
    }

    // DELETE - non-existent returns 404
    [Fact]
    public async Task Delete_NonExistent_Returns404()
    {
        var response = await _client.DeleteAsync("/api/v1/skills/999999999");
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
            var response = await _client.DeleteAsync($"/api/v1/skills/{deletedId}");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        finally { await CleanupAsync(context, seeded); }
    }

    // DELETE - with active CargoSkills returns 409
    [Fact]
    public async Task Delete_WithActiveCargoSkills_Returns409()
    {
        var (context, seeded) = await SeedDataAsync();
        var targetId = seeded[0].Id;
        var cargoId = 1L; // Pre-existing cargo in DB

        var cargoSkill = new CargoSkillEntity
        {
            CargoId = cargoId,
            SkillId = targetId,
            NivelImportancia = 1,
            Activo = true
        };
        context.CargoSkills.Add(cargoSkill);
        await context.SaveChangesAsync();

        try
        {
            var response = await _client.DeleteAsync($"/api/v1/skills/{targetId}");
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }
        finally
        {
            context.CargoSkills.Remove(cargoSkill);
            await context.SaveChangesAsync();
            await CleanupAsync(context, seeded);
        }
    }

    // DELETE - with active PersonaSkills returns 409
    [Fact]
    public async Task Delete_WithActivePersonaSkills_Returns409()
    {
        var (context, seeded) = await SeedDataAsync();
        var targetId = seeded[0].Id;

        var personaSkill = new PersonaSkillEntity
        {
            PersonaId = 1,
            SkillId = targetId,
            NivelDominio = 3,
            Activo = true
        };
        context.PersonaSkills.Add(personaSkill);
        await context.SaveChangesAsync();

        try
        {
            var response = await _client.DeleteAsync($"/api/v1/skills/{targetId}");
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }
        finally
        {
            context.PersonaSkills.Remove(personaSkill);
            await context.SaveChangesAsync();
            await CleanupAsync(context, seeded);
        }
    }

    // Reactivate - deleted skill returns 200
    [Fact]
    public async Task Reactivate_DeletedSkill_ReturnsOk()
    {
        var (context, seeded) = await SeedDataAsync();
        var deletedId = seeded[2].Id;
        try
        {
            var response = await _client.PostAsync($"/api/v1/skills/{deletedId}/reactivate", null);
            response.EnsureSuccessStatusCode();
            var skill = await response.Content.ReadFromJsonAsync<SkillDetailDto>();
            Assert.NotNull(skill);
            Assert.True(skill.Activo);
        }
        finally { await CleanupAsync(context, seeded); }
    }

    // Reactivate - non-existent returns 404
    [Fact]
    public async Task Reactivate_NonExistent_Returns404()
    {
        var response = await _client.PostAsync("/api/v1/skills/999999999/reactivate", null);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
