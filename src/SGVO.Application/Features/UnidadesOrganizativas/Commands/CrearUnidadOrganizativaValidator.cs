using FluentValidation;
using SGVO.Shared;

namespace SGVO.Application.Features.UnidadesOrganizativas.Commands;

/// <summary>
/// Validator for CrearUnidadOrganizativaCommand.
/// </summary>
public class CrearUnidadOrganizativaValidator : AbstractValidator<CrearUnidadOrganizativaCommand>
{
    public CrearUnidadOrganizativaValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty()
            .WithMessage("El nombre es obligatorio.")
            .MaximumLength(DomainConstants.UnidadOrganizativaNombreMaxLength)
            .WithMessage($"El nombre no puede exceder los {DomainConstants.UnidadOrganizativaNombreMaxLength} caracteres.");

        RuleFor(x => x.TipoUnidadOrganizativaId)
            .GreaterThan(0)
            .WithMessage("El TipoUnidadOrganizativaId debe ser mayor a 0.");
    }
}
