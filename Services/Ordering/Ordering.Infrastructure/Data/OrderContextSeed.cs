using Microsoft.Extensions.Logging;
using Ordering.Core.Entities;

namespace Ordering.Infrastructure.Data;

public class OrderContextSeed
{
    public static async Task SeedAsync(OrderContext context, ILogger<OrderContextSeed> logger)
    {
        if (!context.Orders.Any())
        {
            context.Orders.AddRange(GetOrders());
            await context.SaveChangesAsync();
            logger.LogInformation($"Orders Database: {nameof(OrderContext)} seeded!!!");
        }
    }

    private static IEnumerable<Order> GetOrders()
    {
        return new List<Order>()
        {
            new Order()
            {
                UserName = "Hieu",
                FirstName = "Hieu",
                LastName =  "Hieu",
                EmailAddress = "hieunthe171211@gmail.com",
                AddressLine = "HN",
                Country = "vietnam",
                TotalPrice = 750,
                State = "KA",
                ZipCode = "98052",
                CardName = "Visa",
                CardNumber = "1234567890123456",
                CreatedBy = "Hieu",
                Expiration = "12/25",
                Cvv = "123",
                PaymentMethod = 1,
                LastModifiedBy = "Hieu",
                LastModifiedDate = DateTime.Now,
            }
        };
    }
}