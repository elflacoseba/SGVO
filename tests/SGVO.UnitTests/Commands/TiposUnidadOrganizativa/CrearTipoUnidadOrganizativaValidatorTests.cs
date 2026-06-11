using FluentValidation.TestHelper;
using SGVO.Application.Features.TiposUnidadOrganizativa.Commands;

namespace SGVO.UnitTests.Commands.TiposUnidadOrganizativa;

/// <summary>
/// Unit tests for CrearTipoUnidadOrganizativaValidator.
/// </summary>
public class CrearTipoUnidadOrganizativaValidatorTests
{
    private readonly CrearTipoUnidadOrganizativaValidator _validator = new();

    [Fact]
    public void Nombre_Empty_ShouldFail()
    {
        var command = new CrearTipoUnidadOrganizativaCommand("");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Nombre);
    }

    [Fact]
    public void Nombre_ExceedsMaxLength_ShouldFail()
    {
        var command = new CrearTipoUnidadOrganizativaCommand(new string('a', 101));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Nombre);
    }

    [Fact]
    public void Nombre_AtMaxLength_ShouldPass()
    {
        var command = new CrearTipoUnidadOrganizativaCommand(new string('a', 100));
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Nombre);
    }

    [Fact]
    public void ValidCommand_ShouldPass()
    {
        var command = new CrearTipoUnidadOrganizativaCommand("Facultad");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
