using FluentValidation;

namespace SGVO.Application.Features.TiposUnidadOrganizativa.Commands;

/// <summary>
/// Validator for ActualizarTipoUnidadOrganizativaCommand.
/// </summary>
public class ActualizarTipoUnidadOrganizativaValidator : AbstractValidator<ActualizarTipoUnidadOrganizativaCommand>
{
    public ActualizarTipoUnidadOrganizativaValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("El Id debe ser mayor a 0.");

        RuleFor(x => x.Nombre)
            .NotEmpty()
            .WithMessage("El nombre es obligatorio.")
            .MaximumLength(100)
            .WithMessage("El nombre no puede exceder los 100 caracteres.");
    }
}
