using SGVO.Application.Common;
using SGVO.Application.Features.TiposUnidadOrganizativa.Dtos;

namespace SGVO.Application.Features.TiposUnidadOrganizativa.Commands;

/// <summary>
/// Command to create a new organizational unit type.
/// </summary>
public sealed record CrearTipoUnidadOrganizativaCommand(string Nombre) : ICommand<TipoUnidadOrganizativaDto>;
