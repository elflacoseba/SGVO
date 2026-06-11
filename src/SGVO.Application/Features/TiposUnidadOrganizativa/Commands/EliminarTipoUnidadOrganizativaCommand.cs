using SGVO.Application.Common;
using SGVO.Shared;

namespace SGVO.Application.Features.TiposUnidadOrganizativa.Commands;

/// <summary>
/// Command to soft-delete an organizational unit type.
/// </summary>
public sealed record EliminarTipoUnidadOrganizativaCommand(long Id, ulong EliminadoPor) : ICommand<Unit>;
