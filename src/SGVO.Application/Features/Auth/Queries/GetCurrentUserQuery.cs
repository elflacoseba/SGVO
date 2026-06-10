using SGVO.Application.Common;
using SGVO.Application.Features.Auth.Dtos;

namespace SGVO.Application.Features.Auth.Queries;

/// <summary>
/// Consulta para obtener la información del usuario autenticado actual.
/// </summary>
public sealed record GetCurrentUserQuery(ulong UserId) : IQuery<UserDto?>;
