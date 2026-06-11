using Microsoft.EntityFrameworkCore;
using SGVO.Domain.Interfaces;

namespace SGVO.UnitTests.Queries;

/// <summary>
/// Test implementation of IRepository{T} backed by EF Core InMemory database.
/// Used in unit tests to avoid mocking.
/// </summary>
public class TestRepository<T> : IRepository<T> where T : class
{
    private readonly DbContext _context;

    public TestRepository(DbContext context)
    {
        _context = context;
    }

    public async Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<T>().FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<T>().ToListAsync(cancellationToken);
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
