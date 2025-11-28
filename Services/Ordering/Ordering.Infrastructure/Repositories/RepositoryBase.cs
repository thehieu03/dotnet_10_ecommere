using System.Linq.Expressions;
using Ordering.Core.Common;
using Ordering.Core.Repositories;

namespace Ordering.Infrastructure.Repositories;

public class RepositoryBase<T>:IAsyncRepository<T> where T:EntityBase
{
    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
    {
        throw new NotImplementedException();
    }

    public async Task<T> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<T> AddAsync(T entity)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(T entity)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(T entity)
    {
        throw new NotImplementedException();
    }
}