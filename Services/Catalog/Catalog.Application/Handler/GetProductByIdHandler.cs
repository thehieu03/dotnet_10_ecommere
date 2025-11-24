namespace Catalog.Application.Handler;

public class GetProductByIdHandler: IRequestHandler<GetProductByIdQuery, ProductResponse>
{
    private readonly IProductRepository _repository;

    public GetProductByIdHandler(IProductRepository repository)
    {
        _repository = repository;
    }
    public async Task<ProductResponse> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetProductByIdAsync(request.Id);
        var productResponse = ProductMapper.Mapper.Map<Product, ProductResponse>(product);
        return productResponse;
    }
}