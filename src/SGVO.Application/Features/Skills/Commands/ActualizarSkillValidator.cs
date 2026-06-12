using FluentValidation;
using SGVO.Shared;

namespace SGVO.Application.Features.Skills.Commands;

/// <summary>
/// Validator for ActualizarSkillCommand.
/// </summary>
public class ActualizarSkillValidator : AbstractValidator<ActualizarSkillCommand>
{
    public ActualizarSkillValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("El Id debe ser mayor a 0.");

        RuleFor(x => x.Nombre)
            .NotEmpty()
            .WithMessage("El nombre es obligatorio.")
            .MaximumLength(DomainConstants.SkillNombreMaxLength)
            .WithMessage($"El nombre no puede exceder los {DomainConstants.SkillNombreMaxLength} caracteres.");

        RuleFor(x => x.Categoria)
            .MaximumLength(DomainConstants.SkillCategoriaMaxLength)
            .WithMessage($"La categoría no puede exceder los {DomainConstants.SkillCategoriaMaxLength} caracteres.");

        RuleFor(x => x.Descripcion)
            .MaximumLength(DomainConstants.SkillDescripcionMaxLength)
            .WithMessage($"La descripción no puede exceder los {DomainConstants.SkillDescripcionMaxLength} caracteres.");
    }
}
