using SGVO.Application.Common;
using SGVO.Application.Features.UnidadesOrganizativas.Queries;
using SGVO.Infrastructure.Persistence.Entities;
using SGVO.Infrastructure.Queries;

namespace SGVO.UnitTests.Queries;

/// <summary>
/// Unit tests for GetAllUnidadesOrganizativasQueryHandler.
/// </summary>
public class GetAllUnidadesOrganizativasQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithActiveUnits_ReturnsPagedResult()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        dbContext.TiposUnidadOrganizativa.AddRange(
            new TipoUnidadOrganizativaEntity { Id = 1, Nombre = "Facultad", Activo = true, CreadoEn = DateTime.UtcNow },
            new TipoUnidadOrganizativaEntity { Id = 2, Nombre = "Secretaría", Activo = true, CreadoEn = DateTime.UtcNow }
        );
        dbContext.UnidadesOrganizativas.AddRange(
            new UnidadesOrganizativaEntity { Id = 1, Nombre = "Facultad de Cs", TipoUnidadOrganizativaId = 1, NivelJerarquico = 1, Activo = true, CreadoEn = DateTime.UtcNow },
            new UnidadesOrganizativaEntity { Id = 2, Nombre = "Secretaría Académica", TipoUnidadOrganizativaId = 2, NivelJerarquico = 1, Activo = true, CreadoEn = DateTime.UtcNow }
        );
        await dbContext.SaveChangesAsync();

        var handler = new GetAllUnidadesOrganizativasQueryHandler(dbContext);
        var query = new GetAllUnidadesOrganizativasQuery(new PageParameters(1, 10));

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.TotalCount);
        Assert.Equal(2, result.Value.Items.Count);
    }

    [Fact]
    public async Task Handle_WithSoftDeletedUnits_ExcludesThem()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        dbContext.TiposUnidadOrganizativa.Add(
            new TipoUnidadOrganizativaEntity { Id = 1, Nombre = "Facultad", Activo = true, CreadoEn = DateTime.UtcNow }
        );
        dbContext.UnidadesOrganizativas.AddRange(
            new UnidadesOrganizativaEntity { Id = 1, Nombre = "Activa", TipoUnidadOrganizativaId = 1, Activo = true, CreadoEn = DateTime.UtcNow },
            new UnidadesOrganizativaEntity { Id = 2, Nombre = "Eliminada", TipoUnidadOrganizativaId = 1, Activo = false, EliminadoEn = DateTime.UtcNow, EliminadoPor = 1, CreadoEn = DateTime.UtcNow }
        );
        await dbContext.SaveChangesAsync();

        var handler = new GetAllUnidadesOrganizativasQueryHandler(dbContext);
        var query = new GetAllUnidadesOrganizativasQuery(new PageParameters(1, 10));

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(1, result.Value.TotalCount);
        Assert.Single(result.Value.Items);
        Assert.Equal("Activa", result.Value.Items[0].Nombre);
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        dbContext.TiposUnidadOrganizativa.Add(
            new TipoUnidadOrganizativaEntity { Id = 1, Nombre = "Departamento", Activo = true, CreadoEn = DateTime.UtcNow }
        );
        for (int i = 1; i <= 25; i++)
        {
            dbContext.UnidadesOrganizativas.Add(new UnidadesOrganizativaEntity
            {
                Id = i,
                Nombre = $"Unidad {i}",
                TipoUnidadOrganizativaId = 1,
                Activo = true,
                CreadoEn = DateTime.UtcNow
            });
        }
        await dbContext.SaveChangesAsync();

        var handler = new GetAllUnidadesOrganizativasQueryHandler(dbContext);
        var query = new GetAllUnidadesOrganizativasQuery(new PageParameters(2, 10));

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(25, result.Value.TotalCount);
        Assert.Equal(3, result.Value.TotalPages);
        Assert.Equal(10, result.Value.Items.Count);
        Assert.Equal(2, result.Value.Page);
    }

    [Fact]
    public async Task Handle_WithTipoFilter_ReturnsOnlyMatchingUnits()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        dbContext.TiposUnidadOrganizativa.AddRange(
            new TipoUnidadOrganizativaEntity { Id = 1, Nombre = "Facultad", Activo = true, CreadoEn = DateTime.UtcNow },
            new TipoUnidadOrganizativaEntity { Id = 2, Nombre = "Secretaría", Activo = true, CreadoEn = DateTime.UtcNow }
        );
        dbContext.UnidadesOrganizativas.AddRange(
            new UnidadesOrganizativaEntity { Id = 1, Nombre = "Facultad de Cs", TipoUnidadOrganizativaId = 1, NivelJerarquico = 1, Activo = true, CreadoEn = DateTime.UtcNow },
            new UnidadesOrganizativaEntity { Id = 2, Nombre = "Secretaría Académica", TipoUnidadOrganizativaId = 2, NivelJerarquico = 1, Activo = true, CreadoEn = DateTime.UtcNow },
            new UnidadesOrganizativaEntity { Id = 3, Nombre = "Facultad de Ingeniería", TipoUnidadOrganizativaId = 1, NivelJerarquico = 1, Activo = true, CreadoEn = DateTime.UtcNow }
        );
        await dbContext.SaveChangesAsync();

        var handler = new GetAllUnidadesOrganizativasQueryHandler(dbContext);
        var query = new GetAllUnidadesOrganizativasQuery(new PageParameters(1, 10), TipoUnidadOrganizativaId: 1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.TotalCount);
        Assert.All(result.Value.Items, item => Assert.Equal(1, item.TipoUnidadOrganizativaId));
    }
}
