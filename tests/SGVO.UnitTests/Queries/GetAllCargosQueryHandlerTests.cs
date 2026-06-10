using SGVO.Application.Common;
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
        var result = await handler.Handle(new GetAllCargosQuery(new PageParameters()), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value!.Items.Count);
        Assert.Contains(result.Value.Items, c => c.Nombre == "Desarrollador");
        Assert.Contains(result.Value.Items, c => c.Nombre == "Analista");
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
        var result = await handler.Handle(new GetAllCargosQuery(new PageParameters()), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value!.Items);
        Assert.Equal("Activo", result.Value.Items[0].Nombre);
    }

    [Fact]
    public async Task Handle_SinCargos_DebeRetornarListaVacia()
    {
        // Arrange
        var db = InMemoryDbContextFactory.Create();
        var handler = new GetAllCargosQueryHandler(db);

        // Act
        var result = await handler.Handle(new GetAllCargosQuery(new PageParameters()), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value!.Items);
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
        var result = await handler.Handle(new GetAllCargosQuery(new PageParameters()), CancellationToken.None);

        // Assert
        Assert.NotNull(result.Value);
        var dto = result.Value!.Items[0];
        Assert.Equal(1, dto.Id);
        Assert.Equal("Tester", dto.Nombre);
        Assert.Equal("QA automation", dto.Descripcion);
    }

    [Fact]
    public async Task Handle_ConPaginacion_DebeRetornarSoloElementosDeLaPagina()
    {
        // Arrange
        var db = InMemoryDbContextFactory.Create();
        for (int i = 1; i <= 25; i++)
        {
            db.Cargos.Add(new Cargo
            {
                Id = (ulong)i,
                Nombre = $"Cargo {i}",
                Activo = true,
                CreadoEn = DateTime.UtcNow
            });
        }
        await db.SaveChangesAsync();

        var handler = new GetAllCargosQueryHandler(db);

        // Act - Page 1, PageSize 10
        var result = await handler.Handle(new GetAllCargosQuery(new PageParameters(1, 10)), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(10, result.Value!.Items.Count);
        Assert.Equal(25, result.Value.TotalCount);
        Assert.Equal(1, result.Value.Page);
        Assert.Equal(10, result.Value.PageSize);
        Assert.Equal(3, result.Value.TotalPages);
        Assert.True(result.Value.HasNextPage);
        Assert.False(result.Value.HasPreviousPage);
    }

    [Fact]
    public async Task Handle_ConPaginacionSegundaPagina_DebeRetornarElementosCorrectos()
    {
        // Arrange
        var db = InMemoryDbContextFactory.Create();
        for (int i = 1; i <= 25; i++)
        {
            db.Cargos.Add(new Cargo
            {
                Id = (ulong)i,
                Nombre = $"Cargo {i}",
                Activo = true,
                CreadoEn = DateTime.UtcNow
            });
        }
        await db.SaveChangesAsync();

        var handler = new GetAllCargosQueryHandler(db);

        // Act - Page 2, PageSize 10
        var result = await handler.Handle(new GetAllCargosQuery(new PageParameters(2, 10)), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(10, result.Value!.Items.Count);
        Assert.Equal(25, result.Value.TotalCount);
        Assert.Equal(2, result.Value.Page);
        Assert.True(result.Value.HasNextPage);
        Assert.True(result.Value.HasPreviousPage);
    }

    [Fact]
    public async Task Handle_ConPageSizeInvalido_DebeNormalizarAMaxPageSize()
    {
        // Arrange
        var db = InMemoryDbContextFactory.Create();
        for (int i = 1; i <= 150; i++)
        {
            db.Cargos.Add(new Cargo
            {
                Id = (ulong)i,
                Nombre = $"Cargo {i}",
                Activo = true,
                CreadoEn = DateTime.UtcNow
            });
        }
        await db.SaveChangesAsync();

        var handler = new GetAllCargosQueryHandler(db);

        // Act - PageSize 200 (exceeds max of 100)
        var result = await handler.Handle(new GetAllCargosQuery(new PageParameters(1, 200)), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(100, result.Value!.PageSize); // Normalized to max
        Assert.Equal(100, result.Value.Items.Count);
    }
}
