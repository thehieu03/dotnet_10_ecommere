namespace Catalog.Application.Handler;

public class UpdateProductCommandHandler(IProductRepository productRepository)
    : IRequestHandler<UpdateProductCommand, bool>
{
    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var productEntity = await productRepository.UpdateProductAsync(new Product()
        {
            Id=request.Id,
            Name=request.Name,
            Description=request.Description,
            Price=request.Price,
            ImageFile =  request.ImageFile,
            Summary =  request.Summary,
            Brands =  request.Brands,
            Types =  request.Types
        });
        return true;
    }
}

