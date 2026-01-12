namespace Discount.Infrastructure.Extensions;

public static class DbExtenstion
{
    public static IHost MigrateDatabase<TContext>(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var config= services.GetRequiredService<IConfiguration>();
        var logger=services.GetRequiredService<ILogger<TContext>>();
        try
        {
            logger.LogInformation("Discount Db Migration Started");
            ApplyMigrations(config);
            logger.LogInformation("Discount Db Migration Completed");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        return host;
    }

    private static void ApplyMigrations(IConfiguration config)
    {
        // Đọc connection string từ DatabaseSettings:ConnectionString hoặc ConnectionStrings:DiscountDb (từ Aspire)
        var connectionString = config.GetValue<string>("DatabaseSettings:ConnectionString") 
            ?? config.GetConnectionString("DiscountDb")
            ?? throw new InvalidOperationException("Database connection string is not configured. " +
                $"DatabaseSettings:ConnectionString = {config.GetValue<string>("DatabaseSettings:ConnectionString")}, " +
                $"ConnectionStrings:DiscountDb = {config.GetConnectionString("DiscountDb")}");
        
        Console.WriteLine($"[DEBUG] Using connection string: {(connectionString?.Length > 50 ? connectionString.Substring(0, 50) + "..." : connectionString)}");
        
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        using var cmd = new NpgsqlCommand()
        {
            Connection = connection,
        };
        cmd.CommandText = "DROP TABLE IF EXISTS Coupon";
        cmd.ExecuteNonQuery();
        cmd.CommandText =
            @"CREATE TABLE Coupon(Id SERIAL PRIMARY KEY, ProductName VARCHAR(255) NOT NULL, Description TEXT, Amount INT)";
        cmd.ExecuteNonQuery();
        cmd.CommandText="INSERT INTO Coupon(ProductName, Description, Amount) " +
                        " VALUES('Adidas Quick Force indoor Badminton Shoes','Shoe Discount',500)";
        cmd.ExecuteNonQuery();
        cmd.CommandText="INSERT INTO Coupon(ProductName, Description, Amount)  " +
                        "VALUES('Yonex VCORE Pro 100 A Tennis Racquet (270gm,Strung)','Racquet Discount',700)";
        cmd.ExecuteNonQuery();
    }
}