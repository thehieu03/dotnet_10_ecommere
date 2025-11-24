namespace Catalog.Core.Repositories;

public interface IBrandRepository
{
    Task<IEnumerable<ProductBrand>> getAllBrandsAsync();
}