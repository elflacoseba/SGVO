using SGVO.Application.Features.Skills.Commands;
using SGVO.Domain.Entities;
using SGVO.Infrastructure.Commands;
using SGVO.Infrastructure.Persistence;
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
        var skill = new Skill("Python", "Técnica");
        dbContext.Skills.Add(skill);
        await dbContext.SaveChangesAsync();

        var handler = new ActualizarSkillCommandHandler(dbContext);
        var command = new ActualizarSkillCommand(skill.Id, "Python Avanzado", "Backend", "Desc actualizada");

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
        var skill = new Skill("Python", "Técnica");
        dbContext.Skills.Add(skill);
        await dbContext.SaveChangesAsync();
        skill.Eliminar(1);
        await dbContext.SaveChangesAsync();

        var handler = new ActualizarSkillCommandHandler(dbContext);
        var command = new ActualizarSkillCommand(skill.Id, "Nuevo Nombre", null, null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
    }

    [Fact]
    public async Task Handle_DuplicateActiveName_ReturnsConflict()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        dbContext.Skills.Add(new Skill("Python", "Técnica"));
        dbContext.Skills.Add(new Skill("Java", "Técnica"));
        await dbContext.SaveChangesAsync();

        var skills = dbContext.Skills.ToList();
        var javaSkill = skills.First(s => s.Nombre == "Java");

        var handler = new ActualizarSkillCommandHandler(dbContext);
        var command = new ActualizarSkillCommand(javaSkill.Id, "Python", "Backend", null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("CONFLICT", result.ErrorCode);
    }

    [Fact]
    public async Task Handle_UpdateToSameName_ReturnsSuccess()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var skill = new Skill("Python", "Técnica");
        dbContext.Skills.Add(skill);
        await dbContext.SaveChangesAsync();

        var handler = new ActualizarSkillCommandHandler(dbContext);
        var command = new ActualizarSkillCommand(skill.Id, "Python", "Backend", "Actualizado");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Python", result.Value!.Nombre);
    }
}
