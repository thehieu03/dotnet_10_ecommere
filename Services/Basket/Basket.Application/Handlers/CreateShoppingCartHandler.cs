using Basket.Application.GrpcService;

namespace Basket.Application.Handlers;

public class CreateShoppingCartHandler(IBasketRepository basketRepository,DiscountGrpcService discountGrpcService)
    : IRequestHandler<CreateShoppingCartCommand, ShoppingCartResponse>
{
    private IBasketRepository BasketRepository { get; set; } = basketRepository;
    private DiscountGrpcService _discountGrpcService { get; set; }=discountGrpcService;

    public async Task<ShoppingCartResponse> Handle(CreateShoppingCartCommand request, CancellationToken cancellationToken)
    {
        foreach (var item in request.ShoppingCartItems)
        {
            var coupon = await _discountGrpcService.GetDiscount(item.ProductName);
            item.Price -= coupon.Amount;
        }
        var shoppingCart = await BasketRepository.UpdateBasketAsync(new ShoppingCart()
        {
            UserName = request.UserName,
            Items = request.ShoppingCartItems
        });
        var shoppingCartResponse = BasketMapper.Mapper.Map<ShoppingCart, ShoppingCartResponse>(shoppingCart);
        return shoppingCartResponse;
    }
}