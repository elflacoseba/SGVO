using FluentValidation;

namespace SGVO.Application.Features.UnidadesOrganizativas.Commands;

/// <summary>
/// Validator for ReactivarUnidadOrganizativaCommand.
/// </summary>
public class ReactivarUnidadOrganizativaValidator : AbstractValidator<ReactivarUnidadOrganizativaCommand>
{
    public ReactivarUnidadOrganizativaValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("El Id debe ser mayor a 0.");
    }
}
