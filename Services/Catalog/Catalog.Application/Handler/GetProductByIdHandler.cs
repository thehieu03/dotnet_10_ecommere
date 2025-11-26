namespace Catalog.Application.Handler;

public class GetProductByIdHandler(IProductRepository repository)
    : IRequestHandler<GetProductByIdQuery, ProductResponse>
{
    public async Task<ProductResponse> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await repository.GetProductByIdAsync(request.Id);
        var productResponse = ProductMapper.Mapper.Map<Product, ProductResponse>(product);
        return productResponse;
    }
}