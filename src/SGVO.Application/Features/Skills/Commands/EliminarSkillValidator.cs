using FluentValidation;

namespace SGVO.Application.Features.Skills.Commands;

/// <summary>
/// Validator for EliminarSkillCommand.
/// </summary>
public class EliminarSkillValidator : AbstractValidator<EliminarSkillCommand>
{
    public EliminarSkillValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("El Id debe ser mayor a 0.");

        RuleFor(x => x.EliminadoPor)
            .GreaterThan(0)
            .WithMessage("El usuario que elimina debe ser válido.");
    }
}
