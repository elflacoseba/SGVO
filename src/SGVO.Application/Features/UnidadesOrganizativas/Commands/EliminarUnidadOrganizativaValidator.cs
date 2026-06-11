using FluentValidation;

namespace SGVO.Application.Features.UnidadesOrganizativas.Commands;

/// <summary>
/// Validator for EliminarUnidadOrganizativaCommand.
/// </summary>
public class EliminarUnidadOrganizativaValidator : AbstractValidator<EliminarUnidadOrganizativaCommand>
{
    public EliminarUnidadOrganizativaValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("El Id debe ser mayor a 0.");
    }
}
