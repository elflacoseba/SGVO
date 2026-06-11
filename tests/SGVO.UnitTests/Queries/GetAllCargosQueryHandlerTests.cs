using SGVO.Application.Common;
using SGVO.Application.Features.Cargos.Queries;
using SGVO.Domain.Entities;
using SGVO.Infrastructure.Queries;

namespace SGVO.UnitTests.Queries;

/// <summary>
/// Tests unitarios para GetAllCargosQueryHandler.
/// </summary>
public class GetAllCargosQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithActiveCargos_ReturnsPagedResult()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        dbContext.Cargos.AddRange(
            new Cargo("Cargo 1", "Desc 1"),
            new Cargo("Cargo 2", "Desc 2")
        );
        await dbContext.SaveChangesAsync();

        var handler = new GetAllCargosQueryHandler(dbContext);
        var query = new GetAllCargosQuery(new PageParameters(1, 10));

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.TotalCount);
        Assert.Equal(2, result.Value.Items.Count);
    }

    [Fact]
    public async Task Handle_WithSoftDeletedCargos_ExcludesThem()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var active = new Cargo("Activo", null);
        var deleted = new Cargo("Eliminado", null);
        deleted.Eliminar(1);
        dbContext.Cargos.AddRange(active, deleted);
        await dbContext.SaveChangesAsync();

        var handler = new GetAllCargosQueryHandler(dbContext);
        var query = new GetAllCargosQuery(new PageParameters(1, 10));

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(1, result.Value.TotalCount);
        Assert.Single(result.Value.Items);
        Assert.Equal("Activo", result.Value.Items[0].Nombre);
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        for (int i = 1; i <= 25; i++)
        {
            dbContext.Cargos.Add(new Cargo($"Cargo {i}", null));
        }
        await dbContext.SaveChangesAsync();

        var handler = new GetAllCargosQueryHandler(dbContext);
        var query = new GetAllCargosQuery(new PageParameters(2, 10));

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(25, result.Value.TotalCount);
        Assert.Equal(3, result.Value.TotalPages);
        Assert.Equal(10, result.Value.Items.Count);
        Assert.Equal(2, result.Value.Page);
    }
}
