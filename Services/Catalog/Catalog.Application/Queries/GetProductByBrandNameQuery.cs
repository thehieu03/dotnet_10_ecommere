namespace Catalog.Application.Queries;

public class GetProductByBrandNameQuery:IRequest<IList<ProductResponse>>
{
    public string BrandName { get; set; }
    public GetProductByBrandNameQuery(string brandName)
    {
        BrandName = brandName;
    }
}