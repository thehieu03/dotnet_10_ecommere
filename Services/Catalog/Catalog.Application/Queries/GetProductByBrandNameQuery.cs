namespace Catalog.Application.Queries;

public class GetProductByBrandNameQuery(string brandName) : IRequest<IList<ProductResponse>>
{
    public string BrandName { get; set; } = brandName;
}