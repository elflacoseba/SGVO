using FluentValidation;

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
            .MaximumLength(100)
            .WithMessage("El nombre no puede exceder los 100 caracteres.");
    }
}
