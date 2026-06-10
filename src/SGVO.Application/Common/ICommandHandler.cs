using SGVO.Shared;

namespace SGVO.Application.Common;

/// <summary>
/// Contrato para el manejador de un comando.
/// </summary>
public interface ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken = default);
}
