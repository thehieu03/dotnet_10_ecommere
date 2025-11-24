namespace Catalog.Infastructure.Data;

public class CatalogContext: ICatalogContext
{
    public IMongoCollection<Product> Products { get; }
    public IMongoCollection<ProductBrand> ProductBrands { get; }
    public IMongoCollection<ProductType> ProductTypes { get; }

    public CatalogContext(IConfiguration configuration)
    {
        var client = new MongoClient(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
        var database = client.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));
        ProductBrands = database.GetCollection<ProductBrand>(configuration.GetValue<string>("DatabaseSettings:BrandsCollection"));
        ProductTypes = database.GetCollection<ProductType>(configuration.GetValue<string>("DatabaseSettings:TypesCollection"));
        Products = database.GetCollection<Product>(configuration.GetValue<string>("DatabaseSettings:CollectionName"));
        BrandContextSeed.SeedData(ProductBrands);
        TypeContextSeed.SeedData(ProductTypes);
        CatalogContextSeed.SeedData(Products);
    }
}