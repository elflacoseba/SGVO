using FluentValidation.TestHelper;
using SGVO.Application.Features.Auth.Commands;

namespace SGVO.UnitTests.Validators;

/// <summary>
/// Tests unitarios para RefreshTokenCommandValidator.
/// </summary>
public class RefreshTokenCommandValidatorTests
{
    private readonly RefreshTokenCommandValidator _validator = new();

    [Fact]
    public void RefreshToken_Empty_ShouldFail()
    {
        var command = new RefreshTokenCommand("");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.RefreshToken);
    }

    [Fact]
    public void RefreshToken_ExceedsMaxLength_ShouldFail()
    {
        var command = new RefreshTokenCommand(new string('t', 501));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.RefreshToken);
    }

    [Fact]
    public void RefreshToken_AtMaxLength_ShouldPass()
    {
        var command = new RefreshTokenCommand(new string('t', 500));
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.RefreshToken);
    }

    [Fact]
    public void ValidRefreshToken_ShouldPass()
    {
        var command = new RefreshTokenCommand("valid-refresh-token-here");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
