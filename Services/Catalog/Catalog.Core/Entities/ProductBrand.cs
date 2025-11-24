namespace Catalog.Core.Entities;

public class ProductBrand:BaseEntity
{
    [BsonElement("Name")]
    public string Name { get; set; }
}