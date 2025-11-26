namespace Catalog.Application.Responses;

public class ProductResponse
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; init; }
    [BsonElement("Name")]
    public string Name { get; init; }
    public string Summary { get; init; }
    public string Description { get; init; }
    public string ImageFile { get; init; }
    public ProductBrand Brands { get; init; }
    public ProductType Types { get; init; }
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Price { get; init; }
}