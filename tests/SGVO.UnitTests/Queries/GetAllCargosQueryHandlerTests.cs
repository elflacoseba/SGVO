using SGVO.Application.Common;
using SGVO.Application.Features.Cargos.Queries;
using SGVO.Infrastructure.Persistence.Entities;
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
            new CargoEntity { Id = 1, Nombre = "Cargo 1", Descripcion = "Desc 1", Activo = true, CreadoEn = DateTime.UtcNow },
            new CargoEntity { Id = 2, Nombre = "Cargo 2", Descripcion = "Desc 2", Activo = true, CreadoEn = DateTime.UtcNow }
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
        dbContext.Cargos.AddRange(
            new CargoEntity { Id = 1, Nombre = "Activo", Activo = true, CreadoEn = DateTime.UtcNow },
            new CargoEntity { Id = 2, Nombre = "Eliminado", Activo = false, EliminadoEn = DateTime.UtcNow, EliminadoPor = 1, CreadoEn = DateTime.UtcNow }
        );
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
            dbContext.Cargos.Add(new CargoEntity
            {
                Id = (ulong)i,
                Nombre = $"Cargo {i}",
                Activo = true,
                CreadoEn = DateTime.UtcNow
            });
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
