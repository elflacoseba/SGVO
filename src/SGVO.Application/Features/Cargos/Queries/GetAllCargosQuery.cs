using SGVO.Application.Common;

namespace SGVO.Application.Features.Cargos.Queries;

public sealed record GetAllCargosQuery : IQuery<IReadOnlyList<CargoDto>>;
