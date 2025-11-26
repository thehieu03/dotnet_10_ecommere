namespace Basket.Application.Handlers;

public class GetBasketByUserNameHandler(IBasketRepository basketRepository)
    : IRequestHandler<GetBasketByUserNameQuery, ShoppingCartResponse>
{
    private readonly IBasketRepository _basketRepository = basketRepository;

    public async Task<ShoppingCartResponse> Handle(GetBasketByUserNameQuery request, CancellationToken cancellationToken)
    {
        var shoppingCart=await _basketRepository.GetBasketAsync(request.Username);
        var shoppingCartResponse = BasketMapper.Mapper.Map<ShoppingCartResponse>(shoppingCart);
        return shoppingCartResponse;
    }
}