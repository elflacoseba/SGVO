using SGVO.Application.Common;
using SGVO.Application.Features.Skills.Queries;
using SGVO.Infrastructure.Persistence.Entities;
using SGVO.Infrastructure.Queries;

namespace SGVO.UnitTests.Queries;

/// <summary>
/// Tests unitarios para el query handler de listado de skills.
/// </summary>
public class GetAllSkillsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ConSkillsActivos_DebeRetornarListado()
    {
        // Arrange
        var db = InMemoryDbContextFactory.Create();
        db.Skills.Add(new Skill
        {
            Id = 1,
            Nombre = "C#",
            Categoria = "Tecnica",
            Activo = true,
            CreadoEn = DateTime.UtcNow
        });
        db.Skills.Add(new Skill
        {
            Id = 2,
            Nombre = "Comunicacion",
            Categoria = "Blanda",
            Activo = true,
            CreadoEn = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var handler = new GetAllSkillsQueryHandler(db);

        // Act
        var result = await handler.Handle(new GetAllSkillsQuery(new PageParameters()), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value!.Items.Count);
        Assert.Contains(result.Value.Items, s => s.Nombre == "C#");
        Assert.Contains(result.Value.Items, s => s.Nombre == "Comunicacion");
    }

    [Fact]
    public async Task Handle_ConSkillsEliminados_DebeExcluirLosEliminados()
    {
        // Arrange
        var db = InMemoryDbContextFactory.Create();
        db.Skills.Add(new Skill
        {
            Id = 1,
            Nombre = "Activo",
            Activo = true,
            CreadoEn = DateTime.UtcNow
        });
        db.Skills.Add(new Skill
        {
            Id = 2,
            Nombre = "Eliminado",
            Activo = true,
            CreadoEn = DateTime.UtcNow,
            EliminadoEn = DateTime.UtcNow,
            EliminadoPor = 1
        });
        await db.SaveChangesAsync();

        var handler = new GetAllSkillsQueryHandler(db);

        // Act
        var result = await handler.Handle(new GetAllSkillsQuery(new PageParameters()), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value!.Items);
        Assert.Equal("Activo", result.Value.Items[0].Nombre);
    }

    [Fact]
    public async Task Handle_SinSkills_DebeRetornarListaVacia()
    {
        // Arrange
        var db = InMemoryDbContextFactory.Create();
        var handler = new GetAllSkillsQueryHandler(db);

        // Act
        var result = await handler.Handle(new GetAllSkillsQuery(new PageParameters()), CancellationToken.None);

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
        db.Skills.Add(new Skill
        {
            Id = 1,
            Nombre = "Docker",
            Categoria = "DevOps",
            Activo = true,
            CreadoEn = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var handler = new GetAllSkillsQueryHandler(db);

        // Act
        var result = await handler.Handle(new GetAllSkillsQuery(new PageParameters()), CancellationToken.None);

        // Assert
        Assert.NotNull(result.Value);
        var dto = result.Value!.Items[0];
        Assert.Equal(1, dto.Id);
        Assert.Equal("Docker", dto.Nombre);
        Assert.Equal("DevOps", dto.Categoria);
    }
}
