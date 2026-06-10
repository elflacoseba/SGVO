using Microsoft.EntityFrameworkCore;
using SGVO.Infrastructure.Persistence;

namespace SGVO.UnitTests.Queries;

/// <summary>
/// Helper para crear instancias de SgvoDbContext usando EF Core InMemory.
/// </summary>
public static class InMemoryDbContextFactory
{
    public static SgvoDbContext Create()
    {
        var options = new DbContextOptionsBuilder<SgvoDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new SgvoDbContext(options);
    }
}
