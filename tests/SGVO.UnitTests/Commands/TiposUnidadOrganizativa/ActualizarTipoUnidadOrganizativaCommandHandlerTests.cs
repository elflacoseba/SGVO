using SGVO.Application.Features.TiposUnidadOrganizativa.Commands;
using SGVO.Infrastructure.Commands;
using SGVO.Infrastructure.Persistence.Entities;
using SGVO.UnitTests.Queries;

namespace SGVO.UnitTests.Commands.TiposUnidadOrganizativa;

/// <summary>
/// Unit tests for ActualizarTipoUnidadOrganizativaCommandHandler.
/// </summary>
public class ActualizarTipoUnidadOrganizativaCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidUpdate_ReturnsSuccess()
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

        var handler = new ActualizarTipoUnidadOrganizativaCommandHandler(dbContext);
        var command = new ActualizarTipoUnidadOrganizativaCommand(1, "Facultad de Ciencias");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Facultad de Ciencias", result.Value!.Nombre);
        Assert.NotNull(result.Value.ModificadoEn);
    }

    [Fact]
    public async Task Handle_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var handler = new ActualizarTipoUnidadOrganizativaCommandHandler(dbContext);
        var command = new ActualizarTipoUnidadOrganizativaCommand(999, "Facultad");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
    }

    [Fact]
    public async Task Handle_WithSoftDeletedTarget_ReturnsNotFound()
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

        var handler = new ActualizarTipoUnidadOrganizativaCommandHandler(dbContext);
        var command = new ActualizarTipoUnidadOrganizativaCommand(1, "Nueva Facultad");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
    }

    [Fact]
    public async Task Handle_WithDuplicateNombre_ReturnsConflict()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        dbContext.TiposUnidadOrganizativa.AddRange(
            new TipoUnidadOrganizativaEntity { Id = 1, Nombre = "Facultad", Activo = true, CreadoEn = DateTime.UtcNow },
            new TipoUnidadOrganizativaEntity { Id = 2, Nombre = "Secretaría", Activo = true, CreadoEn = DateTime.UtcNow }
        );
        await dbContext.SaveChangesAsync();

        var handler = new ActualizarTipoUnidadOrganizativaCommandHandler(dbContext);
        var command = new ActualizarTipoUnidadOrganizativaCommand(1, "Secretaría");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("CONFLICT", result.ErrorCode);
    }

    [Fact]
    public async Task Handle_UpdatingToSameName_ReturnsSuccess()
    {
        // Arrange — updating to same name should be allowed
        var dbContext = InMemoryDbContextFactory.Create();
        dbContext.TiposUnidadOrganizativa.Add(new TipoUnidadOrganizativaEntity
        {
            Id = 1,
            Nombre = "Facultad",
            Activo = true,
            CreadoEn = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync();

        var handler = new ActualizarTipoUnidadOrganizativaCommandHandler(dbContext);
        var command = new ActualizarTipoUnidadOrganizativaCommand(1, "Facultad");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Facultad", result.Value!.Nombre);
    }
}
