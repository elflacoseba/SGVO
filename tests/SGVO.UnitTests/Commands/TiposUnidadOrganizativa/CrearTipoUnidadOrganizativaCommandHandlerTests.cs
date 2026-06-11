using SGVO.Application.Features.TiposUnidadOrganizativa.Commands;
using SGVO.Infrastructure.Commands;
using SGVO.Infrastructure.Persistence.Entities;
using SGVO.UnitTests.Queries;

namespace SGVO.UnitTests.Commands.TiposUnidadOrganizativa;

/// <summary>
/// Unit tests for CrearTipoUnidadOrganizativaCommandHandler.
/// </summary>
public class CrearTipoUnidadOrganizativaCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidNombre_ReturnsSuccess()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var handler = new CrearTipoUnidadOrganizativaCommandHandler(dbContext);
        var command = new CrearTipoUnidadOrganizativaCommand("Facultad");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Facultad", result.Value.Nombre);
        Assert.True(result.Value.Activo);
        Assert.True(result.Value.Id > 0);
    }

    [Fact]
    public async Task Handle_WithDuplicateNombre_ReturnsConflict()
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

        var handler = new CrearTipoUnidadOrganizativaCommandHandler(dbContext);
        var command = new CrearTipoUnidadOrganizativaCommand("Facultad");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("CONFLICT", result.ErrorCode);
    }

    [Fact]
    public async Task Handle_WithDuplicateNombreOfDeletedType_ReturnsSuccess()
    {
        // Arrange — deleted types should not block name reuse
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

        var handler = new CrearTipoUnidadOrganizativaCommandHandler(dbContext);
        var command = new CrearTipoUnidadOrganizativaCommand("Facultad");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Facultad", result.Value!.Nombre);
    }

    [Fact]
    public async Task Handle_SetsTimestampCorrectly()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var handler = new CrearTipoUnidadOrganizativaCommandHandler(dbContext);
        var before = DateTime.UtcNow;
        var command = new CrearTipoUnidadOrganizativaCommand("Secretaría");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value!.CreadoEn >= before);
    }
}
