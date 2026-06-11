using FluentValidation;
using SGVO.Shared;

namespace SGVO.Application.Features.Cargos.Commands;

/// <summary>
/// Validator for CrearCargoCommand.
/// </summary>
public class CrearCargoValidator : AbstractValidator<CrearCargoCommand>
{
    public CrearCargoValidator()
    {
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
