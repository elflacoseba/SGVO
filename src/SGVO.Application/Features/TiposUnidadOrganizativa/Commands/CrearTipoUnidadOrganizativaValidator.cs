using FluentValidation;
using SGVO.Shared;

namespace SGVO.Application.Features.TiposUnidadOrganizativa.Commands;

/// <summary>
/// Validator for CrearTipoUnidadOrganizativaCommand.
/// </summary>
public class CrearTipoUnidadOrganizativaValidator : AbstractValidator<CrearTipoUnidadOrganizativaCommand>
{
    public CrearTipoUnidadOrganizativaValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty()
            .WithMessage("El nombre es obligatorio.")
            .MaximumLength(DomainConstants.TipoUnidadOrganizativaNombreMaxLength)
            .WithMessage($"El nombre no puede exceder los {DomainConstants.TipoUnidadOrganizativaNombreMaxLength} caracteres.");
    }
}
