using FluentValidation.TestHelper;
using SGVO.Application.Features.UnidadesOrganizativas.Commands;

namespace SGVO.UnitTests.Commands.UnidadesOrganizativas;

/// <summary>
/// Unit tests for ActualizarUnidadOrganizativaValidator.
/// </summary>
public class ActualizarUnidadOrganizativaValidatorTests
{
    private readonly ActualizarUnidadOrganizativaValidator _validator = new();

    [Fact]
    public void Id_Zero_ShouldFail()
    {
        var command = new ActualizarUnidadOrganizativaCommand(0, "Test", 1, null, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Id_Negative_ShouldFail()
    {
        var command = new ActualizarUnidadOrganizativaCommand(-1, "Test", 1, null, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Nombre_Empty_ShouldFail()
    {
        var command = new ActualizarUnidadOrganizativaCommand(1, "", 1, null, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Nombre);
    }

    [Fact]
    public void Nombre_ExceedsMaxLength_ShouldFail()
    {
        var command = new ActualizarUnidadOrganizativaCommand(1, new string('a', 201), 1, null, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Nombre);
    }

    [Fact]
    public void TipoUnidadOrganizativaId_Zero_ShouldFail()
    {
        var command = new ActualizarUnidadOrganizativaCommand(1, "Test", 0, null, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.TipoUnidadOrganizativaId);
    }

    [Fact]
    public void ValidCommand_ShouldPass()
    {
        var command = new ActualizarUnidadOrganizativaCommand(1, "Facultad", 1, 1, null);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
