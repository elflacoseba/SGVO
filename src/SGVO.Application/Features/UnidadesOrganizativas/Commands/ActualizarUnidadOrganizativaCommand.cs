using SGVO.Application.Common;
using SGVO.Application.Features.UnidadesOrganizativas.Queries;

namespace SGVO.Application.Features.UnidadesOrganizativas.Commands;

/// <summary>
/// Command to update an existing organizational unit.
/// </summary>
public sealed record ActualizarUnidadOrganizativaCommand(
    long Id,
    string Nombre,
    long TipoUnidadOrganizativaId,
    int? NivelJerarquico,
    long? PadreId) : ICommand<UnidadOrganizativaDetailDto>;
