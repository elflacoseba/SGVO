using SGVO.Application.Features.Cargos.Queries;
using SGVO.Domain.Entities;
using SGVO.Infrastructure.Queries;
using SGVO.UnitTests.Queries;

namespace SGVO.UnitTests.Queries;

/// <summary>
/// Unit tests for GetCargoByIdQueryHandler.
/// </summary>
public class GetCargoByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_FoundActiveCargo_ReturnsDto()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var cargo = new Cargo("Analista Senior", "Análisis avanzado");
        dbContext.Cargos.Add(cargo);
        await dbContext.SaveChangesAsync();

        var handler = new GetCargoByIdQueryHandler(dbContext);
        var query = new GetCargoByIdQuery(cargo.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Analista Senior", result.Value.Nombre);
        Assert.Equal("Análisis avanzado", result.Value.Descripcion);
        Assert.True(result.Value.Activo);
    }

    [Fact]
    public async Task Handle_NotFound_ReturnsNull()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var handler = new GetCargoByIdQueryHandler(dbContext);
        var query = new GetCargoByIdQuery(999);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task Handle_SoftDeleted_ReturnsNull()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var cargo = new Cargo("Analista", null);
        cargo.Eliminar(1);
        dbContext.Cargos.Add(cargo);
        await dbContext.SaveChangesAsync();

        var handler = new GetCargoByIdQueryHandler(dbContext);
        var query = new GetCargoByIdQuery(cargo.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value);
    }
}
