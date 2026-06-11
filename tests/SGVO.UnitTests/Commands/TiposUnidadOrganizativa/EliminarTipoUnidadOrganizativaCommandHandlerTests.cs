using SGVO.Application.Features.TiposUnidadOrganizativa.Commands;
using SGVO.Infrastructure.Commands;
using SGVO.Infrastructure.Persistence.Entities;
using SGVO.UnitTests.Queries;

namespace SGVO.UnitTests.Commands.TiposUnidadOrganizativa;

/// <summary>
/// Unit tests for EliminarTipoUnidadOrganizativaCommandHandler.
/// </summary>
public class EliminarTipoUnidadOrganizativaCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidId_SoftDeletes()
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

        var handler = new EliminarTipoUnidadOrganizativaCommandHandler(dbContext);
        var command = new EliminarTipoUnidadOrganizativaCommand(1, 42);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        var entity = dbContext.TiposUnidadOrganizativa.First(e => e.Id == 1);
        Assert.False(entity.Activo);
        Assert.NotNull(entity.EliminadoEn);
        Assert.Equal(42L, entity.EliminadoPor);
    }

    [Fact]
    public async Task Handle_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var handler = new EliminarTipoUnidadOrganizativaCommandHandler(dbContext);
        var command = new EliminarTipoUnidadOrganizativaCommand(999, 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
    }

    [Fact]
    public async Task Handle_WithAlreadyDeletedType_ReturnsNotFound()
    {
        // Arrange
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

        var handler = new EliminarTipoUnidadOrganizativaCommandHandler(dbContext);
        var command = new EliminarTipoUnidadOrganizativaCommand(1, 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
    }

    [Fact]
    public async Task Handle_WithActiveReferences_ReturnsConflict()
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
        dbContext.UnidadesOrganizativas.Add(new UnidadesOrganizativaEntity
        {
            Id = 1,
            Nombre = "Facultad de Ciencias",
            TipoUnidadOrganizativaId = 1,
            Activo = true,
            CreadoEn = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync();

        var handler = new EliminarTipoUnidadOrganizativaCommandHandler(dbContext);
        var command = new EliminarTipoUnidadOrganizativaCommand(1, 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("CONFLICT", result.ErrorCode);
    }

    [Fact]
    public async Task Handle_WithSoftDeletedReferences_Succeeds()
    {
        // Arrange — only active references should block deletion
        var dbContext = InMemoryDbContextFactory.Create();
        dbContext.TiposUnidadOrganizativa.Add(new TipoUnidadOrganizativaEntity
        {
            Id = 1,
            Nombre = "Facultad",
            Activo = true,
            CreadoEn = DateTime.UtcNow
        });
        dbContext.UnidadesOrganizativas.Add(new UnidadesOrganizativaEntity
        {
            Id = 1,
            Nombre = "Facultad Eliminada",
            TipoUnidadOrganizativaId = 1,
            Activo = false,
            EliminadoEn = DateTime.UtcNow,
            EliminadoPor = 1,
            CreadoEn = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync();

        var handler = new EliminarTipoUnidadOrganizativaCommandHandler(dbContext);
        var command = new EliminarTipoUnidadOrganizativaCommand(1, 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }
}
