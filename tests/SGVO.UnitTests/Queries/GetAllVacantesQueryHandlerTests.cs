using SGVO.Application.Common;
using SGVO.Application.Features.Vacantes.Queries;
using SGVO.Infrastructure.Persistence.Entities;
using SGVO.Infrastructure.Queries;

namespace SGVO.UnitTests.Queries;

/// <summary>
/// Tests unitarios para GetAllVacantesQueryHandler y GetVacanteByIdQueryHandler.
/// </summary>
public class GetAllVacantesQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithActiveVacantes_ReturnsPagedResult()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        dbContext.Vacantes.AddRange(
            new VacanteEntity { Id = 1, PuestoId = 1, Motivo = "Vacante 1", Estado = "Abierta", Activo = true, FechaApertura = DateTime.UtcNow, CreadoEn = DateTime.UtcNow },
            new VacanteEntity { Id = 2, PuestoId = 2, Motivo = "Vacante 2", Estado = "Abierta", Activo = true, FechaApertura = DateTime.UtcNow, CreadoEn = DateTime.UtcNow }
        );
        await dbContext.SaveChangesAsync();

        var handler = new GetAllVacantesQueryHandler(dbContext);
        var query = new GetAllVacantesQuery(new PageParameters(1, 10));

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.TotalCount);
        Assert.Equal(2, result.Value.Items.Count);
    }

    [Fact]
    public async Task Handle_WithSoftDeletedVacantes_ExcludesThem()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        dbContext.Vacantes.AddRange(
            new VacanteEntity { Id = 1, PuestoId = 1, Motivo = "Activa", Estado = "Abierta", Activo = true, FechaApertura = DateTime.UtcNow, CreadoEn = DateTime.UtcNow },
            new VacanteEntity { Id = 2, PuestoId = 2, Motivo = "Eliminada", Estado = "Abierta", Activo = false, EliminadoEn = DateTime.UtcNow, EliminadoPor = 1, FechaApertura = DateTime.UtcNow, CreadoEn = DateTime.UtcNow }
        );
        await dbContext.SaveChangesAsync();

        var handler = new GetAllVacantesQueryHandler(dbContext);
        var query = new GetAllVacantesQuery(new PageParameters(1, 10));

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(1, result.Value.TotalCount);
        Assert.Single(result.Value.Items);
        Assert.Equal("Activa", result.Value.Items[0].Motivo);
    }
}

public class GetVacanteByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingVacante_ReturnsDto()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        dbContext.Vacantes.Add(new VacanteEntity
        {
            Id = 1,
            PuestoId = 10,
            Motivo = "Vacante de prueba",
            Estado = "Abierta",
            Activo = true,
            FechaApertura = DateTime.UtcNow,
            CreadoEn = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync();

        var handler = new GetVacanteByIdQueryHandler(dbContext);
        var query = new GetVacanteByIdQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(1, result.Value.Id);
        Assert.Equal("Vacante de prueba", result.Value.Motivo);
    }

    [Fact]
    public async Task Handle_WithNonExistingVacante_ReturnsNull()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        var handler = new GetVacanteByIdQueryHandler(dbContext);
        var query = new GetVacanteByIdQuery(999);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task Handle_WithSoftDeletedVacante_ReturnsNull()
    {
        // Arrange
        var dbContext = InMemoryDbContextFactory.Create();
        dbContext.Vacantes.Add(new VacanteEntity
        {
            Id = 1,
            PuestoId = 10,
            Motivo = "Eliminada",
            Estado = "Abierta",
            Activo = false,
            EliminadoEn = DateTime.UtcNow,
            EliminadoPor = 1,
            FechaApertura = DateTime.UtcNow,
            CreadoEn = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync();

        var handler = new GetVacanteByIdQueryHandler(dbContext);
        var query = new GetVacanteByIdQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value);
    }
}
