namespace Catalog.Core.Repositories;

public interface ITypeRepository
{
    Task<IEnumerable<Type>> GetTypesAsync();
}