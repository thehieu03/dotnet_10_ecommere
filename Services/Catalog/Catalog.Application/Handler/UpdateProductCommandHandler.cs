namespace Catalog.Application.Handler;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
{
    private readonly IProductRepository _productRepository;

    public UpdateProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var productEntity = await _productRepository.UpdateProductAsync(new Product()
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

