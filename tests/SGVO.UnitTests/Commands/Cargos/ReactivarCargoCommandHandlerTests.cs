using SGVO.Application.Features.Cargos.Commands;
using SGVO.Domain.Entities;
using SGVO.Infrastructure.Commands;
using SGVO.UnitTests.Queries;

namespace SGVO.UnitTests.Commands.Cargos;

/// <summary>
/// Unit tests for ReactivarCargoCommandHandler.
/// </summary>
public class ReactivarCargoCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidReactivation_ReturnsSuccess()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var cargo = new Cargo("Analista", null);
        cargo.Eliminar(1);
        dbContext.Cargos.Add(cargo);
        await dbContext.SaveChangesAsync();

        var repository = new TestRepository<Cargo>(dbContext);
        var handler = new ReactivarCargoCommandHandler(repository, dbContext);
        var command = new ReactivarCargoCommand(cargo.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value!.Activo);
    }

    [Fact]
    public async Task Handle_AlreadyActive_ReturnsSuccess_Idempotent()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var cargo = new Cargo("Analista", null);
        dbContext.Cargos.Add(cargo);
        await dbContext.SaveChangesAsync();

        var repository = new TestRepository<Cargo>(dbContext);
        var handler = new ReactivarCargoCommandHandler(repository, dbContext);
        var command = new ReactivarCargoCommand(cargo.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value!.Activo);
    }

    [Fact]
    public async Task Handle_NotFound_ReturnsFailure()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var repository = new TestRepository<Cargo>(dbContext);
        var handler = new ReactivarCargoCommandHandler(repository, dbContext);
        var command = new ReactivarCargoCommand(999);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
    }
}
