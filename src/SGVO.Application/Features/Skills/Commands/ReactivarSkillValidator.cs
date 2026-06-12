using FluentValidation;

namespace SGVO.Application.Features.Skills.Commands;

/// <summary>
/// Validator for ReactivarSkillCommand.
/// </summary>
public class ReactivarSkillValidator : AbstractValidator<ReactivarSkillCommand>
{
    public ReactivarSkillValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("El Id debe ser mayor a 0.");
    }
}
