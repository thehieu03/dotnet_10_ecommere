namespace Catalog.Application.Mapper;

public static class ProductMapper
{
    private static readonly Lazy<IMapper> _mapper = new Lazy<IMapper>(() =>
    {
        var config = new MapperConfiguration(
            cfg =>
            {
                cfg.ShouldMapProperty = p => p.GetMethod.IsPublic || p.GetMethod.IsAssembly;
                cfg.AddProfile<ProductMappingProfile>();
            },
            loggerFactory: null
        );

        return config.CreateMapper();
    });

    public static IMapper Mapper => _mapper.Value;
}