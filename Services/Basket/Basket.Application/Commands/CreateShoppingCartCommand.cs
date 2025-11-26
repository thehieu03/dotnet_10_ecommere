using Basket.Core.Entities;

namespace Basket.Application.Commands;

public class CreateShoppingCartCommand(string userName, List<ShoppingCartItem> shoppingCartItems)
    : IRequest<ShoppingCartResponse>
{
    public  string UserName { get; set; } = userName;
    public List<ShoppingCartItem> ShoppingCartItems { get; set; } = shoppingCartItems;
}