namespace Catalog.Application.Handler;

public class GetProductByNameHandler:IRequestHandler<GetProductByNameQuery,IList<ProductResponse>>
{
    private readonly IProductRepository _repository;

    public GetProductByNameHandler(IProductRepository repository)
    {
        _repository = repository;
    }
    public async Task<IList<ProductResponse>> Handle(GetProductByNameQuery request, CancellationToken cancellationToken)
    {
        var products = await _repository.GetProductByNameAsync(request.Name);
        var productResponses = ProductMapper.Mapper.Map<IEnumerable<Product>, IList<ProductResponse>>(products.ToList());
        return productResponses;
    }
}