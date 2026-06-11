using FluentValidation.TestHelper;
using SGVO.Application.Features.TiposUnidadOrganizativa.Commands;

namespace SGVO.UnitTests.Commands.TiposUnidadOrganizativa;

/// <summary>
/// Unit tests for ActualizarTipoUnidadOrganizativaValidator.
/// </summary>
public class ActualizarTipoUnidadOrganizativaValidatorTests
{
    private readonly ActualizarTipoUnidadOrganizativaValidator _validator = new();

    [Fact]
    public void Id_Zero_ShouldFail()
    {
        var command = new ActualizarTipoUnidadOrganizativaCommand(0, "Facultad");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Id_Negative_ShouldFail()
    {
        var command = new ActualizarTipoUnidadOrganizativaCommand(-1, "Facultad");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Nombre_Empty_ShouldFail()
    {
        var command = new ActualizarTipoUnidadOrganizativaCommand(1, "");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Nombre);
    }

    [Fact]
    public void Nombre_ExceedsMaxLength_ShouldFail()
    {
        var command = new ActualizarTipoUnidadOrganizativaCommand(1, new string('a', 101));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Nombre);
    }

    [Fact]
    public void Nombre_AtMaxLength_ShouldPass()
    {
        var command = new ActualizarTipoUnidadOrganizativaCommand(1, new string('a', 100));
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Nombre);
    }

    [Fact]
    public void ValidCommand_ShouldPass()
    {
        var command = new ActualizarTipoUnidadOrganizativaCommand(1, "Facultad");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
