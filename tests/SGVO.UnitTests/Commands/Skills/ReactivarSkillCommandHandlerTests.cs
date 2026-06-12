using SGVO.Application.Features.Skills.Commands;
using SGVO.Infrastructure.Commands;
using SGVO.Infrastructure.Persistence;
using SGVO.Infrastructure.Persistence.Entities;
using SGVO.UnitTests.Queries;

namespace SGVO.UnitTests.Commands.Skills;

/// <summary>
/// Unit tests for ReactivarSkillCommandHandler.
/// </summary>
public class ReactivarSkillCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidReactivation_ReturnsSuccess()
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

        var handler = new ReactivarSkillCommandHandler(dbContext);
        var command = new ReactivarSkillCommand(entity.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value!.Activo);
        Assert.NotNull(result.Value.ModificadoEn);
    }

    [Fact]
    public async Task Handle_AlreadyActive_ReturnsSuccess_Idempotent()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var entity = new SkillEntity
        {
            Nombre = "Python",
            Categoria = "Técnica",
            Activo = true,
            CreadoEn = DateTime.UtcNow
        };
        dbContext.Skills.Add(entity);
        await dbContext.SaveChangesAsync();

        var handler = new ReactivarSkillCommandHandler(dbContext);
        var command = new ReactivarSkillCommand(entity.Id);

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
        var handler = new ReactivarSkillCommandHandler(dbContext);
        var command = new ReactivarSkillCommand(999);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
    }
}
