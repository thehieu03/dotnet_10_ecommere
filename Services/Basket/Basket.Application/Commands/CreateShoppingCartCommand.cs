using Basket.Core.Entities;

namespace Basket.Application.Commands;

public class CreateShoppingCartCommand : IRequest<ShoppingCartResponse>
{
    public string UserName { get; set; } = null!;
    public List<ShoppingCartItem> ShoppingCartItems { get; set; } = [];
}