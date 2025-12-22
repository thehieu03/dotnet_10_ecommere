using Basket.Application.Responses;
using Basket.Core.Entities;
using EventBus.Messages.Common;

namespace Basket.Application.Mappers;

public class BasketMappingProfile:Profile
{
    public BasketMappingProfile()
    {
        CreateMap<ShoppingCart, ShoppingCartResponse>().ReverseMap();
        CreateMap<ShoppingCartItem, ShoppingCartItemResponse>().ReverseMap();
        CreateMap<BasketCheckout, BasketCheckoutEvent>().ReverseMap();
        CreateMap<BasketCheckoutEventV2, BasketCheckoutV2>().ReverseMap();
    }
}