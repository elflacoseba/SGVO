using SGVO.Application.Common;
using SGVO.Application.Features.Postulantes.Queries;
using SGVO.Infrastructure.Persistence.Entities;
using SGVO.Infrastructure.Queries;

namespace SGVO.UnitTests.Queries;

/// <summary>
/// Tests unitarios para GetAllPostulantesQueryHandler.
/// </summary>
public class GetAllPostulantesQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithActivePostulantes_ReturnsPagedResult()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        dbContext.Postulantes.AddRange(
            new PostulanteEntity { Id = 1, Nombre = "Juan", Apellido = "Perez", Email = "juan@test.com", Origen = "Externo", Activo = true, CreadoEn = DateTime.UtcNow },
            new PostulanteEntity { Id = 2, Nombre = "Maria", Apellido = "Gomez", Email = "maria@test.com", Origen = "Interno", Activo = true, CreadoEn = DateTime.UtcNow }
        );
        await dbContext.SaveChangesAsync();

        var handler = new GetAllPostulantesQueryHandler(dbContext);
        var query = new GetAllPostulantesQuery(new PageParameters(1, 10));

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.TotalCount);
        Assert.Equal(2, result.Value.Items.Count);
    }

    [Fact]
    public async Task Handle_WithSoftDeletedPostulantes_ExcludesThem()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        dbContext.Postulantes.AddRange(
            new PostulanteEntity { Id = 1, Nombre = "Activo", Apellido = "Test", Email = "activo@test.com", Origen = "Externo", Activo = true, CreadoEn = DateTime.UtcNow },
            new PostulanteEntity { Id = 2, Nombre = "Eliminado", Apellido = "Test", Email = "eliminado@test.com", Origen = "Externo", Activo = false, EliminadoEn = DateTime.UtcNow, EliminadoPor = 1, CreadoEn = DateTime.UtcNow }
        );
        await dbContext.SaveChangesAsync();

        var handler = new GetAllPostulantesQueryHandler(dbContext);
        var query = new GetAllPostulantesQuery(new PageParameters(1, 10));

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
