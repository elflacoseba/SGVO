using SGVO.Application.Common;
using SGVO.Application.Features.TiposUnidadOrganizativa.Dtos;

namespace SGVO.Application.Features.TiposUnidadOrganizativa.Commands;

/// <summary>
/// Command to update an existing organizational unit type.
/// </summary>
public sealed record ActualizarTipoUnidadOrganizativaCommand(long Id, string Nombre) : ICommand<TipoUnidadOrganizativaDto>;
