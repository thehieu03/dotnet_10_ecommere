namespace Catalog.Application.Commands;

public class UpdateProductCommand : IRequest<bool>
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Summary { get; set; }
    public string Description { get; set; }
    public string ImageFile { get; set; }
    public ProductBrand Brands { get; set; }
    public ProductType Types { get; set; }
    public decimal Price { get; set; }

    public UpdateProductCommand(string id, string name, string summary, string description, string imageFile, ProductBrand brands, ProductType types, decimal price)
    {
        Id = id;
        Name = name;
        Summary = summary;
        Description = description;
        ImageFile = imageFile;
        Brands = brands;
        Types = types;
        Price = price;
    }
}

