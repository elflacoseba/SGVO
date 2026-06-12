using SGVO.Application.Features.Skills.Commands;
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
        var entity = new SkillEntity
        {
            Nombre = "Python",
            Categoria = "Técnica",
            Activo = true,
            CreadoEn = DateTime.UtcNow
        };
        dbContext.Skills.Add(entity);
        await dbContext.SaveChangesAsync();

        var handler = new EliminarSkillCommandHandler(dbContext);
        var command = new EliminarSkillCommand(entity.Id, 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        var deleted = await dbContext.Skills.FindAsync(entity.Id);
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

        var handler = new EliminarSkillCommandHandler(dbContext);
        var command = new EliminarSkillCommand(entity.Id, 1);

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
        var entity = new SkillEntity
        {
            Nombre = "Python",
            Categoria = "Técnica",
            Activo = true,
            CreadoEn = DateTime.UtcNow
        };
        dbContext.Skills.Add(entity);
        await dbContext.SaveChangesAsync();

        dbContext.CargoSkills.Add(new CargoSkillEntity
        {
            CargoId = 1,
            SkillId = entity.Id,
            NivelImportancia = 1,
            Activo = true
        });
        await dbContext.SaveChangesAsync();

        var handler = new EliminarSkillCommandHandler(dbContext);
        var command = new EliminarSkillCommand(entity.Id, 1);

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
        var entity = new SkillEntity
        {
            Nombre = "Python",
            Categoria = "Técnica",
            Activo = true,
            CreadoEn = DateTime.UtcNow
        };
        dbContext.Skills.Add(entity);
        await dbContext.SaveChangesAsync();

        dbContext.PersonaSkills.Add(new PersonaSkillEntity
        {
            PersonaId = 1,
            SkillId = entity.Id,
            NivelDominio = 3,
            Activo = true
        });
        await dbContext.SaveChangesAsync();

        var handler = new EliminarSkillCommandHandler(dbContext);
        var command = new EliminarSkillCommand(entity.Id, 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("CONFLICT", result.ErrorCode);
    }
}
