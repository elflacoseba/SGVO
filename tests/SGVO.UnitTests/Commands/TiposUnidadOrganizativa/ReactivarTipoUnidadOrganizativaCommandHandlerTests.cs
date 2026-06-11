using SGVO.Application.Features.TiposUnidadOrganizativa.Commands;
using SGVO.Infrastructure.Commands;
using SGVO.Infrastructure.Persistence.Entities;
using SGVO.UnitTests.Queries;

namespace SGVO.UnitTests.Commands.TiposUnidadOrganizativa;

/// <summary>
/// Unit tests for ReactivarTipoUnidadOrganizativaCommandHandler.
/// </summary>
public class ReactivarTipoUnidadOrganizativaCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithDeletedType_Reactivates()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        dbContext.TiposUnidadOrganizativa.Add(new TipoUnidadOrganizativaEntity
        {
            Id = 1,
            Nombre = "Facultad",
            Activo = false,
            EliminadoEn = DateTime.UtcNow,
            EliminadoPor = 42,
            CreadoEn = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync();

        var handler = new ReactivarTipoUnidadOrganizativaCommandHandler(dbContext);
        var command = new ReactivarTipoUnidadOrganizativaCommand(1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value!.Activo);

        var entity = dbContext.TiposUnidadOrganizativa.First(e => e.Id == 1);
        Assert.True(entity.Activo);
        Assert.Null(entity.EliminadoEn);
        Assert.Null(entity.EliminadoPor);
    }

    [Fact]
    public async Task Handle_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var handler = new ReactivarTipoUnidadOrganizativaCommandHandler(dbContext);
        var command = new ReactivarTipoUnidadOrganizativaCommand(999);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
    }

    [Fact]
    public async Task Handle_WithAlreadyActiveType_ReturnsConflict()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        dbContext.TiposUnidadOrganizativa.Add(new TipoUnidadOrganizativaEntity
        {
            Id = 1,
            Nombre = "Facultad",
            Activo = true,
            CreadoEn = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync();

        var handler = new ReactivarTipoUnidadOrganizativaCommandHandler(dbContext);
        var command = new ReactivarTipoUnidadOrganizativaCommand(1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("CONFLICT", result.ErrorCode);
    }

    [Fact]
    public async Task Handle_WithNameConflict_ReturnsConflict()
    {
        // Arrange — another active type has the same name
        var dbContext = InMemoryDbContextFactory.Create();
        dbContext.TiposUnidadOrganizativa.AddRange(
            new TipoUnidadOrganizativaEntity { Id = 1, Nombre = "Facultad", Activo = false, EliminadoEn = DateTime.UtcNow, EliminadoPor = 1, CreadoEn = DateTime.UtcNow },
            new TipoUnidadOrganizativaEntity { Id = 2, Nombre = "Facultad", Activo = true, CreadoEn = DateTime.UtcNow }
        );
        await dbContext.SaveChangesAsync();

        var handler = new ReactivarTipoUnidadOrganizativaCommandHandler(dbContext);
        var command = new ReactivarTipoUnidadOrganizativaCommand(1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("CONFLICT", result.ErrorCode);
    }

    [Fact]
    public async Task Handle_WithNoNameConflict_Succeeds()
    {
        // Arrange — deleted type with unique name can be reactivated
        var dbContext = InMemoryDbContextFactory.Create();
        dbContext.TiposUnidadOrganizativa.Add(new TipoUnidadOrganizativaEntity
        {
            Id = 1,
            Nombre = "Facultad",
            Activo = false,
            EliminadoEn = DateTime.UtcNow,
            EliminadoPor = 1,
            CreadoEn = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync();

        var handler = new ReactivarTipoUnidadOrganizativaCommandHandler(dbContext);
        var command = new ReactivarTipoUnidadOrganizativaCommand(1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Facultad", result.Value!.Nombre);
        Assert.True(result.Value.Activo);
    }
}
