using Catalog.Core.Specs;

namespace Catalog.Application.Queries;

public class GetAllProductsQuery:IRequest<Pagination<ProductResponse>>
{
    public readonly CatalogSpecParams CatalogSpecParams;

    public GetAllProductsQuery(CatalogSpecParams catalogSpecParams)
    {
        CatalogSpecParams = catalogSpecParams;
    }
}