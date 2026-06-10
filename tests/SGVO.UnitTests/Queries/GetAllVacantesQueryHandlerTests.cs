using SGVO.Application.Features.Vacantes.Queries;
using SGVO.Infrastructure.Persistence.Entities;
using SGVO.Infrastructure.Queries;

namespace SGVO.UnitTests.Queries;

/// <summary>
/// Tests unitarios para el query handler de listado de vacantes.
/// </summary>
public class GetAllVacantesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ConVacantesActivas_DebeRetornarListado()
    {
        // Arrange
        var db = InMemoryDbContextFactory.Create();
        db.Vacantes.Add(new Vacante
        {
            Id = 1,
            PuestoId = 10,
            FechaApertura = DateTime.UtcNow,
            Motivo = "Nueva posicion",
            Estado = "Abierta",
            Activo = true,
            CreadoEn = DateTime.UtcNow
        });
        db.Vacantes.Add(new Vacante
        {
            Id = 2,
            PuestoId = 20,
            FechaApertura = DateTime.UtcNow,
            Motivo = "Reemplazo",
            Estado = "Cubierta",
            Activo = true,
            CreadoEn = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var handler = new GetAllVacantesQueryHandler(db);

        // Act
        var result = await handler.Handle(new GetAllVacantesQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value!.Count);
        Assert.Contains(result.Value, v => v.Motivo == "Nueva posicion");
        Assert.Contains(result.Value, v => v.Motivo == "Reemplazo");
    }

    [Fact]
    public async Task Handle_ConVacantesEliminadas_DebeExcluirLasEliminadas()
    {
        // Arrange
        var db = InMemoryDbContextFactory.Create();
        db.Vacantes.Add(new Vacante
        {
            Id = 1,
            PuestoId = 10,
            FechaApertura = DateTime.UtcNow,
            Motivo = "Activa",
            Estado = "Abierta",
            Activo = true,
            CreadoEn = DateTime.UtcNow
        });
        db.Vacantes.Add(new Vacante
        {
            Id = 2,
            PuestoId = 20,
            FechaApertura = DateTime.UtcNow,
            Motivo = "Eliminada",
            Estado = "Abierta",
            Activo = true,
            CreadoEn = DateTime.UtcNow,
            EliminadoEn = DateTime.UtcNow,
            EliminadoPor = 1
        });
        await db.SaveChangesAsync();

        var handler = new GetAllVacantesQueryHandler(db);

        // Act
        var result = await handler.Handle(new GetAllVacantesQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value!);
        Assert.Equal("Activa", result.Value![0].Motivo);
    }

    [Fact]
    public async Task Handle_SinVacantes_DebeRetornarListaVacia()
    {
        // Arrange
        var db = InMemoryDbContextFactory.Create();
        var handler = new GetAllVacantesQueryHandler(db);

        // Act
        var result = await handler.Handle(new GetAllVacantesQuery(), CancellationToken.None);

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
        var fechaApertura = new DateTime(2026, 1, 15);
        db.Vacantes.Add(new Vacante
        {
            Id = 1,
            PuestoId = 10,
            FechaApertura = fechaApertura,
            FechaCierre = new DateTime(2026, 3, 20),
            Motivo = "Expansion",
            Estado = "Cubierta",
            Activo = true,
            CreadoEn = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var handler = new GetAllVacantesQueryHandler(db);

        // Act
        var result = await handler.Handle(new GetAllVacantesQuery(), CancellationToken.None);

        // Assert
        Assert.NotNull(result.Value);
        var dto = result.Value[0];
        Assert.Equal(1, dto.Id);
        Assert.Equal(10, dto.PuestoId);
        Assert.Equal(fechaApertura, dto.FechaApertura);
        Assert.Equal("Expansion", dto.Motivo);
        Assert.Equal("Cubierta", dto.Estado);
    }
}
