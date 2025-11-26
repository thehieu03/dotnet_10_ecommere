using Catalog.Core.Specs;
using MongoDB.Bson;

namespace Catalog.Infastructure.Data.Repositories;

public class ProductRepository(ICatalogContext catalogContext) : IProductRepository, IBrandRepository, ITypeRepository
{
    public async Task<Pagination<Product>> GetAllProductsAsync(CatalogSpecParams catalogSpecParams)
    {
        var builder = Builders<Product>.Filter;
        var filter = builder.Empty;
        
        if (!string.IsNullOrEmpty(catalogSpecParams.Search))
        {
            var searchFilter = builder.Regex(p => p.Name, new BsonRegularExpression(catalogSpecParams.Search, "i"));
            filter = filter & searchFilter;
        }

        if (!string.IsNullOrEmpty(catalogSpecParams.BrandId))
        {
            var brandFilter = builder.Eq(p => p.Brands.Id, catalogSpecParams.BrandId);
            filter = filter & brandFilter;
        }
        
        if (!string.IsNullOrEmpty(catalogSpecParams.TypeId))
        {
            var typeFilter = builder.Eq(p => p.Types.Id, catalogSpecParams.TypeId);
            filter = filter & typeFilter;
        }
        
        var totalItems = await catalogContext
            .Products
            .CountDocumentsAsync(filter);

        var data = await DataFilter(catalogSpecParams, filter);
            
        return new Pagination<Product>(
            catalogSpecParams.PageIndex,
            catalogSpecParams.PageSize,
            (int)totalItems,
            data
        );
    }

    private async Task<IReadOnlyList<Product>> DataFilter(CatalogSpecParams catalogSpecParams, FilterDefinition<Product> filter)
    {
        var sortDefn = Builders<Product>.Sort.Ascending("Name");
        if (!string.IsNullOrEmpty(catalogSpecParams.Short))
        {
            switch (catalogSpecParams.Short)
            {
                case "priceAsc":
                    sortDefn = Builders<Product>.Sort.Ascending(p => p.Price);
                    break;
                case "priceDesc":
                    sortDefn = Builders<Product>.Sort.Descending(p => p.Price);
                    break;
                default:
                    sortDefn = Builders<Product>.Sort.Ascending(p => p.Name);
                    break;
            }
        }

        return await catalogContext
            .Products
            .Find(filter)
            .Sort(sortDefn)
            .Skip(catalogSpecParams.PageSize * (catalogSpecParams.PageIndex - 1))
            .Limit(catalogSpecParams.PageSize)
            .ToListAsync();
    }

    public async Task<Product> GetProductByIdAsync(string productId)
    {
        return await catalogContext
            .Products
            .Find(p=>p.Id==productId)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Product>> GetProductByNameAsync(string productName)
    {
        return await catalogContext
            .Products
            .Find(p => p.Name.ToLower() == productName.ToLower())
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductByBrandAsync(string brandName)
    {
        return await catalogContext
            .Products
            .Find(p => p.Brands.Name.ToLower() == brandName.ToLower())
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductByTypeAsync(string typeName)
    {
        return await catalogContext
            .Products
            .Find(p => p.Types.Name.ToLower() == typeName.ToLower())
            .ToListAsync();
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        await catalogContext.Products.InsertOneAsync(product);
        return product;
    }

    public async Task<bool> UpdateProductAsync(Product product)
    {
        var updateResult = await catalogContext
            .Products
            .ReplaceOneAsync(p => p.Id == product.Id, product);
        return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
    }

    public async Task<bool> DeleteProductAsync(string productId)
    {
        var deleteResult = await catalogContext
            .Products
            .DeleteOneAsync(p => p.Id == productId);
        return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
    }

    public async Task<IEnumerable<ProductBrand>> GetAllBrandsAsync()
    {
        return await catalogContext
            .ProductBrands
            .Find(_ => true)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductType>> GetTypesAsync()
    {
        return await catalogContext
            .ProductTypes
            .Find(_ => true)
            .ToListAsync();
    }
}