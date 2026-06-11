using FluentValidation;

namespace SGVO.Application.Features.TiposUnidadOrganizativa.Commands;

/// <summary>
/// Validator for EliminarTipoUnidadOrganizativaCommand.
/// </summary>
public class EliminarTipoUnidadOrganizativaValidator : AbstractValidator<EliminarTipoUnidadOrganizativaCommand>
{
    public EliminarTipoUnidadOrganizativaValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("El Id debe ser mayor a 0.");
    }
}
