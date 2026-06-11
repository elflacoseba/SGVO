using FluentValidation.TestHelper;
using SGVO.Application.Features.Cargos.Commands;

namespace SGVO.UnitTests.Commands.Cargos;

/// <summary>
/// FluentValidation tests for CrearCargoValidator.
/// </summary>
public class CrearCargoValidatorTests
{
    private readonly CrearCargoValidator _validator = new();

    [Fact]
    public void ValidRequest_ShouldNotHaveErrors()
    {
        var command = new CrearCargoCommand("Analista Senior", "Descripción válida");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyNombre_ShouldHaveError()
    {
        var command = new CrearCargoCommand("", null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Nombre);
    }

    [Fact]
    public void NombreExceeding150Chars_ShouldHaveError()
    {
        var command = new CrearCargoCommand(new string('A', 151), null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Nombre);
    }

    [Fact]
    public void DescripcionExceeding500Chars_ShouldHaveError()
    {
        var command = new CrearCargoCommand("Valid", new string('A', 501));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Descripcion);
    }

    [Fact]
    public void NullDescripcion_ShouldNotHaveError()
    {
        var command = new CrearCargoCommand("Analista", null);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Descripcion);
    }
}
