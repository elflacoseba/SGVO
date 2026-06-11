using FluentValidation.TestHelper;
using SGVO.Application.Features.Cargos.Commands;

namespace SGVO.UnitTests.Commands.Cargos;

/// <summary>
/// FluentValidation tests for ReactivarCargoValidator.
/// </summary>
public class ReactivarCargoValidatorTests
{
    private readonly ReactivarCargoValidator _validator = new();

    [Fact]
    public void ValidRequest_ShouldNotHaveErrors()
    {
        var command = new ReactivarCargoCommand(1);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void IdLessOrEqualZero_ShouldHaveError()
    {
        var command = new ReactivarCargoCommand(0);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}
