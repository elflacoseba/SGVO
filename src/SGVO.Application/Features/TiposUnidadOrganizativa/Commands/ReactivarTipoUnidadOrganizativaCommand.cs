using SGVO.Application.Common;
using SGVO.Application.Features.TiposUnidadOrganizativa.Dtos;

namespace SGVO.Application.Features.TiposUnidadOrganizativa.Commands;

/// <summary>
/// Command to reactivate a soft-deleted organizational unit type.
/// </summary>
public sealed record ReactivarTipoUnidadOrganizativaCommand(long Id) : ICommand<TipoUnidadOrganizativaDto>;
