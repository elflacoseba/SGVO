using SGVO.Application.Features.Skills.Queries;
using SGVO.Infrastructure.Queries;
using SGVO.Infrastructure.Persistence.Entities;
using SGVO.UnitTests.Queries;

namespace SGVO.UnitTests.Queries;

/// <summary>
/// Unit tests for GetSkillByIdQueryHandler.
/// </summary>
public class GetSkillByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_FoundActiveSkill_ReturnsDto()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var entity = new SkillEntity
        {
            Nombre = "Python",
            Categoria = "Técnica",
            Descripcion = "Backend avanzado",
            Activo = true,
            CreadoEn = DateTime.UtcNow
        };
        dbContext.Skills.Add(entity);
        await dbContext.SaveChangesAsync();

        var handler = new GetSkillByIdQueryHandler(dbContext);
        var query = new GetSkillByIdQuery(entity.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Python", result.Value.Nombre);
        Assert.Equal("Técnica", result.Value.Categoria);
        Assert.Equal("Backend avanzado", result.Value.Descripcion);
        Assert.True(result.Value.Activo);
    }

    [Fact]
    public async Task Handle_NotFound_ReturnsNull()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var handler = new GetSkillByIdQueryHandler(dbContext);
        var query = new GetSkillByIdQuery(999);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task Handle_SoftDeleted_ReturnsNull()
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

        var handler = new GetSkillByIdQueryHandler(dbContext);
        var query = new GetSkillByIdQuery(entity.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value);
    }
}
