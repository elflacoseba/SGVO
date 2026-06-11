using SGVO.Application.Features.UnidadesOrganizativas.Commands;
using SGVO.Infrastructure.Commands;
using SGVO.Infrastructure.Persistence.Entities;
using SGVO.UnitTests.Queries;

namespace SGVO.UnitTests.Commands.UnidadesOrganizativas;

/// <summary>
/// Unit tests for ReactivarUnidadOrganizativaCommandHandler.
/// </summary>
public class ReactivarUnidadOrganizativaCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidDeletedEntity_ReturnsSuccess()
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
            Activo = false,
            EliminadoEn = DateTime.UtcNow,
            EliminadoPor = 1,
            CreadoEn = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync();

        var handler = new ReactivarUnidadOrganizativaCommandHandler(dbContext);
        var command = new ReactivarUnidadOrganizativaCommand(1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value!.Activo);
        Assert.Equal("Facultad de Ciencias", result.Value.Nombre);
    }

    [Fact]
    public async Task Handle_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var handler = new ReactivarUnidadOrganizativaCommandHandler(dbContext);
        var command = new ReactivarUnidadOrganizativaCommand(999);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
    }

    [Fact]
    public async Task Handle_WithActiveEntity_ReturnsConflict()
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
            Nombre = "Active Unit",
            TipoUnidadOrganizativaId = 1,
            Activo = true,
            CreadoEn = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync();

        var handler = new ReactivarUnidadOrganizativaCommandHandler(dbContext);
        var command = new ReactivarUnidadOrganizativaCommand(1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("CONFLICT", result.ErrorCode);
    }

    [Fact]
    public async Task Handle_WithDeletedTipo_ReturnsValidation()
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
        dbContext.UnidadesOrganizativas.Add(new UnidadesOrganizativaEntity
        {
            Id = 1,
            Nombre = "Unit",
            TipoUnidadOrganizativaId = 1,
            Activo = false,
            EliminadoEn = DateTime.UtcNow,
            EliminadoPor = 1,
            CreadoEn = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync();

        var handler = new ReactivarUnidadOrganizativaCommandHandler(dbContext);
        var command = new ReactivarUnidadOrganizativaCommand(1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("VALIDATION", result.ErrorCode);
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
        dbContext.UnidadesOrganizativas.AddRange(
            new UnidadesOrganizativaEntity
            {
                Id = 1,
                Nombre = "Parent",
                TipoUnidadOrganizativaId = 1,
                Activo = false,
                EliminadoEn = DateTime.UtcNow,
                EliminadoPor = 1,
                CreadoEn = DateTime.UtcNow
            },
            new UnidadesOrganizativaEntity
            {
                Id = 2,
                Nombre = "Child",
                TipoUnidadOrganizativaId = 1,
                PadreId = 1,
                Activo = false,
                EliminadoEn = DateTime.UtcNow,
                EliminadoPor = 1,
                CreadoEn = DateTime.UtcNow
            });
        await dbContext.SaveChangesAsync();

        var handler = new ReactivarUnidadOrganizativaCommandHandler(dbContext);
        var command = new ReactivarUnidadOrganizativaCommand(2);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("VALIDATION", result.ErrorCode);
    }

    [Fact]
    public async Task Handle_WithActiveParent_ReturnsSuccess()
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
        dbContext.UnidadesOrganizativas.AddRange(
            new UnidadesOrganizativaEntity
            {
                Id = 1,
                Nombre = "Parent",
                TipoUnidadOrganizativaId = 1,
                Activo = true,
                CreadoEn = DateTime.UtcNow
            },
            new UnidadesOrganizativaEntity
            {
                Id = 2,
                Nombre = "Child",
                TipoUnidadOrganizativaId = 1,
                PadreId = 1,
                Activo = false,
                EliminadoEn = DateTime.UtcNow,
                EliminadoPor = 1,
                CreadoEn = DateTime.UtcNow
            });
        await dbContext.SaveChangesAsync();

        var handler = new ReactivarUnidadOrganizativaCommandHandler(dbContext);
        var command = new ReactivarUnidadOrganizativaCommand(2);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value!.Activo);
        Assert.Equal(1, result.Value.PadreId);
    }
}
