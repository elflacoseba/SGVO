using FluentValidation;

namespace SGVO.Application.Features.Cargos.Commands;

/// <summary>
/// Validator for ReactivarCargoCommand.
/// </summary>
public class ReactivarCargoValidator : AbstractValidator<ReactivarCargoCommand>
{
    public ReactivarCargoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("El Id debe ser mayor a 0.");
    }
}
