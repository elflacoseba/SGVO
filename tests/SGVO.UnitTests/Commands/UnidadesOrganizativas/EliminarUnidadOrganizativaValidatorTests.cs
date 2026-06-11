using FluentValidation.TestHelper;
using SGVO.Application.Features.UnidadesOrganizativas.Commands;

namespace SGVO.UnitTests.Commands.UnidadesOrganizativas;

/// <summary>
/// Unit tests for EliminarUnidadOrganizativaValidator.
/// </summary>
public class EliminarUnidadOrganizativaValidatorTests
{
    private readonly EliminarUnidadOrganizativaValidator _validator = new();

    [Fact]
    public void Id_Zero_ShouldFail()
    {
        var command = new EliminarUnidadOrganizativaCommand(0, 1);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Id_Negative_ShouldFail()
    {
        var command = new EliminarUnidadOrganizativaCommand(-1, 1);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ValidCommand_ShouldPass()
    {
        var command = new EliminarUnidadOrganizativaCommand(1, 1);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
