namespace Catalog.Application.Commands;

public class UpdateProductCommand(
    string id,
    string name,
    string summary,
    string description,
    string imageFile,
    ProductBrand brands,
    ProductType types,
    decimal price)
    : IRequest<bool>
{
    public string Id { get; } = id;
    public string Name { get; } = name;
    public string Summary { get; } = summary;
    public string Description { get; } = description;
    public string ImageFile { get; } = imageFile;
    public ProductBrand Brands { get; } = brands;
    public ProductType Types { get; } = types;
    public decimal Price { get; } = price;
}

