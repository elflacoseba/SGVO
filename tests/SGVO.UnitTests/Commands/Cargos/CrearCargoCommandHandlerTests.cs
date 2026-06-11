using SGVO.Application.Features.Cargos.Commands;
using SGVO.Domain.Entities;
using SGVO.Infrastructure.Commands;
using SGVO.Infrastructure.Persistence;
using SGVO.UnitTests.Queries;

namespace SGVO.UnitTests.Commands.Cargos;

/// <summary>
/// Unit tests for CrearCargoCommandHandler.
/// </summary>
public class CrearCargoCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var repository = new TestRepository<Cargo>(dbContext);
        var handler = new CrearCargoCommandHandler(repository, dbContext);
        var command = new CrearCargoCommand("Analista Senior", "Análisis de datos avanzado");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Analista Senior", result.Value.Nombre);
        Assert.Equal("Análisis de datos avanzado", result.Value.Descripcion);
        Assert.True(result.Value.Activo);
        Assert.True(result.Value.Id > 0);
    }

    [Fact]
    public async Task Handle_WithNullDescripcion_ReturnsSuccess()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var repository = new TestRepository<Cargo>(dbContext);
        var handler = new CrearCargoCommandHandler(repository, dbContext);
        var command = new CrearCargoCommand("Gerente", null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value!.Descripcion);
    }
}
