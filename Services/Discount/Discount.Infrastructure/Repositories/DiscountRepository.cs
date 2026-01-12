namespace Discount.Infrastructure.Repositories;

public class DiscountRepository(IConfiguration configuration) : IDiscountRepository
{
    private string GetConnectionString()
    {
        return configuration.GetValue<string>("DatabaseSettings:ConnectionString") 
            ?? configuration.GetConnectionString("DiscountDb")
            ?? throw new InvalidOperationException("Database connection string is not configured");
    }

    public async Task<Coupon> GetDiscountAsync(string productName)
    {
        await using var connection = new NpgsqlConnection(GetConnectionString());
        var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>
            ("SELECT * FROM Coupon WHERE ProductName=@ProductName", new { ProductName = productName });
        if (coupon == null)
        {
            return new Coupon
            {
                ProductName = "No Discount",
                Amount = 0,
                Description = "No Discount Available"
            };
        }

        return coupon;
    }

    public async Task<bool> CreateDiscountAsync(Coupon coupon)
    {
        await using var connection = new NpgsqlConnection(GetConnectionString());
        var affected = await connection.ExecuteAsync
        ("INSERT INTO Coupon (ProductName, Description, Amout) VALUES (@ProductName, @Description, @Amout)",
            new { ProductName = coupon.ProductName, Description = coupon.Description, Amout = coupon.Amount });
        return affected != 0;
    }

    public async Task<bool> UpdateDiscountAsync(Coupon coupon)
    {
        await using var connection = new NpgsqlConnection(GetConnectionString());
        var affected = await connection.ExecuteAsync
        ("UPDATE Coupon SET ProductName=@ProductName, Description=@Description, Amout=@Amout WHERE Id=@Id",
            new
            {
                ProductName = coupon.ProductName, Description = coupon.Description, Amout = coupon.Amount,
                Id = coupon.Id
            });
        return affected != 0;
    }

    public async Task<bool> DeleteDiscountAsync(string productName)
    {
        await using var connection =
            new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString") ??
                                 throw new InvalidOperationException());
        var affected = await connection.ExecuteAsync("DELETE FROM Coupon WHERE ProductName = @ProductName",
            new { ProductName = productName });
        return affected != 0;
    }
}