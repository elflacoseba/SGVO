using FluentValidation.TestHelper;
using SGVO.Application.Features.Skills.Commands;

namespace SGVO.UnitTests.Commands.Skills;

/// <summary>
/// FluentValidation tests for ActualizarSkillValidator.
/// </summary>
public class ActualizarSkillValidatorTests
{
    private readonly ActualizarSkillValidator _validator = new();

    [Fact]
    public void ValidRequest_ShouldNotHaveErrors()
    {
        var command = new ActualizarSkillCommand(1, "Python", "Técnica", "Backend");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyNombre_ShouldHaveError()
    {
        var command = new ActualizarSkillCommand(1, "", null, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Nombre);
    }

    [Fact]
    public void IdLessOrEqualZero_ShouldHaveError()
    {
        var command = new ActualizarSkillCommand(0, "Valid", null, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void NombreExceeding150Chars_ShouldHaveError()
    {
        var command = new ActualizarSkillCommand(1, new string('A', 151), null, null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Nombre);
    }
}
