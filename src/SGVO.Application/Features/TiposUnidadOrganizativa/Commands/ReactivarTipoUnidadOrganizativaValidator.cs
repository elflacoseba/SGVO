using FluentValidation;

namespace SGVO.Application.Features.TiposUnidadOrganizativa.Commands;

/// <summary>
/// Validator for ReactivarTipoUnidadOrganizativaCommand.
/// </summary>
public class ReactivarTipoUnidadOrganizativaValidator : AbstractValidator<ReactivarTipoUnidadOrganizativaCommand>
{
    public ReactivarTipoUnidadOrganizativaValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("El Id debe ser mayor a 0.");
    }
}
