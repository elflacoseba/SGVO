using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SGVO.Application.Features.UnidadesOrganizativas.Dtos;
using SGVO.Application.Features.UnidadesOrganizativas.Queries;
using SGVO.Infrastructure.Persistence;
using SGVO.Infrastructure.Persistence.Entities;

namespace SGVO.IntegrationTests.Api;

/// <summary>
/// Integration tests for UnidadesOrganizativas write API endpoints.
/// Uses the real local MySQL database with test auth.
/// Tests clean up after themselves.
/// </summary>
[Collection("IntegrationTests")]
public class UnidadesOrganizativasWriteTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public UnidadesOrganizativasWriteTests(WebApplicationFactory<Program> factory)
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

    private static SgvoDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<SgvoDbContext>()
            .UseMySql(
                "Server=localhost;Database=sgvo;User=root;Password=;",
                ServerVersion.AutoDetect("Server=localhost;Database=sgvo;User=root;Password=;"))
            .Options;
        return new SgvoDbContext(options);
    }

    private static async Task<(SgvoDbContext Context, List<TipoUnidadOrganizativaEntity> Tipos, List<UnidadesOrganizativaEntity> Units)> SeedDataAsync()
    {
        var context = CreateDbContext();
        var tipos = new List<TipoUnidadOrganizativaEntity>();
        var units = new List<UnidadesOrganizativaEntity>();

        // Clean any leftover test data (order matters: units before tipos due to FK)
        var existingUnits = await context.UnidadesOrganizativas
            .Where(u => u.Nombre.StartsWith("TEST_"))
            .ToListAsync();
        context.UnidadesOrganizativas.RemoveRange(existingUnits);
        await context.SaveChangesAsync();

        var existingTipos = await context.TiposUnidadOrganizativa
            .Where(t => t.Nombre.StartsWith("TEST_"))
            .ToListAsync();
        context.TiposUnidadOrganizativa.RemoveRange(existingTipos);
        await context.SaveChangesAsync();

        var tipoFacultad = new TipoUnidadOrganizativaEntity
        {
            Nombre = "TEST_Facultad",
            Activo = true,
            CreadoEn = DateTime.UtcNow
        };
        var tipoSecretaria = new TipoUnidadOrganizativaEntity
        {
            Nombre = "TEST_Secretaría",
            Activo = true,
            CreadoEn = DateTime.UtcNow
        };
        var tipoEliminado = new TipoUnidadOrganizativaEntity
        {
            Nombre = "TEST_Eliminado",
            Activo = false,
            EliminadoEn = DateTime.UtcNow,
            EliminadoPor = 1,
            CreadoEn = DateTime.UtcNow
        };

        context.TiposUnidadOrganizativa.AddRange(tipoFacultad, tipoSecretaria, tipoEliminado);
        await context.SaveChangesAsync();
        tipos.AddRange(tipoFacultad, tipoSecretaria, tipoEliminado);

        var unidadPadre = new UnidadesOrganizativaEntity
        {
            Nombre = "TEST_FacultadCiencias",
            TipoUnidadOrganizativaId = tipoFacultad.Id,
            NivelJerarquico = 1,
            Activo = true,
            CreadoEn = DateTime.UtcNow
        };
        context.UnidadesOrganizativas.Add(unidadPadre);
        await context.SaveChangesAsync();
        units.Add(unidadPadre);

        return (context, tipos, units);
    }

    private static async Task CleanupAsync(
        SgvoDbContext context,
        List<TipoUnidadOrganizativaEntity> tipos,
        List<UnidadesOrganizativaEntity> units)
    {
        // Collect all test unit IDs (seeded + any created during test)
        var unitIds = units.Select(u => u.Id).ToList();
        var allTestUnits = await context.UnidadesOrganizativas
            .Where(u => u.Nombre.StartsWith("TEST_") || unitIds.Contains(u.Id))
            .ToListAsync();
        var allTestUnitIds = allTestUnits.Select(u => u.Id).ToList();

        // Delete test Puestos referencing test units via raw SQL (avoids entity/DB Descripcion mismatch)
        if (allTestUnitIds.Count > 0)
        {
            var idsCsv = string.Join(",", allTestUnitIds);
            var sql = "DELETE FROM Puestos WHERE UnidadOrganizativaId IN (" + idsCsv + ") AND Nombre LIKE 'TEST_%'";
            await context.Database.ExecuteSqlRawAsync(sql);
        }

        // Delete test Cargos
        await context.Database.ExecuteSqlRawAsync("DELETE FROM Cargos WHERE Nombre LIKE 'TEST_%'");

        // Delete units BEFORE tipos (FK constraint)
        context.UnidadesOrganizativas.RemoveRange(allTestUnits);
        await context.SaveChangesAsync();

        // Remove test tipos
        var tipoIds = tipos.Select(t => t.Id).ToList();
        var allTestTipos = await context.TiposUnidadOrganizativa
            .Where(t => tipoIds.Contains(t.Id))
            .ToListAsync();
        context.TiposUnidadOrganizativa.RemoveRange(allTestTipos);

        await context.SaveChangesAsync();
        await context.DisposeAsync();
    }

    [Fact]
    public async Task Create_ValidRequest_Returns201()
    {
        // Arrange
        var (context, tipos, units) = await SeedDataAsync();

        try
        {
            var request = new CreateUnidadOrganizativaRequest
            {
                Nombre = "TEST_DeptMatematica",
                TipoUnidadOrganizativaId = (long)tipos[0].Id,
                NivelJerarquico = 2,
                PadreId = (long)units[0].Id
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/unidades-organizativas", request);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var unidad = await response.Content.ReadFromJsonAsync<UnidadOrganizativaDetailDto>();
            Assert.NotNull(unidad);
            Assert.Equal("TEST_DeptMatematica", unidad.Nombre);
            Assert.True(unidad.Activo);
            Assert.Equal((long)units[0].Id, unidad.PadreId);
        }
        finally
        {
            await CleanupAsync(context, tipos, units);
        }
    }

    [Fact]
    public async Task Create_InvalidTipo_Returns404()
    {
        // Arrange
        var (context, tipos, units) = await SeedDataAsync();

        try
        {
            var request = new CreateUnidadOrganizativaRequest
            {
                Nombre = "TEST_Invalid",
                TipoUnidadOrganizativaId = 999999999
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/unidades-organizativas", request);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        finally
        {
            await CleanupAsync(context, tipos, units);
        }
    }

    [Fact]
    public async Task Create_EmptyNombre_Returns400()
    {
        // Arrange
        var (context, tipos, units) = await SeedDataAsync();

        try
        {
            var request = new CreateUnidadOrganizativaRequest
            {
                Nombre = "",
                TipoUnidadOrganizativaId = (long)tipos[0].Id
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/unidades-organizativas", request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        finally
        {
            await CleanupAsync(context, tipos, units);
        }
    }

    [Fact]
    public async Task Create_InvalidTipoId_Returns400()
    {
        // Arrange
        var (context, tipos, units) = await SeedDataAsync();

        try
        {
            var request = new CreateUnidadOrganizativaRequest
            {
                Nombre = "TEST_InvalidTipo",
                TipoUnidadOrganizativaId = 0
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/unidades-organizativas", request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        finally
        {
            await CleanupAsync(context, tipos, units);
        }
    }

    [Fact]
    public async Task Create_WithDeletedParent_Returns400()
    {
        // Arrange
        var (context, tipos, units) = await SeedDataAsync();

        try
        {
            // Create a deleted parent
            var deletedParent = new UnidadesOrganizativaEntity
            {
                Nombre = "TEST_DeletedParent",
                TipoUnidadOrganizativaId = tipos[0].Id,
                Activo = false,
                EliminadoEn = DateTime.UtcNow,
                EliminadoPor = 1,
                CreadoEn = DateTime.UtcNow
            };
            context.UnidadesOrganizativas.Add(deletedParent);
            await context.SaveChangesAsync();
            units.Add(deletedParent);

            var request = new CreateUnidadOrganizativaRequest
            {
                Nombre = "TEST_ChildOfDeleted",
                TipoUnidadOrganizativaId = (long)tipos[0].Id,
                PadreId = (long)deletedParent.Id
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/unidades-organizativas", request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        finally
        {
            await CleanupAsync(context, tipos, units);
        }
    }

    [Fact]
    public async Task Update_ValidRequest_ReturnsOk()
    {
        // Arrange
        var (context, tipos, units) = await SeedDataAsync();

        try
        {
            var request = new UpdateUnidadOrganizativaRequest
            {
                Nombre = "TEST_FacultadUpdated",
                TipoUnidadOrganizativaId = (long)tipos[0].Id,
                NivelJerarquico = 1
            };

            // Act
            var response = await _client.PutAsJsonAsync(
                $"/api/v1/unidades-organizativas/{units[0].Id}", request);

            // Assert
            response.EnsureSuccessStatusCode();
            var unidad = await response.Content.ReadFromJsonAsync<UnidadOrganizativaDetailDto>();
            Assert.NotNull(unidad);
            Assert.Equal("TEST_FacultadUpdated", unidad.Nombre);
        }
        finally
        {
            await CleanupAsync(context, tipos, units);
        }
    }

    [Fact]
    public async Task Update_NonExistent_Returns404()
    {
        // Arrange
        var request = new UpdateUnidadOrganizativaRequest
        {
            Nombre = "TEST_Whatever",
            TipoUnidadOrganizativaId = 1
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/unidades-organizativas/999999999", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_CircularHierarchy_Returns409()
    {
        // Arrange: A -> B, try to set A's parent to B
        var (context, tipos, units) = await SeedDataAsync();

        try
        {
            var child = new UnidadesOrganizativaEntity
            {
                Nombre = "TEST_Child",
                TipoUnidadOrganizativaId = tipos[0].Id,
                PadreId = units[0].Id,
                Activo = true,
                CreadoEn = DateTime.UtcNow
            };
            context.UnidadesOrganizativas.Add(child);
            await context.SaveChangesAsync();
            units.Add(child);

            var request = new UpdateUnidadOrganizativaRequest
            {
                Nombre = units[0].Nombre,
                TipoUnidadOrganizativaId = (long)tipos[0].Id,
                PadreId = (long)child.Id
            };

            // Act
            var response = await _client.PutAsJsonAsync(
                $"/api/v1/unidades-organizativas/{units[0].Id}", request);

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }
        finally
        {
            await CleanupAsync(context, tipos, units);
        }
    }

    [Fact]
    public async Task Delete_ActiveUnitNoPuestos_Returns204()
    {
        // Arrange
        var (context, tipos, units) = await SeedDataAsync();

        try
        {
            // Act
            var response = await _client.DeleteAsync($"/api/v1/unidades-organizativas/{units[0].Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
        finally
        {
            await CleanupAsync(context, tipos, units);
        }
    }

    [Fact]
    public async Task Delete_WithActivePuestos_Returns409()
    {
        // Arrange
        var (context, tipos, units) = await SeedDataAsync();

        try
        {
            // Insert test Cargo and Puesto via raw SQL (avoids entity/DB Descripcion mismatch)
            await context.Database.ExecuteSqlRawAsync(
                "INSERT INTO Cargos (Nombre, Activo, CreadoEn) VALUES ('TEST_CargoDecano', 1, NOW())");
            var cargoId = await context.Cargos
                .Where(c => c.Nombre == "TEST_CargoDecano")
                .Select(c => c.Id)
                .FirstAsync();
            await context.Database.ExecuteSqlRawAsync(
                "INSERT INTO Puestos (Nombre, UnidadOrganizativaId, CargoId, Activo, CreadoEn) VALUES ('TEST_Decano', {0}, {1}, 1, NOW())",
                units[0].Id, cargoId);

            // Act
            var response = await _client.DeleteAsync($"/api/v1/unidades-organizativas/{units[0].Id}");

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }
        finally
        {
            await CleanupAsync(context, tipos, units);
        }
    }

    [Fact]
    public async Task Delete_AlreadyDeleted_Returns404()
    {
        // Arrange
        var (context, tipos, units) = await SeedDataAsync();

        try
        {
            // Soft-delete first
            var response1 = await _client.DeleteAsync($"/api/v1/unidades-organizativas/{units[0].Id}");
            Assert.Equal(HttpStatusCode.NoContent, response1.StatusCode);

            // Act — try to delete again
            var response2 = await _client.DeleteAsync($"/api/v1/unidades-organizativas/{units[0].Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response2.StatusCode);
        }
        finally
        {
            await CleanupAsync(context, tipos, units);
        }
    }

    [Fact]
    public async Task Reactivate_DeletedUnit_ReturnsOk()
    {
        // Arrange
        var (context, tipos, units) = await SeedDataAsync();

        try
        {
            // Soft-delete first
            var deleteResponse = await _client.DeleteAsync($"/api/v1/unidades-organizativas/{units[0].Id}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            // Act
            var response = await _client.PostAsync(
                $"/api/v1/unidades-organizativas/{units[0].Id}/reactivate", null);

            // Assert
            response.EnsureSuccessStatusCode();
            var unidad = await response.Content.ReadFromJsonAsync<UnidadOrganizativaDetailDto>();
            Assert.NotNull(unidad);
            Assert.True(unidad.Activo);
        }
        finally
        {
            await CleanupAsync(context, tipos, units);
        }
    }

    [Fact]
    public async Task Reactivate_ActiveUnit_Returns409()
    {
        // Arrange
        var (context, tipos, units) = await SeedDataAsync();

        try
        {
            // Act
            var response = await _client.PostAsync(
                $"/api/v1/unidades-organizativas/{units[0].Id}/reactivate", null);

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }
        finally
        {
            await CleanupAsync(context, tipos, units);
        }
    }

    [Fact]
    public async Task Reactivate_WithDeletedParent_Returns400()
    {
        // Arrange
        var (context, tipos, units) = await SeedDataAsync();

        try
        {
            var parent = new UnidadesOrganizativaEntity
            {
                Nombre = "TEST_ParentToDelete",
                TipoUnidadOrganizativaId = tipos[0].Id,
                Activo = true,
                CreadoEn = DateTime.UtcNow
            };
            context.UnidadesOrganizativas.Add(parent);
            await context.SaveChangesAsync();
            units.Add(parent);

            var child = new UnidadesOrganizativaEntity
            {
                Nombre = "TEST_ChildOfParent",
                TipoUnidadOrganizativaId = tipos[0].Id,
                PadreId = parent.Id,
                Activo = true,
                CreadoEn = DateTime.UtcNow
            };
            context.UnidadesOrganizativas.Add(child);
            await context.SaveChangesAsync();
            units.Add(child);

            // Delete child first, then parent
            await _client.DeleteAsync($"/api/v1/unidades-organizativas/{child.Id}");
            await _client.DeleteAsync($"/api/v1/unidades-organizativas/{parent.Id}");

            // Act — try to reactivate child while parent is still deleted
            var response = await _client.PostAsync(
                $"/api/v1/unidades-organizativas/{child.Id}/reactivate", null);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        finally
        {
            await CleanupAsync(context, tipos, units);
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

        var request = new CreateUnidadOrganizativaRequest
        {
            Nombre = "TEST_Unauth",
            TipoUnidadOrganizativaId = 1
        };

        // Act
        var response = await unauthClient.PostAsJsonAsync("/api/v1/unidades-organizativas", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WithoutAuth_Returns401()
    {
        // Arrange
        var unauthFactory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
                });
            });
        });
        var unauthClient = unauthFactory.CreateClient();

        // Act
        var response = await unauthClient.DeleteAsync("/api/v1/unidades-organizativas/1");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Update_WithoutAuth_Returns401()
    {
        // Arrange
        var unauthFactory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
                });
            });
        });
        var unauthClient = unauthFactory.CreateClient();

        var request = new UpdateUnidadOrganizativaRequest
        {
            Nombre = "TEST_Unauth",
            TipoUnidadOrganizativaId = 1
        };

        // Act
        var response = await unauthClient.PutAsJsonAsync("/api/v1/unidades-organizativas/1", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Reactivate_WithoutAuth_Returns401()
    {
        // Arrange
        var unauthFactory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
                });
            });
        });
        var unauthClient = unauthFactory.CreateClient();

        // Act
        var response = await unauthClient.PostAsync("/api/v1/unidades-organizativas/1/reactivate", null);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
