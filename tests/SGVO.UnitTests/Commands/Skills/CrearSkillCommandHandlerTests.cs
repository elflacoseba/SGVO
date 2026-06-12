using SGVO.Application.Features.Skills.Commands;
using SGVO.Domain.Entities;
using SGVO.Infrastructure.Commands;
using SGVO.Infrastructure.Persistence;
using SGVO.Infrastructure.Persistence.Entities;
using SGVO.UnitTests.Queries;

namespace SGVO.UnitTests.Commands.Skills;

/// <summary>
/// Unit tests for CrearSkillCommandHandler.
/// </summary>
public class CrearSkillCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var handler = new CrearSkillCommandHandler(dbContext);
        var command = new CrearSkillCommand("Python", "Técnica", "Backend");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Python", result.Value.Nombre);
        Assert.Equal("Técnica", result.Value.Categoria);
        Assert.Equal("Backend", result.Value.Descripcion);
        Assert.True(result.Value.Activo);
        Assert.True(result.Value.Id > 0);
    }

    [Fact]
    public async Task Handle_WithNullOptionalFields_ReturnsSuccess()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var handler = new CrearSkillCommandHandler(dbContext);
        var command = new CrearSkillCommand("JavaScript", null, null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value!.Categoria);
        Assert.Null(result.Value.Descripcion);
    }

    [Fact]
    public async Task Handle_DuplicateActiveName_ReturnsConflict()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        dbContext.Skills.Add(new Skill("Python", "Técnica"));
        await dbContext.SaveChangesAsync();

        var handler = new CrearSkillCommandHandler(dbContext);
        var command = new CrearSkillCommand("Python", null, null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("CONFLICT", result.ErrorCode);
    }

    [Fact]
    public async Task Handle_WithTrimmedWhitespace_DetectsDuplicate()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        dbContext.Skills.Add(new Skill("Python", "Técnica"));
        await dbContext.SaveChangesAsync();

        var handler = new CrearSkillCommandHandler(dbContext);
        var command = new CrearSkillCommand("  Python  ", null, null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("CONFLICT", result.ErrorCode);
    }

    [Fact]
    public async Task Handle_DuplicateSoftDeletedName_AllowsReuse()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var skill = new Skill("Python", "Técnica");
        dbContext.Skills.Add(skill);
        await dbContext.SaveChangesAsync();

        // Soft delete it
        skill.Eliminar(1);
        await dbContext.SaveChangesAsync();

        var handler = new CrearSkillCommandHandler(dbContext);
        var command = new CrearSkillCommand("Python", null, null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Python", result.Value!.Nombre);
    }
}
