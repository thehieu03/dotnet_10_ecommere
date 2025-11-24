namespace Catalog.Application.Handler;

public class GetProductByTypeNameHandler: IRequestHandler<GetProductByTypeNameQuery, IList<ProductResponse>>
{
    private readonly IProductRepository _repository;

    public GetProductByTypeNameHandler(IProductRepository repository)
    {
        _repository=repository;
    }
    public async Task<IList<ProductResponse>> Handle(GetProductByTypeNameQuery request, CancellationToken cancellationToken)
    {
        var products = await _repository.GetProductByTypeAsync(request.TypeName);
        var productResponses = ProductMapper.Mapper.Map<IEnumerable<Product>, IList<ProductResponse>>(products.ToList());
        return productResponses;
    }
}