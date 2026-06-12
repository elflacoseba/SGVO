using FluentValidation.TestHelper;
using SGVO.Application.Features.Skills.Commands;

namespace SGVO.UnitTests.Commands.Skills;

/// <summary>
/// FluentValidation tests for EliminarSkillValidator.
/// </summary>
public class EliminarSkillValidatorTests
{
    private readonly EliminarSkillValidator _validator = new();

    [Fact]
    public void ValidRequest_ShouldNotHaveErrors()
    {
        var command = new EliminarSkillCommand(1, 5);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void IdLessOrEqualZero_ShouldHaveError()
    {
        var command = new EliminarSkillCommand(0, 5);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void EliminadoPorLessOrEqualZero_ShouldHaveError()
    {
        var command = new EliminarSkillCommand(1, 0);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.EliminadoPor);
    }
}
