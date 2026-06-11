using SGVO.Application.Common;
using SGVO.Shared;

namespace SGVO.Application.Features.UnidadesOrganizativas.Commands;

/// <summary>
/// Command to soft-delete an organizational unit.
/// </summary>
public sealed record EliminarUnidadOrganizativaCommand(long Id, long EliminadoPor) : ICommand<Unit>;
