using FluentValidation.TestHelper;
using SGVO.Application.Features.Skills.Commands;

namespace SGVO.UnitTests.Commands.Skills;

/// <summary>
/// FluentValidation tests for CrearSkillValidator.
/// </summary>
public class CrearSkillValidatorTests
{
    private readonly CrearSkillValidator _validator = new();

    [Fact]
    public void ValidRequest_ShouldNotHaveErrors()
    {
        var command = new CrearSkillCommand("Python", "Técnica", "Backend");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyNombre_ShouldHaveError()
    {
        var command = new CrearSkillCommand("", null, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Nombre);
    }

    [Fact]
    public void NombreExceeding150Chars_ShouldHaveError()
    {
        var command = new CrearSkillCommand(new string('A', 151), null, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Nombre);
    }

    [Fact]
    public void CategoriaExceeding100Chars_ShouldHaveError()
    {
        var command = new CrearSkillCommand("Valid", new string('A', 101), null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Categoria);
    }

    [Fact]
    public void DescripcionExceeding500Chars_ShouldHaveError()
    {
        var command = new CrearSkillCommand("Valid", null, new string('A', 501));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Descripcion);
    }

    [Fact]
    public void NullOptionalFields_ShouldNotHaveErrors()
    {
        var command = new CrearSkillCommand("Python", null, null);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Categoria);
        result.ShouldNotHaveValidationErrorFor(x => x.Descripcion);
    }
}
