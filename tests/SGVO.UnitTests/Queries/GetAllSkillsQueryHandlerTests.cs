using SGVO.Application.Common;
using SGVO.Application.Features.Skills.Queries;
using SGVO.Domain.Entities;
using SGVO.Infrastructure.Queries;
using SGVO.UnitTests.Queries;

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
            new Skill("Skill 1", "Tech"),
            new Skill("Skill 2", "Soft")
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
        var activo = new Skill("Activo");
        var eliminado = new Skill("Eliminado");
        dbContext.Skills.AddRange(activo, eliminado);
        await dbContext.SaveChangesAsync();
        eliminado.Eliminar(1);
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
