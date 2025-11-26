namespace Basket.Application.Queries;

public class GetBasketByUserNameQuery(string username) : IRequest<ShoppingCartResponse>
{
    public string Username { get; set; } = username;
}