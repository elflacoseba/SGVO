using SGVO.Application.Features.Cargos.Commands;
using SGVO.Domain.Entities;
using SGVO.Infrastructure.Commands;
using SGVO.Infrastructure.Persistence;
using SGVO.Infrastructure.Persistence.Entities;
using SGVO.UnitTests.Queries;

namespace SGVO.UnitTests.Commands.Cargos;

/// <summary>
/// Unit tests for ActualizarCargoCommandHandler.
/// </summary>
public class ActualizarCargoCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var cargo = new Cargo("Analista", "Desc");
        dbContext.Cargos.Add(cargo);
        await dbContext.SaveChangesAsync();

        var repository = new TestRepository<Cargo>(dbContext);
        var handler = new ActualizarCargoCommandHandler(repository, dbContext);
        var command = new ActualizarCargoCommand(cargo.Id, "Analista Senior", "Updated desc");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Analista Senior", result.Value!.Nombre);
        Assert.Equal("Updated desc", result.Value.Descripcion);
    }

    [Fact]
    public async Task Handle_NotFound_ReturnsFailure()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var repository = new TestRepository<Cargo>(dbContext);
        var handler = new ActualizarCargoCommandHandler(repository, dbContext);
        var command = new ActualizarCargoCommand(999, "Whatever", null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
    }

    [Fact]
    public async Task Handle_SoftDeleted_ReturnsFailure()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var cargo = new Cargo("Analista", null);
        cargo.Eliminar(1);
        dbContext.Cargos.Add(cargo);
        await dbContext.SaveChangesAsync();

        var repository = new TestRepository<Cargo>(dbContext);
        var handler = new ActualizarCargoCommandHandler(repository, dbContext);
        var command = new ActualizarCargoCommand(cargo.Id, "Nuevo Nombre", null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
    }

}
