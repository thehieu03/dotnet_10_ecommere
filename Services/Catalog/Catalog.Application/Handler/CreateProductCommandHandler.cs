namespace Catalog.Application.Handler;

public class CreateProductCommandHandler(IProductRepository productRepository)
    : IRequestHandler<CreateProductCommand, ProductResponse>
{
    public async Task<ProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = ProductMapper.Mapper.Map<CreateProductCommand, Product>(request);
        if (product is null)
        {
            throw new ApplicationException("There is an issue with the product creatint new product.");
        }

        var newProduct = await productRepository.CreateProductAsync(product);
        var productResponse = ProductMapper.Mapper.Map<Product, ProductResponse>(newProduct);
        return productResponse;
    }
}

