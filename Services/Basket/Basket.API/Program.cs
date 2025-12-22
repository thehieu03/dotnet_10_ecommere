using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Basket.Application.GrpcService;
using Common.Logging;
using Discount.Grpc.Protos;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);
// Add Serilog Configuration
builder.Host.UseSerilog(Logging.cfg);
// Add Controllers and API Versioning
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Add API Versioning với ApiExplorer
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version"));
})
.AddMvc()
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // Format: v1.0, v2.0
    options.SubstituteApiVersionInUrl = true;
});

// Configure Swagger để tự động discover versions
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Basket API", Version = "v1" });
    c.SwaggerDoc("v2", new() { Title = "Basket API", Version = "v2" });
    // Configure Swagger to use versioning
    c.DocInclusionPredicate((version, apiDescription) =>
    {
        if (!apiDescription.TryGetMethodInfo(out var methodInfo))
        {
            return false;
        }
        var versions = methodInfo.DeclaringType?
                        .GetCustomAttributes(true)
                        .OfType<ApiVersionAttribute>()
                        .SelectMany(a => a.Versions);

        return versions?.Any(v => $"v{v.ToString()}" == version) ?? false;
    });

});

// Add autoMapper and
builder.Services.AddAutoMapper(typeof(Program).Assembly);
// Add MediatR
var assemblies = new Assembly[]
{
    Assembly.GetExecutingAssembly(), typeof(CreateShoppingCartHandler).Assembly,
};
builder.Services.AddMediatR(cfg => { cfg.RegisterServicesFromAssemblies(assemblies); });
// Add Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetValue<string>("CacheSettings:ConnectionString");
});
builder.Services.AddMassTransit(configure =>
{
    configure.UsingRabbitMq((ct, cfg) => { cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]); });
});
builder.Services.AddMassTransitHostedService();
// Application Services
builder.Services.AddScoped<IBasketRepository, BaskRepository>();
builder.Services.AddScoped<DiscountGrpcService>();

// Register gRPC Client for Discount Service
builder.Services.AddGrpcClient<DiscountService.DiscountServiceClient>(cfg =>
{
    var discountUrl = builder.Configuration["GrpcSettings:DiscountUrl"]
        ?? throw new InvalidOperationException("GrpcSettings:DiscountUrl is not configured");
    cfg.Address = new Uri(discountUrl);
});

// Register DiscountGrpcService
builder.Services.AddScoped<DiscountGrpcService>();
var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Basket.API v1");
    c.SwaggerEndpoint("/swagger/v2/swagger.json", "Basket.API v2");
    c.RoutePrefix = "swagger"; // Swagger UI sẽ ở /swagger
});
app.UseHttpsRedirection();
app.MapControllers();
app.Run();