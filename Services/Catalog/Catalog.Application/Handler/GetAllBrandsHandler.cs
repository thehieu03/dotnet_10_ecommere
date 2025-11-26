namespace Catalog.Application.Handler;

public class GetAllBrandsHandler(IBrandRepository brandRepository)
    : IRequestHandler<GetAllBrandsQuery, IList<BrandResponse>>
{
    public async Task<IList<BrandResponse>> Handle(GetAllBrandsQuery request, CancellationToken cancellationToken)
    {
        var brands = await brandRepository.GetAllBrandsAsync();
        var brandResponses = ProductMapper.Mapper.Map<IEnumerable<ProductBrand>, IList<BrandResponse>>(brands.ToList());
        return brandResponses;
    }
}