using SGVO.Application.Common;
using SGVO.Application.Features.Skills.Queries;
using SGVO.Infrastructure.Persistence.Entities;
using SGVO.Infrastructure.Queries;

namespace SGVO.UnitTests.Queries;

/// <summary>
/// Tests unitarios para GetAllSkillsQueryHandler.
/// </summary>
public class GetAllSkillsQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithActiveSkills_ReturnsPagedResult()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        dbContext.Skills.AddRange(
            new SkillEntity { Id = 1, Nombre = "Skill 1", Categoria = "Tech", Activo = true, CreadoEn = DateTime.UtcNow },
            new SkillEntity { Id = 2, Nombre = "Skill 2", Categoria = "Soft", Activo = true, CreadoEn = DateTime.UtcNow }
        );
        await dbContext.SaveChangesAsync();

        var handler = new GetAllSkillsQueryHandler(dbContext);
        var query = new GetAllSkillsQuery(new PageParameters(1, 10));

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.TotalCount);
        Assert.Equal(2, result.Value.Items.Count);
    }

    [Fact]
    public async Task Handle_WithSoftDeletedSkills_ExcludesThem()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        dbContext.Skills.AddRange(
            new SkillEntity { Id = 1, Nombre = "Activo", Activo = true, CreadoEn = DateTime.UtcNow },
            new SkillEntity { Id = 2, Nombre = "Eliminado", Activo = false, EliminadoEn = DateTime.UtcNow, EliminadoPor = 1, CreadoEn = DateTime.UtcNow }
        );
        await dbContext.SaveChangesAsync();

        var handler = new GetAllSkillsQueryHandler(dbContext);
        var query = new GetAllSkillsQuery(new PageParameters(1, 10));

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(1, result.Value.TotalCount);
        Assert.Single(result.Value.Items);
        Assert.Equal("Activo", result.Value.Items[0].Nombre);
    }
}
