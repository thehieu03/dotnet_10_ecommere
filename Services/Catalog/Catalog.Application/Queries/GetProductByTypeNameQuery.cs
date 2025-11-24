namespace Catalog.Application.Queries;

public class GetProductByTypeNameQuery:IRequest<IList<ProductResponse>>
{
    public string TypeName { get; set; }

    public GetProductByTypeNameQuery(string typeName)
    {
        TypeName = typeName;
    }
}