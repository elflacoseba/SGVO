using SGVO.Application.Features.Vacantes.Queries;
using SGVO.Infrastructure.Persistence.Entities;
using SGVO.Infrastructure.Queries;

namespace SGVO.UnitTests.Queries;

/// <summary>
/// Tests unitarios para el query handler de vacante por ID.
/// </summary>
public class GetVacanteByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_VacanteExistente_DebeRetornarDto()
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
        await db.SaveChangesAsync();

        var handler = new GetVacanteByIdQueryHandler(db);

        // Act
        var result = await handler.Handle(new GetVacanteByIdQuery(1), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(1, result.Value.Id);
        Assert.Equal(10, result.Value.PuestoId);
        Assert.Equal("Nueva posicion", result.Value.Motivo);
    }

    [Fact]
    public async Task Handle_VacanteInexistente_DebeRetornarNull()
    {
        // Arrange
        var db = InMemoryDbContextFactory.Create();
        var handler = new GetVacanteByIdQueryHandler(db);

        // Act
        var result = await handler.Handle(new GetVacanteByIdQuery(999), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task Handle_VacanteEliminada_DebeRetornarNull()
    {
        // Arrange
        var db = InMemoryDbContextFactory.Create();
        db.Vacantes.Add(new Vacante
        {
            Id = 1,
            PuestoId = 10,
            FechaApertura = DateTime.UtcNow,
            Motivo = "Eliminada",
            Estado = "Abierta",
            Activo = true,
            CreadoEn = DateTime.UtcNow,
            EliminadoEn = DateTime.UtcNow,
            EliminadoPor = 1
        });
        await db.SaveChangesAsync();

        var handler = new GetVacanteByIdQueryHandler(db);

        // Act
        var result = await handler.Handle(new GetVacanteByIdQuery(1), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task Handle_DebeMapearCorrectamenteADto()
    {
        // Arrange
        var db = InMemoryDbContextFactory.Create();
        var fechaApertura = new DateTime(2026, 6, 1);
        db.Vacantes.Add(new Vacante
        {
            Id = 5,
            PuestoId = 42,
            FechaApertura = fechaApertura,
            FechaCierre = new DateTime(2026, 7, 15),
            Motivo = "Sustitucion",
            Estado = "EnSeleccion",
            Activo = true,
            CreadoEn = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var handler = new GetVacanteByIdQueryHandler(db);

        // Act
        var result = await handler.Handle(new GetVacanteByIdQuery(5), CancellationToken.None);

        // Assert
        var dto = result.Value!;
        Assert.Equal(5, dto.Id);
        Assert.Equal(42, dto.PuestoId);
        Assert.Equal(fechaApertura, dto.FechaApertura);
        Assert.Equal("Sustitucion", dto.Motivo);
        Assert.Equal("EnSeleccion", dto.Estado);
    }
}
