namespace Catalog.Core.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllProductAsync();
    Task<Product> GetProductByIdAsync(string productId);
    Task<IEnumerable<Product>> GetProductByNameAsync(string productName);
    Task<IEnumerable<Product>> GetProductByBrandAsync(string brandName);
    Task<IEnumerable<Product>> GetProductByTypeAsync(string typeName);
    Task<Product> CreateProductAsync(Product product);
    Task<bool> UpdateProductAsync(Product product);
    Task<bool> DeleteProductAsync(string productId);
}