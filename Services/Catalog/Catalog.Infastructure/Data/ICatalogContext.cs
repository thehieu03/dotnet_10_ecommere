namespace Catalog.Infastructure.Data;

public interface ICatalogContext
{
    IMongoCollection<Product> Products { get; }
    IMongoCollection<ProductBrand> ProductBrands { get; }
    IMongoCollection<ProductType> ProductTypes { get; }
}