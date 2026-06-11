using SGVO.Application.Features.UnidadesOrganizativas.Commands;
using SGVO.Infrastructure.Commands;
using SGVO.Infrastructure.Persistence.Entities;
using SGVO.UnitTests.Queries;

namespace SGVO.UnitTests.Commands.UnidadesOrganizativas;

/// <summary>
/// Unit tests for ActualizarUnidadOrganizativaCommandHandler.
/// </summary>
public class ActualizarUnidadOrganizativaCommandHandlerTests
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
        dbContext.UnidadesOrganizativas.Add(new UnidadesOrganizativaEntity
        {
            Id = 1,
            Nombre = "Facultad de Ciencias",
            TipoUnidadOrganizativaId = 1,
            Activo = true,
            CreadoEn = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync();

        var handler = new ActualizarUnidadOrganizativaCommandHandler(dbContext);
        var command = new ActualizarUnidadOrganizativaCommand(1, "Facultad de Ingeniería", 1, 1, null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Facultad de Ingeniería", result.Value!.Nombre);
        Assert.NotNull(result.Value.ModificadoEn);
    }

    [Fact]
    public async Task Handle_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var handler = new ActualizarUnidadOrganizativaCommandHandler(dbContext);
        var command = new ActualizarUnidadOrganizativaCommand(999, "Test", 1, null, null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
    }

    [Fact]
    public async Task Handle_WithDeletedEntity_ReturnsNotFound()
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

        var handler = new ActualizarUnidadOrganizativaCommandHandler(dbContext);
        var command = new ActualizarUnidadOrganizativaCommand(1, "Test", 1, null, null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
    }

    [Fact]
    public async Task Handle_ChangingParentToSelf_ReturnsConflict()
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
            Nombre = "Unit",
            TipoUnidadOrganizativaId = 1,
            Activo = true,
            CreadoEn = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync();

        var handler = new ActualizarUnidadOrganizativaCommandHandler(dbContext);
        var command = new ActualizarUnidadOrganizativaCommand(1, "Unit", 1, null, 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("CONFLICT", result.ErrorCode);
    }

    [Fact]
    public async Task Handle_CircularHierarchy_ReturnsConflict()
    {
        // Arrange: A(1) -> B(2) -> C(3), try to set A's parent to C
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
                Nombre = "A",
                TipoUnidadOrganizativaId = 1,
                Activo = true,
                CreadoEn = DateTime.UtcNow
            },
            new UnidadesOrganizativaEntity
            {
                Id = 2,
                Nombre = "B",
                TipoUnidadOrganizativaId = 1,
                PadreId = 1,
                Activo = true,
                CreadoEn = DateTime.UtcNow
            },
            new UnidadesOrganizativaEntity
            {
                Id = 3,
                Nombre = "C",
                TipoUnidadOrganizativaId = 1,
                PadreId = 2,
                Activo = true,
                CreadoEn = DateTime.UtcNow
            });
        await dbContext.SaveChangesAsync();

        var handler = new ActualizarUnidadOrganizativaCommandHandler(dbContext);
        // Try to set A's parent to C => C->A->B->C = cycle
        var command = new ActualizarUnidadOrganizativaCommand(1, "A", 1, null, 3);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("CONFLICT", result.ErrorCode);
        Assert.Contains("ciclo", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_ValidParentChange_ReturnsSuccess()
    {
        // Arrange: A(1), B(2) root, set B's parent to A
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
                Nombre = "A",
                TipoUnidadOrganizativaId = 1,
                Activo = true,
                CreadoEn = DateTime.UtcNow
            },
            new UnidadesOrganizativaEntity
            {
                Id = 2,
                Nombre = "B",
                TipoUnidadOrganizativaId = 1,
                Activo = true,
                CreadoEn = DateTime.UtcNow
            });
        await dbContext.SaveChangesAsync();

        var handler = new ActualizarUnidadOrganizativaCommandHandler(dbContext);
        var command = new ActualizarUnidadOrganizativaCommand(2, "B", 1, null, 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.PadreId);
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
        dbContext.UnidadesOrganizativas.Add(new UnidadesOrganizativaEntity
        {
            Id = 1,
            Nombre = "Unit",
            TipoUnidadOrganizativaId = 1,
            Activo = true,
            CreadoEn = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync();

        var handler = new ActualizarUnidadOrganizativaCommandHandler(dbContext);
        var command = new ActualizarUnidadOrganizativaCommand(1, "Unit", 1, null, null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
    }
}
