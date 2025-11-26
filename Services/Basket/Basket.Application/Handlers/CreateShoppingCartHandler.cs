namespace Basket.Application.Handlers;

public class CreateShoppingCartHandler(IBasketRepository basketRepository)
    : IRequestHandler<CreateShoppingCartCommand, ShoppingCartResponse>
{
    private IBasketRepository BasketRepository { get; set; } = basketRepository;

    public async Task<ShoppingCartResponse> Handle(CreateShoppingCartCommand request, CancellationToken cancellationToken)
    {
        var shoppingCart = await BasketRepository.UpdateBasketAsync(new ShoppingCart()
        {
            UserName = request.UserName,
            Items = request.ShoppingCartItems
        });
        var shoppingCartResponse = BasketMapper.Mapper.Map<ShoppingCart, ShoppingCartResponse>(shoppingCart);
        return shoppingCartResponse;
    }
}