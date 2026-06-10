using SGVO.Application.Common;

namespace SGVO.Application.Features.Postulantes.Queries;

public sealed record GetAllPostulantesQuery : IQuery<IReadOnlyList<PostulanteDto>>;
