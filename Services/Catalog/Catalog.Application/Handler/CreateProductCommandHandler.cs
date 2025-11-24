namespace Catalog.Application.Handler;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductResponse>
{
    private readonly IProductRepository _productRepository;

    public CreateProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = ProductMapper.Mapper.Map<CreateProductCommand, Product>(request);
        if (product is null)
        {
            throw new ApplicationException("There is an issue with the product creatint new product.");
        }

        var newProduct = await _productRepository.CreateProductAsync(product);
        var productResponse = ProductMapper.Mapper.Map<Product, ProductResponse>(newProduct);
        return productResponse;
    }
}

