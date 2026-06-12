using SGVO.Application.Features.Skills.Commands;
using SGVO.Domain.Entities;
using SGVO.Infrastructure.Commands;
using SGVO.Infrastructure.Persistence;
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
        var skill = new Skill("Python", "Técnica");
        dbContext.Skills.Add(skill);
        await dbContext.SaveChangesAsync();
        skill.Eliminar(1);
        await dbContext.SaveChangesAsync();

        var handler = new ReactivarSkillCommandHandler(dbContext);
        var command = new ReactivarSkillCommand(skill.Id);

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
        var skill = new Skill("Python", "Técnica");
        dbContext.Skills.Add(skill);
        await dbContext.SaveChangesAsync();

        var handler = new ReactivarSkillCommandHandler(dbContext);
        var command = new ReactivarSkillCommand(skill.Id);

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
