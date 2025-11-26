namespace Catalog.Application.Handler;

public class GetProductByTypeNameHandler(IProductRepository repository)
    : IRequestHandler<GetProductByTypeNameQuery, IList<ProductResponse>>
{
    public async Task<IList<ProductResponse>> Handle(GetProductByTypeNameQuery request, CancellationToken cancellationToken)
    {
        var products = await repository.GetProductByTypeAsync(request.TypeName);
        var productResponses = ProductMapper.Mapper.Map<IEnumerable<Product>, IList<ProductResponse>>(products.ToList());
        return productResponses;
    }
}