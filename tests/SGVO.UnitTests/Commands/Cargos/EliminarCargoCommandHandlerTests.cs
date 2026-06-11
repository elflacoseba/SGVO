using SGVO.Application.Features.Cargos.Commands;
using SGVO.Domain.Entities;
using SGVO.Infrastructure.Commands;
using SGVO.Infrastructure.Persistence.Entities;
using SGVO.UnitTests.Queries;

namespace SGVO.UnitTests.Commands.Cargos;

/// <summary>
/// Unit tests for EliminarCargoCommandHandler.
/// </summary>
public class EliminarCargoCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidSoftDelete_ReturnsSuccess()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var cargo = new Cargo("Analista", null);
        dbContext.Cargos.Add(cargo);
        await dbContext.SaveChangesAsync();

        var repository = new TestRepository<Cargo>(dbContext);
        var handler = new EliminarCargoCommandHandler(repository, dbContext);
        var command = new EliminarCargoCommand(cargo.Id, 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        var deleted = await dbContext.Cargos.FindAsync(cargo.Id);
        Assert.NotNull(deleted);
        Assert.False(deleted.Activo);
        Assert.NotNull(deleted.EliminadoEn);
    }

    [Fact]
    public async Task Handle_NotFound_ReturnsFailure()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var repository = new TestRepository<Cargo>(dbContext);
        var handler = new EliminarCargoCommandHandler(repository, dbContext);
        var command = new EliminarCargoCommand(999, 1);

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
        var cargo = new Cargo("Analista", null);
        cargo.Eliminar(1);
        dbContext.Cargos.Add(cargo);
        await dbContext.SaveChangesAsync();

        var repository = new TestRepository<Cargo>(dbContext);
        var handler = new EliminarCargoCommandHandler(repository, dbContext);
        var command = new EliminarCargoCommand(cargo.Id, 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
    }

    [Fact]
    public async Task Handle_BlockedByActivePuestos_ReturnsConflict()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var cargo = new Cargo("Analista", null);
        dbContext.Cargos.Add(cargo);
        dbContext.Puestos.Add(new PuestoEntity
        {
            Nombre = "Puesto 1",
            CargoId = cargo.Id,
            Activo = true,
            CreadoEn = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync();

        var repository = new TestRepository<Cargo>(dbContext);
        var handler = new EliminarCargoCommandHandler(repository, dbContext);
        var command = new EliminarCargoCommand(cargo.Id, 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("CONFLICT", result.ErrorCode);
    }

    [Fact]
    public async Task Handle_BlockedByActiveCargoSkills_ReturnsConflict()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var cargo = new Cargo("Analista", null);
        dbContext.Cargos.Add(cargo);
        dbContext.Skills.Add(new SkillEntity
        {
            Nombre = "Skill 1",
            Activo = true,
            CreadoEn = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync();

        var skillId = dbContext.Skills.First().Id;

        dbContext.CargoSkills.Add(new CargoSkillEntity
        {
            CargoId = cargo.Id,
            SkillId = skillId,
            NivelImportancia = 1,
            Activo = true
        });
        await dbContext.SaveChangesAsync();

        var repository = new TestRepository<Cargo>(dbContext);
        var handler = new EliminarCargoCommandHandler(repository, dbContext);
        var command = new EliminarCargoCommand(cargo.Id, 1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("CONFLICT", result.ErrorCode);
    }
}
