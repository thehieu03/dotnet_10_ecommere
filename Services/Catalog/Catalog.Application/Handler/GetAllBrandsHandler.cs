namespace Catalog.Application.Handler;

public class GetAllBrandsHandler: IRequestHandler<GetAllBrandsQuery,IList<BrandResponse>>
{
    private readonly IBrandRepository _brandRepository;

    public GetAllBrandsHandler(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }
    public async Task<IList<BrandResponse>> Handle(GetAllBrandsQuery request, CancellationToken cancellationToken)
    {
        var brands = await _brandRepository.GetAllBrandsAsync();
        var brandResponses = ProductMapper.Mapper.Map<IEnumerable<ProductBrand>, IList<BrandResponse>>(brands.ToList());
        return brandResponses;
    }
}