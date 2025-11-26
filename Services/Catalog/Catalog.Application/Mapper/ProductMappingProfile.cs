using Catalog.Core.Specs;

namespace Catalog.Application.Mapper;

public class ProductMappingProfile:Profile
{
    public ProductMappingProfile()
    {
        CreateMap<ProductBrand,BrandResponse>().ReverseMap();
        CreateMap<Product,ProductResponse>().ReverseMap();
        CreateMap<ProductType,TypesResponse>().ReverseMap();
        CreateMap<Product,CreateProductCommand>().ReverseMap();
        CreateMap<Pagination<Product>,Pagination<ProductResponse>>().ReverseMap();
    }
}