using FluentValidation;
using SGVO.Shared;

namespace SGVO.Application.Features.Skills.Commands;

/// <summary>
/// Validator for CrearSkillCommand.
/// </summary>
public class CrearSkillValidator : AbstractValidator<CrearSkillCommand>
{
    public CrearSkillValidator()
    {
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
