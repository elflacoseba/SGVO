using Microsoft.EntityFrameworkCore;
using SGVO.Infrastructure.Persistence;
using Xunit;

namespace SGVO.IntegrationTests;

/// <summary>
/// Shared test collection to prevent parallel execution of integration tests
/// that modify the same MySQL database.
/// </summary>
[CollectionDefinition("IntegrationTests")]
public class IntegrationTestsCollectionDefinition : ICollectionFixture<DatabaseFixture>
{
}

/// <summary>
/// Fixture that ensures the database is clean before tests run.
/// </summary>
public class DatabaseFixture : IDisposable
{
    public DatabaseFixture()
    {
        // Clean up any leftover test data on startup
        CleanupTestData().GetAwaiter().GetResult();
    }

    public void Dispose()
    {
        CleanupTestData().GetAwaiter().GetResult();
    }

    private static async Task CleanupTestData()
    {
        var options = new DbContextOptionsBuilder<SgvoDbContext>()
            .UseMySql(
                "Server=localhost;Database=sgvo;User=root;Password=;",
                ServerVersion.AutoDetect("Server=localhost;Database=sgvo;User=root;Password=;"))
            .Options;

        await using var context = new SgvoDbContext(options);

        // Delete test units first (FK constraint)
        var testUnits = await context.UnidadesOrganizativas
            .Where(u => u.Nombre.StartsWith("TEST_"))
            .ToListAsync();
        context.UnidadesOrganizativas.RemoveRange(testUnits);
        await context.SaveChangesAsync();

        // Delete test tipos
        var testTipos = await context.TiposUnidadOrganizativa
            .Where(t => t.Nombre.StartsWith("TEST_"))
            .ToListAsync();
        context.TiposUnidadOrganizativa.RemoveRange(testTipos);
        await context.SaveChangesAsync();
    }
}
