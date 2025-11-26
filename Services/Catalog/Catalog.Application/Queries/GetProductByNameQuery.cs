namespace Catalog.Application.Queries;

public class GetProductByNameQuery(string name) : IRequest<IList<ProductResponse>>
{
    public string Name { get; set; } = name;
}