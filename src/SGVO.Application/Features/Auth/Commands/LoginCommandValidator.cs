using FluentValidation;

namespace SGVO.Application.Features.Auth.Commands;

/// <summary>
/// Validador para LoginCommand.
/// </summary>
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("El nombre de usuario es obligatorio.")
            .MaximumLength(100)
            .WithMessage("El nombre de usuario no puede exceder los 100 caracteres.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("La contraseña es obligatoria.")
            .MaximumLength(255)
            .WithMessage("La contraseña no puede exceder los 255 caracteres.");
    }
}
