namespace Basket.Application.Responses;

public class ShoppingCartResponse
{
    public string UserName { get; init; }
    public List<ShoppingCartItemResponse> Items { get; init; }

    public ShoppingCartResponse(string username)
    {
        UserName = username;
    }

    public decimal TotalPrice
    {
        get
        {
            decimal totalPrice = 0;
            foreach (var item in Items)
            {
                totalPrice += item.Price*item.Quantity;
            }
            return totalPrice;
        }
    }
}