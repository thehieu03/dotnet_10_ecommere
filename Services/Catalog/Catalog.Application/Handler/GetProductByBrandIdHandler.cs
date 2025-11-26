namespace Catalog.Application.Handler;

public class GetProductByBrandIdHandler(IProductRepository repository)
    : IRequestHandler<GetProductByBrandNameQuery, IList<ProductResponse>>
{
    public async Task<IList<ProductResponse>> Handle(GetProductByBrandNameQuery request, CancellationToken cancellationToken)
    {
        var products = await repository.GetProductByBrandAsync(request.BrandName);
        var productResponses = ProductMapper.Mapper.Map<IEnumerable<Product>, IList<ProductResponse>>(products.ToList());
        return productResponses;
    }
}