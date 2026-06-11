using FluentValidation.TestHelper;
using SGVO.Application.Features.UnidadesOrganizativas.Commands;

namespace SGVO.UnitTests.Commands.UnidadesOrganizativas;

/// <summary>
/// Unit tests for CrearUnidadOrganizativaValidator.
/// </summary>
public class CrearUnidadOrganizativaValidatorTests
{
    private readonly CrearUnidadOrganizativaValidator _validator = new();

    [Fact]
    public void Nombre_Empty_ShouldFail()
    {
        var command = new CrearUnidadOrganizativaCommand("", 1, null, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Nombre);
    }

    [Fact]
    public void Nombre_ExceedsMaxLength_ShouldFail()
    {
        var command = new CrearUnidadOrganizativaCommand(new string('a', 201), 1, null, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Nombre);
    }

    [Fact]
    public void Nombre_AtMaxLength_ShouldPass()
    {
        var command = new CrearUnidadOrganizativaCommand(new string('a', 200), 1, null, null);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Nombre);
    }

    [Fact]
    public void TipoUnidadOrganizativaId_Zero_ShouldFail()
    {
        var command = new CrearUnidadOrganizativaCommand("Test", 0, null, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.TipoUnidadOrganizativaId);
    }

    [Fact]
    public void TipoUnidadOrganizativaId_Negative_ShouldFail()
    {
        var command = new CrearUnidadOrganizativaCommand("Test", -1, null, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.TipoUnidadOrganizativaId);
    }

    [Fact]
    public void ValidCommand_ShouldPass()
    {
        var command = new CrearUnidadOrganizativaCommand("Facultad de Ciencias", 1, 1, null);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ValidCommand_WithPadreId_ShouldPass()
    {
        var command = new CrearUnidadOrganizativaCommand("Departamento", 1, 2, 5);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ValidCommand_WithoutOptionalFields_ShouldPass()
    {
        var command = new CrearUnidadOrganizativaCommand("Test", 1, null, null);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
