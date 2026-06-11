using SGVO.Application.Common;
using SGVO.Application.Features.UnidadesOrganizativas.Queries;

namespace SGVO.Application.Features.UnidadesOrganizativas.Commands;

/// <summary>
/// Command to reactivate a soft-deleted organizational unit.
/// </summary>
public sealed record ReactivarUnidadOrganizativaCommand(long Id) : ICommand<UnidadOrganizativaDetailDto>;
