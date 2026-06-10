using SGVO.Application.Features.Cargos.Queries;
using SGVO.Infrastructure.Persistence.Entities;
using SGVO.Infrastructure.Queries;
using SGVO.Shared;

namespace SGVO.UnitTests.Queries;

/// <summary>
/// Tests unitarios para el query handler de listado de cargos.
/// </summary>
public class GetAllCargosQueryHandlerTests
{
    [Fact]
    public async Task Handle_ConCargosActivos_DebeRetornarListado()
    {
        // Arrange
        var db = InMemoryDbContextFactory.Create();
        db.Cargos.Add(new Cargo
        {
            Id = 1,
            Nombre = "Desarrollador",
            Descripcion = "Desarrollo de software",
            Activo = true,
            CreadoEn = DateTime.UtcNow
        });
        db.Cargos.Add(new Cargo
        {
            Id = 2,
            Nombre = "Analista",
            Descripcion = "Analisis de requisitos",
            Activo = true,
            CreadoEn = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var handler = new GetAllCargosQueryHandler(db);

        // Act
        var result = await handler.Handle(new GetAllCargosQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value!.Count);
        Assert.Contains(result.Value, c => c.Nombre == "Desarrollador");
        Assert.Contains(result.Value, c => c.Nombre == "Analista");
    }

    [Fact]
    public async Task Handle_ConCargosEliminados_DebeExcluirLosEliminados()
    {
        // Arrange
        var db = InMemoryDbContextFactory.Create();
        db.Cargos.Add(new Cargo
        {
            Id = 1,
            Nombre = "Activo",
            Activo = true,
            CreadoEn = DateTime.UtcNow
        });
        db.Cargos.Add(new Cargo
        {
            Id = 2,
            Nombre = "Eliminado",
            Activo = true,
            CreadoEn = DateTime.UtcNow,
            EliminadoEn = DateTime.UtcNow,
            EliminadoPor = 1
        });
        await db.SaveChangesAsync();

        var handler = new GetAllCargosQueryHandler(db);

        // Act
        var result = await handler.Handle(new GetAllCargosQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value!);
        Assert.Equal("Activo", result.Value[0].Nombre);
    }

    [Fact]
    public async Task Handle_SinCargos_DebeRetornarListaVacia()
    {
        // Arrange
        var db = InMemoryDbContextFactory.Create();
        var handler = new GetAllCargosQueryHandler(db);

        // Act
        var result = await handler.Handle(new GetAllCargosQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value!);
    }

    [Fact]
    public async Task Handle_DebeMapearCorrectamenteADto()
    {
        // Arrange
        var db = InMemoryDbContextFactory.Create();
        db.Cargos.Add(new Cargo
        {
            Id = 1,
            Nombre = "Tester",
            Descripcion = "QA automation",
            Activo = true,
            CreadoEn = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var handler = new GetAllCargosQueryHandler(db);

        // Act
        var result = await handler.Handle(new GetAllCargosQuery(), CancellationToken.None);

        // Assert
        Assert.NotNull(result.Value);
        var dto = result.Value[0];
        Assert.Equal(1, dto.Id);
        Assert.Equal("Tester", dto.Nombre);
        Assert.Equal("QA automation", dto.Descripcion);
    }
}
