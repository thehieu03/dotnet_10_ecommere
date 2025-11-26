using Catalog.Core.Specs;

namespace Catalog.Application.Queries;

public class GetAllProductsQuery(CatalogSpecParams catalogSpecParams) : IRequest<Pagination<ProductResponse>>
{
    public readonly CatalogSpecParams CatalogSpecParams = catalogSpecParams;
}