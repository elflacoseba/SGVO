namespace SGVO.Domain.Interfaces;

/// <summary>
/// Contrato para el patrón Unit of Work.
/// Permite agrupar múltiples operaciones en una única transacción.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Guarda los cambios pendientes de forma asíncrona.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
