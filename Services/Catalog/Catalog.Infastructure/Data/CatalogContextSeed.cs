namespace Catalog.Infastructure.Data;

public static class CatalogContextSeed
{
    public static void SeedData(IMongoCollection<Product> productCollection)
    {
        bool checkProduct = productCollection.Find(p => true).Any();
        var basePath = Path.GetDirectoryName(typeof(CatalogContextSeed).Assembly.Location) ?? string.Empty;
        string path = Path.Combine(basePath, "Data", "SeedData", "products.json");
        if(!checkProduct)
        {
            var productsData = File.ReadAllText(path);
            var products = JsonSerializer.Deserialize<List<Product>>(productsData);
            if (products != null && products.Count > 0)
            {
                foreach (var product in products)
                {
                    productCollection.InsertOneAsync(product);
                }
            }
        }
    }
}