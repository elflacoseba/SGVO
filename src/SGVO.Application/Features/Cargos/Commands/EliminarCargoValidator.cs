using FluentValidation;

namespace SGVO.Application.Features.Cargos.Commands;

/// <summary>
/// Validator for EliminarCargoCommand.
/// </summary>
public class EliminarCargoValidator : AbstractValidator<EliminarCargoCommand>
{
    public EliminarCargoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("El Id debe ser mayor a 0.");

        RuleFor(x => x.EliminadoPor)
            .GreaterThan(0)
            .WithMessage("El usuario que elimina debe ser válido.");
    }
}
