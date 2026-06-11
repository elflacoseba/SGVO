using FluentValidation.TestHelper;
using SGVO.Application.Features.Cargos.Commands;

namespace SGVO.UnitTests.Commands.Cargos;

/// <summary>
/// FluentValidation tests for EliminarCargoValidator.
/// </summary>
public class EliminarCargoValidatorTests
{
    private readonly EliminarCargoValidator _validator = new();

    [Fact]
    public void ValidRequest_ShouldNotHaveErrors()
    {
        var command = new EliminarCargoCommand(1, 5);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void IdLessOrEqualZero_ShouldHaveError()
    {
        var command = new EliminarCargoCommand(0, 5);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void EliminadoPorLessOrEqualZero_ShouldHaveError()
    {
        var command = new EliminarCargoCommand(1, 0);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.EliminadoPor);
    }
}
