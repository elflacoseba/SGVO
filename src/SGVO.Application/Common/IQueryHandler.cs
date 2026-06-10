using SGVO.Shared;

namespace SGVO.Application.Common;

/// <summary>
/// Contrato para el manejador de una consulta.
/// </summary>
public interface IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken = default);
}
