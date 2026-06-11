using FluentValidation.TestHelper;
using SGVO.Application.Features.TiposUnidadOrganizativa.Commands;

namespace SGVO.UnitTests.Commands.TiposUnidadOrganizativa;

/// <summary>
/// Unit tests for ReactivarTipoUnidadOrganizativaValidator.
/// </summary>
public class ReactivarTipoUnidadOrganizativaValidatorTests
{
    private readonly ReactivarTipoUnidadOrganizativaValidator _validator = new();

    [Fact]
    public void Id_Zero_ShouldFail()
    {
        var command = new ReactivarTipoUnidadOrganizativaCommand(0);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Id_Negative_ShouldFail()
    {
        var command = new ReactivarTipoUnidadOrganizativaCommand(-1);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ValidCommand_ShouldPass()
    {
        var command = new ReactivarTipoUnidadOrganizativaCommand(1);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
