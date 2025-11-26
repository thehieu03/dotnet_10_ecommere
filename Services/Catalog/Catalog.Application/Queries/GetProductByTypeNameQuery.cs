namespace Catalog.Application.Queries;

public class GetProductByTypeNameQuery(string typeName) : IRequest<IList<ProductResponse>>
{
    public string TypeName { get; set; } = typeName;
}