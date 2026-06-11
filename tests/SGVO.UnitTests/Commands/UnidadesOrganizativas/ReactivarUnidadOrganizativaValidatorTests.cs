using FluentValidation.TestHelper;
using SGVO.Application.Features.UnidadesOrganizativas.Commands;

namespace SGVO.UnitTests.Commands.UnidadesOrganizativas;

/// <summary>
/// Unit tests for ReactivarUnidadOrganizativaValidator.
/// </summary>
public class ReactivarUnidadOrganizativaValidatorTests
{
    private readonly ReactivarUnidadOrganizativaValidator _validator = new();

    [Fact]
    public void Id_Zero_ShouldFail()
    {
        var command = new ReactivarUnidadOrganizativaCommand(0);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Id_Negative_ShouldFail()
    {
        var command = new ReactivarUnidadOrganizativaCommand(-1);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ValidCommand_ShouldPass()
    {
        var command = new ReactivarUnidadOrganizativaCommand(1);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
