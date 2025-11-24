namespace Catalog.Infastructure.Data.Repositories;

public class ProductRepository:IProductRepository,IBrandRepository,ITypeRepository
{
    private readonly ICatalogContext _context;

    public ProductRepository(ICatalogContext catalogContext)
    {
        _context = catalogContext;
    }
    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return await  _context
            .Products
            .Find(_ => true)
            .ToListAsync();
    }

    public async Task<Product> GetProductByIdAsync(string productId)
    {
        return await _context
            .Products
            .Find(p=>p.Id==productId)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Product>> GetProductByNameAsync(string productName)
    {
        return await _context
            .Products
            .Find(p => p.Name.ToLower() == productName.ToLower())
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductByBrandAsync(string brandName)
    {
        return await _context
            .Products
            .Find(p => p.Brands.Name.ToLower() == brandName.ToLower())
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductByTypeAsync(string typeName)
    {
        return await _context
            .Products
            .Find(p => p.Types.Name.ToLower() == typeName.ToLower())
            .ToListAsync();
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        await _context.Products.InsertOneAsync(product);
        return product;
    }

    public async Task<bool> UpdateProductAsync(Product product)
    {
        var updateResult = await _context
            .Products
            .ReplaceOneAsync(p => p.Id == product.Id, product);
        return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
    }

    public async Task<bool> DeleteProductAsync(string productId)
    {
        var deleteResult = await _context
            .Products
            .DeleteOneAsync(p => p.Id == productId);
        return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
    }

    public async Task<IEnumerable<ProductBrand>> GetAllBrandsAsync()
    {
        return await _context
            .ProductBrands
            .Find(_ => true)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductType>> GetTypesAsync()
    {
        return await _context
            .ProductTypes
            .Find(_ => true)
            .ToListAsync();
    }
}