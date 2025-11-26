namespace Basket.Infrastructure.Repositories;

public class BaskRepository(IDistributedCache distributedCache) : IBasketRepository
{
    public async Task<ShoppingCart> GetBasketAsync(string userName)
    {
        var basket=await distributedCache.GetStringAsync(userName);
        if(string.IsNullOrEmpty(basket))
            return null;
        return JsonConvert.DeserializeObject<ShoppingCart>(basket);
    }

    public async Task<ShoppingCart> UpdateBasketAsync(ShoppingCart basket)
    {
        await distributedCache.SetStringAsync(basket.UserName, JsonConvert.SerializeObject(basket));
        return await GetBasketAsync(basket.UserName);
    }

    public async Task DeleteBasketAsync(string userName)
    {
        await distributedCache.RemoveAsync(userName);
    }
}