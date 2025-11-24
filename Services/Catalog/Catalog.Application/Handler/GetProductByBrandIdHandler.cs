namespace Catalog.Application.Handler;

public class GetProductByBrandIdHandler:IRequestHandler<GetProductByBrandNameQuery,IList<ProductResponse>>
{
    private readonly IProductRepository _repository;

    public GetProductByBrandIdHandler(IProductRepository repository)
    {
        _repository=repository;
    }
    public async Task<IList<ProductResponse>> Handle(GetProductByBrandNameQuery request, CancellationToken cancellationToken)
    {
        var products = await _repository.GetProductByBrandAsync(request.BrandName);
        var productResponses = ProductMapper.Mapper.Map<IEnumerable<Product>, IList<ProductResponse>>(products.ToList());
        return productResponses;
    }
}