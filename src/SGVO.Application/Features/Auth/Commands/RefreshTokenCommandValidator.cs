using FluentValidation;
using SGVO.Application.Features.Auth.Commands;

namespace SGVO.Application.Features.Auth.Commands;

/// <summary>
/// Validator for RefreshTokenCommand.
/// </summary>
public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("El refresh token es obligatorio.")
            .MaximumLength(500)
            .WithMessage("El refresh token no puede exceder los 500 caracteres.");
    }
}
