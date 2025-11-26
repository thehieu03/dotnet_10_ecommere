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
    public string Name { get; set; } = name;
    public string Summary { get; set; } = summary;
    public string Description { get; set; } = description;
    public string ImageFile { get; set; } = imageFile;
    public ProductBrand Brands { get; set; } = brands;
    public ProductType Types { get; set; } = types;
    public decimal Price { get; set; } = price;
}