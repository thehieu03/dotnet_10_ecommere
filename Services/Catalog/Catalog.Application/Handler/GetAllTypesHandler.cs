namespace Catalog.Application.Handler;

public class GetAllTypesHandler: IRequestHandler<GetAllTypesQuery, IList<TypesResponse>>
{
    private readonly ITypeRepository _repository;

    public GetAllTypesHandler(ITypeRepository repository)
    {
        _repository= repository;
    }
    public async Task<IList<TypesResponse>> Handle(GetAllTypesQuery request, CancellationToken cancellationToken)
    {
        var typeList = await _repository.GetTypesAsync();
        var typeResponses = ProductMapper.Mapper.Map<IEnumerable<ProductType>, IList<TypesResponse>>(typeList.ToList());
        return typeResponses;
    }
}