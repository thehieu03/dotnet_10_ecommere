namespace Basket.Infrastructure.Repositories;

public class BaskRepository:IBasketRepository
{
    private readonly IDistributedCache _redisCache;

    public BaskRepository(IDistributedCache distributedCache)
    {
        _redisCache = distributedCache;
    }
    public async Task<ShoppingCart> GetBasketAsync(string userName)
    {
        var basket=await _redisCache.GetStringAsync(userName);
        if(string.IsNullOrEmpty(basket))
            return null;
        return JsonConvert.DeserializeObject<ShoppingCart>(basket);
    }

    public async Task<ShoppingCart> UpdateBasketAsync(ShoppingCart basket)
    {
        await _redisCache.SetStringAsync(basket.UserName, JsonConvert.SerializeObject(basket));
        return await GetBasketAsync(basket.UserName);
    }

    public async Task DeleteBasketAsync(string userName)
    {
        await _redisCache.RemoveAsync(userName);
    }
}