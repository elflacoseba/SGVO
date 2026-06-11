using SGVO.Application.Features.UnidadesOrganizativas.Queries;
using SGVO.Infrastructure.Persistence.Entities;
using SGVO.Infrastructure.Queries;

namespace SGVO.UnitTests.Queries;

/// <summary>
/// Unit tests for GetUnidadOrganizativaByIdQueryHandler.
/// </summary>
public class GetUnidadOrganizativaByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingUnit_ReturnsDto()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        dbContext.TiposUnidadOrganizativa.Add(
            new TipoUnidadOrganizativaEntity { Id = 1, Nombre = "Facultad", Activo = true, CreadoEn = DateTime.UtcNow }
        );
        dbContext.UnidadesOrganizativas.Add(new UnidadesOrganizativaEntity
        {
            Id = 1,
            Nombre = "Facultad de Ciencias",
            TipoUnidadOrganizativaId = 1,
            NivelJerarquico = 1,
            Activo = true,
            CreadoEn = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync();

        var handler = new GetUnidadOrganizativaByIdQueryHandler(dbContext);
        var query = new GetUnidadOrganizativaByIdQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(1, result.Value.Id);
        Assert.Equal("Facultad de Ciencias", result.Value.Nombre);
        Assert.Equal("Facultad", result.Value.TipoNombre);
    }

    [Fact]
    public async Task Handle_WithNonExistingUnit_ReturnsNull()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var handler = new GetUnidadOrganizativaByIdQueryHandler(dbContext);
        var query = new GetUnidadOrganizativaByIdQuery(999);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task Handle_WithSoftDeletedUnit_ReturnsNull()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        dbContext.TiposUnidadOrganizativa.Add(
            new TipoUnidadOrganizativaEntity { Id = 1, Nombre = "Facultad", Activo = true, CreadoEn = DateTime.UtcNow }
        );
        dbContext.UnidadesOrganizativas.Add(new UnidadesOrganizativaEntity
        {
            Id = 1,
            Nombre = "Eliminada",
            TipoUnidadOrganizativaId = 1,
            Activo = false,
            EliminadoEn = DateTime.UtcNow,
            EliminadoPor = 1,
            CreadoEn = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync();

        var handler = new GetUnidadOrganizativaByIdQueryHandler(dbContext);
        var query = new GetUnidadOrganizativaByIdQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value);
    }
}
