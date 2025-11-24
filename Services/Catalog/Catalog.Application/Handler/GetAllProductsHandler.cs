namespace Catalog.Application.Handler;

public class GetAllProductsHandler: IRequestHandler<GetAllProductsQuery, IList<ProductResponse>>
{
    private readonly IProductRepository _repository;

    public GetAllProductsHandler(IProductRepository repository)
    {
        _repository = repository;
    }
    public async Task<IList<ProductResponse>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var productList = await _repository.GetAllProductsAsync();
        var productResponses = ProductMapper.Mapper.Map<IList<Product>, IList<ProductResponse>>(productList.ToList());
        return productResponses;
    }
}