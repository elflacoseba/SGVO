using FluentValidation;
using SGVO.Shared;

namespace SGVO.Application.Features.Cargos.Commands;

/// <summary>
/// Validator for ActualizarCargoCommand.
/// </summary>
public class ActualizarCargoValidator : AbstractValidator<ActualizarCargoCommand>
{
    public ActualizarCargoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("El Id debe ser mayor a 0.");

        RuleFor(x => x.Nombre)
            .NotEmpty()
            .WithMessage("El nombre es obligatorio.")
            .MaximumLength(DomainConstants.CargoNombreMaxLength)
            .WithMessage($"El nombre no puede exceder los {DomainConstants.CargoNombreMaxLength} caracteres.");

        RuleFor(x => x.Descripcion)
            .MaximumLength(DomainConstants.CargoDescripcionMaxLength)
            .WithMessage($"La descripción no puede exceder los {DomainConstants.CargoDescripcionMaxLength} caracteres.");
    }
}
