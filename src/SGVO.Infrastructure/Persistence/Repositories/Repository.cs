using Microsoft.EntityFrameworkCore;
using SGVO.Domain.Interfaces;

namespace SGVO.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación genérica del repositorio utilizando Entity Framework Core.
/// Proporciona operaciones CRUD básicas sobre cualquier entidad.
/// </summary>
public class Repository<T> : IRepository<T> where T : class
{
    private readonly Microsoft.EntityFrameworkCore.DbContext _context;

    public Repository(Microsoft.EntityFrameworkCore.DbContext context)
    {
        _context = context;
    }

    public async Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<T>().FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<T>().ToListAsync<T>(cancellationToken);
    }

    public void Add(T entity)
    {
        _context.Set<T>().Add(entity);
    }

    public void Update(T entity)
    {
        _context.Set<T>().Update(entity);
    }

    public void Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
    }
}
