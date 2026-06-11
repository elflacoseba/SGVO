using SGVO.Application.Features.UnidadesOrganizativas.Commands;
using SGVO.Infrastructure.Commands;
using SGVO.Infrastructure.Persistence.Entities;
using SGVO.UnitTests.Queries;

namespace SGVO.UnitTests.Commands.UnidadesOrganizativas;

/// <summary>
/// Unit tests for EliminarUnidadOrganizativaCommandHandler.
/// </summary>
public class EliminarUnidadOrganizativaCommandHandlerTests
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
        dbContext.UnidadesOrganizativas.Add(new UnidadesOrganizativaEntity
        {
            Id = 1,
            Nombre = "Facultad de Ciencias",
            TipoUnidadOrganizativaId = 1,
            Activo = true,
            CreadoEn = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync();

        var handler = new EliminarUnidadOrganizativaCommandHandler(dbContext);
        var command = new EliminarUnidadOrganizativaCommand(1, 42);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        var entity = dbContext.UnidadesOrganizativas.First(e => e.Id == 1);
        Assert.False(entity.Activo);
        Assert.NotNull(entity.EliminadoEn);
        Assert.Equal(42L, entity.EliminadoPor);
    }

    [Fact]
    public async Task Handle_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var handler = new EliminarUnidadOrganizativaCommandHandler(dbContext);
        var command = new EliminarUnidadOrganizativaCommand(999, 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
    }

    [Fact]
    public async Task Handle_WithAlreadyDeletedEntity_ReturnsNotFound()
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
            Nombre = "Deleted Unit",
            TipoUnidadOrganizativaId = 1,
            Activo = false,
            EliminadoEn = DateTime.UtcNow,
            EliminadoPor = 1,
            CreadoEn = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync();

        var handler = new EliminarUnidadOrganizativaCommandHandler(dbContext);
        var command = new EliminarUnidadOrganizativaCommand(1, 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
    }

    [Fact]
    public async Task Handle_WithActivePuestos_ReturnsConflict()
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
        dbContext.Puestos.Add(new PuestoEntity
        {
            Id = 1,
            Nombre = "Decano",
            UnidadOrganizativaId = 1,
            Activo = true,
            CreadoEn = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync();

        var handler = new EliminarUnidadOrganizativaCommandHandler(dbContext);
        var command = new EliminarUnidadOrganizativaCommand(1, 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("CONFLICT", result.ErrorCode);
    }

    [Fact]
    public async Task Handle_WithSoftDeletedPuestos_Succeeds()
    {
        // Arrange — only active Puestos should block deletion
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
        dbContext.Puestos.Add(new PuestoEntity
        {
            Id = 1,
            Nombre = "Decano Eliminado",
            UnidadOrganizativaId = 1,
            Activo = false,
            EliminadoEn = DateTime.UtcNow,
            EliminadoPor = 1,
            CreadoEn = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync();

        var handler = new EliminarUnidadOrganizativaCommandHandler(dbContext);
        var command = new EliminarUnidadOrganizativaCommand(1, 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }
}
