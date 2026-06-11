using SGVO.Application.Common;

namespace SGVO.Application.Features.Cargos.Queries;

/// <summary>
/// Query to retrieve a single cargo by ID.
/// </summary>
public sealed record GetCargoByIdQuery(long Id) : IQuery<CargoDetailDto?>;
