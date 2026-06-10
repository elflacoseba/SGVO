using FluentValidation.TestHelper;
using SGVO.Application.Features.Auth.Commands;

namespace SGVO.UnitTests.Validators;

/// <summary>
/// Tests unitarios para LoginCommandValidator.
/// </summary>
public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    [Fact]
    public void Username_Empty_ShouldFail()
    {
        var command = new LoginCommand("", "password123");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void Password_Empty_ShouldFail()
    {
        var command = new LoginCommand("admin", "");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Username_ExceedsMaxLength_ShouldFail()
    {
        var command = new LoginCommand(new string('a', 101), "password123");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void Password_ExceedsMaxLength_ShouldFail()
    {
        var command = new LoginCommand("admin", new string('p', 256));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Username_AtMaxLength_ShouldPass()
    {
        var command = new LoginCommand(new string('a', 100), "password123");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void Password_AtMaxLength_ShouldPass()
    {
        var command = new LoginCommand("admin", new string('p', 255));
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void ValidCommand_ShouldPass()
    {
        var command = new LoginCommand("admin", "password123");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
