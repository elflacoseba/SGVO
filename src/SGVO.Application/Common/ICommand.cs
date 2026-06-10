using SGVO.Shared;

namespace SGVO.Application.Common;

/// <summary>
/// Contrato para un comando que retorna un Result<T>.
/// Un comando representa una operación que modifica el estado del sistema.
/// </summary>
public interface ICommand<TResponse>
{
}
