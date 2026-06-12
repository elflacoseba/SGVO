using SGVO.Application.Features.Skills.Commands;
using SGVO.Domain.Entities;
using SGVO.Infrastructure.Commands;
using SGVO.Infrastructure.Persistence;
using SGVO.Infrastructure.Persistence.Entities;
using SGVO.UnitTests.Queries;

namespace SGVO.UnitTests.Commands.Skills;

/// <summary>
/// Unit tests for EliminarSkillCommandHandler.
/// </summary>
public class EliminarSkillCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidSoftDelete_ReturnsSuccess()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var skill = new Skill("Python", "Técnica");
        dbContext.Skills.Add(skill);
        await dbContext.SaveChangesAsync();

        var handler = new EliminarSkillCommandHandler(dbContext);
        var command = new EliminarSkillCommand(skill.Id, 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        var deleted = await dbContext.Skills.FindAsync(skill.Id);
        Assert.NotNull(deleted);
        Assert.False(deleted!.Activo);
        Assert.NotNull(deleted.EliminadoEn);
    }

    [Fact]
    public async Task Handle_NotFound_ReturnsFailure()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var handler = new EliminarSkillCommandHandler(dbContext);
        var command = new EliminarSkillCommand(999, 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
    }

    [Fact]
    public async Task Handle_AlreadyDeleted_ReturnsFailure()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var skill = new Skill("Python", "Técnica");
        dbContext.Skills.Add(skill);
        await dbContext.SaveChangesAsync();
        skill.Eliminar(1);
        await dbContext.SaveChangesAsync();

        var handler = new EliminarSkillCommandHandler(dbContext);
        var command = new EliminarSkillCommand(skill.Id, 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
    }

    [Fact]
    public async Task Handle_BlockedByActiveCargoSkills_ReturnsConflict()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var skill = new Skill("Python", "Técnica");
        dbContext.Skills.Add(skill);
        await dbContext.SaveChangesAsync();

        dbContext.CargoSkills.Add(new CargoSkillEntity
        {
            CargoId = 1,
            SkillId = skill.Id,
            NivelImportancia = 1,
            Activo = true
        });
        await dbContext.SaveChangesAsync();

        var handler = new EliminarSkillCommandHandler(dbContext);
        var command = new EliminarSkillCommand(skill.Id, 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("CONFLICT", result.ErrorCode);
    }

    [Fact]
    public async Task Handle_BlockedByActivePersonaSkills_ReturnsConflict()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var skill = new Skill("Python", "Técnica");
        dbContext.Skills.Add(skill);
        await dbContext.SaveChangesAsync();

        dbContext.PersonaSkills.Add(new PersonaSkillEntity
        {
            PersonaId = 1,
            SkillId = skill.Id,
            NivelDominio = 3,
            Activo = true
        });
        await dbContext.SaveChangesAsync();

        var handler = new EliminarSkillCommandHandler(dbContext);
        var command = new EliminarSkillCommand(skill.Id, 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("CONFLICT", result.ErrorCode);
    }
}
