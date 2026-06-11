using SGVO.Application.Common;
using SGVO.Application.Features.UnidadesOrganizativas.Queries;

namespace SGVO.Application.Features.UnidadesOrganizativas.Commands;

/// <summary>
/// Command to create a new organizational unit.
/// </summary>
public sealed record CrearUnidadOrganizativaCommand(
    string Nombre,
    long TipoUnidadOrganizativaId,
    int? NivelJerarquico,
    long? PadreId) : ICommand<UnidadOrganizativaDetailDto>;
