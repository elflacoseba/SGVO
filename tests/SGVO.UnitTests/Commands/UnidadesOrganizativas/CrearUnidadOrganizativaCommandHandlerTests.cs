using SGVO.Application.Features.UnidadesOrganizativas.Commands;
using SGVO.Infrastructure.Commands;
using SGVO.Infrastructure.Persistence.Entities;
using SGVO.UnitTests.Queries;

namespace SGVO.UnitTests.Commands.UnidadesOrganizativas;

/// <summary>
/// Unit tests for CrearUnidadOrganizativaCommandHandler.
/// </summary>
public class CrearUnidadOrganizativaCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ReturnsSuccess()
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

        var handler = new CrearUnidadOrganizativaCommandHandler(dbContext);
        var command = new CrearUnidadOrganizativaCommand("Facultad de Ciencias", 1, 1, null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Facultad de Ciencias", result.Value.Nombre);
        Assert.Equal(1, result.Value.TipoUnidadOrganizativaId);
        Assert.Equal("Facultad", result.Value.TipoNombre);
        Assert.True(result.Value.Activo);
        Assert.True(result.Value.Id > 0);
    }

    [Fact]
    public async Task Handle_WithParent_ReturnsSuccess()
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

        var handler = new CrearUnidadOrganizativaCommandHandler(dbContext);
        var command = new CrearUnidadOrganizativaCommand("Departamento de Matemática", 1, 2, 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.PadreId);
    }

    [Fact]
    public async Task Handle_WithNonExistentTipo_ReturnsNotFound()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var handler = new CrearUnidadOrganizativaCommandHandler(dbContext);
        var command = new CrearUnidadOrganizativaCommand("Test", 999, null, null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
    }

    [Fact]
    public async Task Handle_WithDeletedTipo_ReturnsNotFound()
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

        var handler = new CrearUnidadOrganizativaCommandHandler(dbContext);
        var command = new CrearUnidadOrganizativaCommand("Test", 1, null, null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
    }

    [Fact]
    public async Task Handle_WithNonExistentParent_ReturnsNotFound()
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

        var handler = new CrearUnidadOrganizativaCommandHandler(dbContext);
        var command = new CrearUnidadOrganizativaCommand("Test", 1, null, 999);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
    }

    [Fact]
    public async Task Handle_WithDeletedParent_ReturnsValidation()
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
            Nombre = "Deleted Parent",
            TipoUnidadOrganizativaId = 1,
            Activo = false,
            EliminadoEn = DateTime.UtcNow,
            EliminadoPor = 1,
            CreadoEn = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync();

        var handler = new CrearUnidadOrganizativaCommandHandler(dbContext);
        var command = new CrearUnidadOrganizativaCommand("Test", 1, null, 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("VALIDATION", result.ErrorCode);
    }

    [Fact]
    public async Task Handle_SetsTimestampCorrectly()
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

        var handler = new CrearUnidadOrganizativaCommandHandler(dbContext);
        var before = DateTime.UtcNow;
        var command = new CrearUnidadOrganizativaCommand("Test", 1, null, null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value!.CreadoEn >= before);
    }
}
