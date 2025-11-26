using Basket.Core.Entities;

namespace Basket.Core.Repositories;

public interface IBasketRepository
{
    Task<ShoppingCart> GetBasketAsync(string userName);
    Task<ShoppingCart> UpdateBasketAsync(ShoppingCart basket);
    Task DeleteBasketAsync(string userName);
}