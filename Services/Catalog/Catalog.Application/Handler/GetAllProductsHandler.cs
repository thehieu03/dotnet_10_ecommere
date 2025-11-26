using Catalog.Core.Specs;

namespace Catalog.Application.Handler;

public class GetAllProductsHandler: IRequestHandler<GetAllProductsQuery,Pagination<ProductResponse>>
{
    private readonly IProductRepository _repository;

    public GetAllProductsHandler(IProductRepository repository)
    {
        _repository = repository;
    }
    public async Task<Pagination<ProductResponse>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var productList = await _repository.GetAllProductsAsync(request.CatalogSpecParams);
        var productResponses = ProductMapper.Mapper.Map<Pagination<Product>, Pagination<ProductResponse>>(productList);
        return productResponses;
    }
}