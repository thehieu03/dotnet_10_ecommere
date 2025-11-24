namespace Catalog.Infastructure.Data;

public static class BrandContextSeed
{
    public static void SeedData(IMongoCollection<ProductBrand> brandCollection)
    {
        bool checkBrand = brandCollection.Find(p => true).Any();
        string path= Path.Combine("Data","SeedData","brands.json");
        if(!checkBrand)
        {
            var brandsData = File.ReadAllText(path);
            var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);
            if (brands != null && brands.Count > 0)
            {
                foreach (var brand in brands)
                {
                    brandCollection.InsertOneAsync(brand);
                }
            }
        }
    }
}