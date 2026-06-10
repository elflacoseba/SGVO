using SGVO.Application.Features.Postulantes.Queries;
using SGVO.Infrastructure.Persistence.Entities;
using SGVO.Infrastructure.Queries;

namespace SGVO.UnitTests.Queries;

/// <summary>
/// Tests unitarios para el query handler de listado de postulantes.
/// </summary>
public class GetAllPostulantesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ConPostulantesActivos_DebeRetornarListado()
    {
        // Arrange
        var db = InMemoryDbContextFactory.Create();
        db.Postulantes.Add(new Postulante
        {
            Id = 1,
            Nombre = "Juan",
            Apellido = "Perez",
            Email = "juan@test.com",
            Origen = "Externo",
            Activo = true,
            CreadoEn = DateTime.UtcNow
        });
        db.Postulantes.Add(new Postulante
        {
            Id = 2,
            Nombre = "Maria",
            Apellido = "Gomez",
            Email = "maria@test.com",
            Origen = "Interno",
            Activo = true,
            CreadoEn = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var handler = new GetAllPostulantesQueryHandler(db);

        // Act
        var result = await handler.Handle(new GetAllPostulantesQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value!.Count);
        Assert.Contains(result.Value, p => p.Nombre == "Juan");
        Assert.Contains(result.Value, p => p.Nombre == "Maria");
    }

    [Fact]
    public async Task Handle_ConPostulantesEliminados_DebeExcluirLosEliminados()
    {
        // Arrange
        var db = InMemoryDbContextFactory.Create();
        db.Postulantes.Add(new Postulante
        {
            Id = 1,
            Nombre = "Activo",
            Apellido = "Test",
            Email = "activo@test.com",
            Origen = "Externo",
            Activo = true,
            CreadoEn = DateTime.UtcNow
        });
        db.Postulantes.Add(new Postulante
        {
            Id = 2,
            Nombre = "Eliminado",
            Apellido = "Test",
            Email = "eliminado@test.com",
            Origen = "Externo",
            Activo = true,
            CreadoEn = DateTime.UtcNow,
            EliminadoEn = DateTime.UtcNow,
            EliminadoPor = 1
        });
        await db.SaveChangesAsync();

        var handler = new GetAllPostulantesQueryHandler(db);

        // Act
        var result = await handler.Handle(new GetAllPostulantesQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value!);
        Assert.Equal("Activo", result.Value![0].Nombre);
    }

    [Fact]
    public async Task Handle_SinPostulantes_DebeRetornarListaVacia()
    {
        // Arrange
        var db = InMemoryDbContextFactory.Create();
        var handler = new GetAllPostulantesQueryHandler(db);

        // Act
        var result = await handler.Handle(new GetAllPostulantesQuery(), CancellationToken.None);

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
        db.Postulantes.Add(new Postulante
        {
            Id = 1,
            Nombre = "Pedro",
            Apellido = "Lopez",
            Email = "pedro@test.com",
            Origen = "Recomendado",
            Activo = true,
            CreadoEn = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var handler = new GetAllPostulantesQueryHandler(db);

        // Act
        var result = await handler.Handle(new GetAllPostulantesQuery(), CancellationToken.None);

        // Assert
        Assert.NotNull(result.Value);
        var dto = result.Value[0];
        Assert.Equal(1, dto.Id);
        Assert.Equal("Pedro", dto.Nombre);
        Assert.Equal("Lopez", dto.Apellido);
        Assert.Equal("pedro@test.com", dto.Email);
        Assert.Equal("Recomendado", dto.Origen);
    }
}
