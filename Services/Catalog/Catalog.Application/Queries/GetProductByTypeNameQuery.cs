namespace Catalog.Application.Queries;

public abstract class GetProductByTypeNameQuery(string typeName) : IRequest<IList<ProductResponse>>
{
    public string TypeName { get; } = typeName;
}