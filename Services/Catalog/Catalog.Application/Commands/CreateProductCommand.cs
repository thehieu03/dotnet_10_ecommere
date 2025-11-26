namespace Catalog.Application.Commands;

public class CreateProductCommand(
    string name,
    string summary,
    string description,
    string imageFile,
    ProductBrand brands,
    ProductType types,
    decimal price)
    : IRequest<ProductResponse>
{
    public string Name { get; init; } = name;
    public string Summary { get; init; } = summary;
    public string Description { get; init; } = description;
    public string ImageFile { get; init; } = imageFile;
    public ProductBrand Brands { get; init; } = brands;
    public ProductType Types { get; init; } = types;
    public decimal Price { get; init; } = price;
}