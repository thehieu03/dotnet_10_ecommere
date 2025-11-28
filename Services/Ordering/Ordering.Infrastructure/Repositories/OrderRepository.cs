using Microsoft.EntityFrameworkCore;
using Ordering.Core.Entities;
using Ordering.Core.Repositories;
using Ordering.Infrastructure.Data;

namespace Ordering.Infrastructure.Repositories;

public class OrderRepository(OrderContext dbContext) : RepositoryBase<Order>(dbContext), IOrderRepository
{
    public async Task<IEnumerable<Order>> GetOrderByUserName(string username)
    {
        var orderList = await dbContext
            .Orders
            .Where(o => o.UserName == username)
            .ToListAsync();
        return orderList;
    }
}
