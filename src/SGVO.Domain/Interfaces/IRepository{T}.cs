namespace SGVO.Domain.Interfaces;

/// <summary>
/// Repositorio genérico para operaciones CRUD básicas sobre entidades del dominio.
/// </summary>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Obtiene una entidad por su identificador.
    /// </summary>
    Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todas las entidades.
    /// </summary>
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Agrega una nueva entidad.
    /// </summary>
    void Add(T entity);

    /// <summary>
    /// Actualiza una entidad existente.
    /// </summary>
    void Update(T entity);

    /// <summary>
    /// Elimina una entidad.
    /// </summary>
    void Delete(T entity);
}
