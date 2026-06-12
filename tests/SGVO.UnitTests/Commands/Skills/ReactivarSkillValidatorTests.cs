using FluentValidation.TestHelper;
using SGVO.Application.Features.Skills.Commands;

namespace SGVO.UnitTests.Commands.Skills;

/// <summary>
/// FluentValidation tests for ReactivarSkillValidator.
/// </summary>
public class ReactivarSkillValidatorTests
{
    private readonly ReactivarSkillValidator _validator = new();

    [Fact]
    public void ValidRequest_ShouldNotHaveErrors()
    {
        var command = new ReactivarSkillCommand(1);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void IdLessOrEqualZero_ShouldHaveError()
    {
        var command = new ReactivarSkillCommand(0);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}
