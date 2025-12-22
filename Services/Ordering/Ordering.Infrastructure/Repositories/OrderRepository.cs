using Microsoft.EntityFrameworkCore;
using Ordering.Core.Entities;
using Ordering.Core.Repositories;
using Ordering.Infrastructure.Data;

namespace Ordering.Infrastructure.Repositories;

public class OrderRepository : RepositoryBase<Order>, IOrderRepository
{
    private readonly OrderContext _dbContext;
    
    public OrderRepository(OrderContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IEnumerable<Order>> GetOrderByUserName(string username)
    {
        var orderList = await _dbContext
            .Orders
            .Where(o => o.UserName == username)
            .ToListAsync();
        return orderList;
    }
}
