using FluentValidation.TestHelper;
using SGVO.Application.Features.Cargos.Commands;

namespace SGVO.UnitTests.Commands.Cargos;

/// <summary>
/// FluentValidation tests for ActualizarCargoValidator.
/// </summary>
public class ActualizarCargoValidatorTests
{
    private readonly ActualizarCargoValidator _validator = new();

    [Fact]
    public void ValidRequest_ShouldNotHaveErrors()
    {
        var command = new ActualizarCargoCommand(1, "Analista Senior", "Updated");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyNombre_ShouldHaveError()
    {
        var command = new ActualizarCargoCommand(1, "", null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Nombre);
    }

    [Fact]
    public void IdLessOrEqualZero_ShouldHaveError()
    {
        var command = new ActualizarCargoCommand(0, "Valid", null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void NombreExceeding150Chars_ShouldHaveError()
    {
        var command = new ActualizarCargoCommand(1, new string('A', 151), null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Nombre);
    }
}
