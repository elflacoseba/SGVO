using SGVO.Application.Features.Skills.Commands;
using SGVO.Infrastructure.Commands;
using SGVO.Infrastructure.Persistence;
using SGVO.Infrastructure.Persistence.Entities;
using SGVO.UnitTests.Queries;

namespace SGVO.UnitTests.Commands.Skills;

/// <summary>
/// Unit tests for ActualizarSkillCommandHandler.
/// </summary>
public class ActualizarSkillCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var entity = new SkillEntity
        {
            Nombre = "Python",
            Categoria = "Técnica",
            Descripcion = null,
            Activo = true,
            CreadoEn = DateTime.UtcNow
        };
        dbContext.Skills.Add(entity);
        await dbContext.SaveChangesAsync();

        var handler = new ActualizarSkillCommandHandler(dbContext);
        var command = new ActualizarSkillCommand(entity.Id, "Python Avanzado", "Backend", "Desc actualizada");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Python Avanzado", result.Value!.Nombre);
        Assert.Equal("Backend", result.Value.Categoria);
        Assert.Equal("Desc actualizada", result.Value.Descripcion);
    }

    [Fact]
    public async Task Handle_NotFound_ReturnsFailure()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var handler = new ActualizarSkillCommandHandler(dbContext);
        var command = new ActualizarSkillCommand(999, "Whatever", null, null);

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
        var entity = new SkillEntity
        {
            Nombre = "Python",
            Categoria = "Técnica",
            Activo = false,
            CreadoEn = DateTime.UtcNow,
            EliminadoEn = DateTime.UtcNow,
            EliminadoPor = 1
        };
        dbContext.Skills.Add(entity);
        await dbContext.SaveChangesAsync();

        var handler = new ActualizarSkillCommandHandler(dbContext);
        var command = new ActualizarSkillCommand(entity.Id, "Nuevo Nombre", null, null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
    }
}
