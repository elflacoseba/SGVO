using FluentValidation.TestHelper;
using SGVO.Application.Features.TiposUnidadOrganizativa.Commands;

namespace SGVO.UnitTests.Commands.TiposUnidadOrganizativa;

/// <summary>
/// Unit tests for EliminarTipoUnidadOrganizativaValidator.
/// </summary>
public class EliminarTipoUnidadOrganizativaValidatorTests
{
    private readonly EliminarTipoUnidadOrganizativaValidator _validator = new();

    [Fact]
    public void Id_Zero_ShouldFail()
    {
        var command = new EliminarTipoUnidadOrganizativaCommand(0, 1);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Id_Negative_ShouldFail()
    {
        var command = new EliminarTipoUnidadOrganizativaCommand(-1, 1);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ValidCommand_ShouldPass()
    {
        var command = new EliminarTipoUnidadOrganizativaCommand(1, 1);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
