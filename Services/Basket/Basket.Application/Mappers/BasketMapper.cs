namespace Basket.Application.Mappers;

public class BasketMapper
{
    private static readonly Lazy<IMapper> lazy = new Lazy<IMapper>(() =>
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.ShouldMapProperty = p => p.GetMethod.IsPublic || p.GetMethod.IsAssembly;
            cfg.AddProfile<BasketMappingProfile>();
        });
        var mapper= config.CreateMapper();
        return mapper;
    });
    public static IMapper Mapper => lazy.Value;
}