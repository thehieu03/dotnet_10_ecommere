namespace Catalog.Infastructure.Data;

public static class TypeContextSeed
{
    public static void SeedData(IMongoCollection<ProductType> typeCollection)
    {
        bool checkType = typeCollection.Find(p => true).Any();
        var basePath = Path.GetDirectoryName(typeof(TypeContextSeed).Assembly.Location) ?? string.Empty;
        string path = Path.Combine(basePath, "Data", "SeedData", "types.json");
        if(!checkType)
        {
            var typesData = File.ReadAllText(path);
            var types = JsonSerializer.Deserialize<List<ProductType>>(typesData);
            if (types != null && types.Count > 0)
            {
                foreach (var type in types)
                {
                    typeCollection.InsertOneAsync(type);
                }
            }
        }
    }
}