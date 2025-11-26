using Catalog.Core.Specs;

namespace Catalog.Application.Handler;

public class GetAllProductsHandler(IProductRepository repository)
    : IRequestHandler<GetAllProductsQuery, Pagination<ProductResponse>>
{
    public async Task<Pagination<ProductResponse>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var productList = await repository.GetAllProductsAsync(request.CatalogSpecParams);
        var productResponses = ProductMapper.Mapper.Map<Pagination<Product>, Pagination<ProductResponse>>(productList);
        return productResponses;
    }
}