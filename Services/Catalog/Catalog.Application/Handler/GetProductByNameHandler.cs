namespace Catalog.Application.Handler;

public class GetProductByNameHandler(IProductRepository repository)
    : IRequestHandler<GetProductByNameQuery, IList<ProductResponse>>
{
    public async Task<IList<ProductResponse>> Handle(GetProductByNameQuery request, CancellationToken cancellationToken)
    {
        var products = await repository.GetProductByNameAsync(request.Name);
        var productResponses = ProductMapper.Mapper.Map<IEnumerable<Product>, IList<ProductResponse>>(products.ToList());
        return productResponses;
    }
}