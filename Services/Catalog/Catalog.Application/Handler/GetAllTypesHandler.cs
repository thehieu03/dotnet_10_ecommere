namespace Catalog.Application.Handler;

public class GetAllTypesHandler(ITypeRepository repository) : IRequestHandler<GetAllTypesQuery, IList<TypesResponse>>
{
    public async Task<IList<TypesResponse>> Handle(GetAllTypesQuery request, CancellationToken cancellationToken)
    {
        var typeList = await repository.GetTypesAsync();
        var typeResponses = ProductMapper.Mapper.Map<IEnumerable<ProductType>, IList<TypesResponse>>(typeList.ToList());
        return typeResponses;
    }
}