using Basket.Application.GrpcService;
using Common.Logging;
using Discount.Grpc.Protos;
using MassTransit;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
// Add Serilog Configuration
builder.Host.UseSerilog(Logging.cfg);
// Add Controllers and API Versioning
builder.Services.AddControllers();
builder.Services.AddOpenApi();
// Add API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
});
// Add versioned API explorer to support Swagger
builder.Services.AddEndpointsApiExplorer();
// Configure Swagger generation
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo()
    {
        Title = "Basket API",
        Version = "v1",
        Description = "Basket Service API"
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
    cfg.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]);
});

// Register DiscountGrpcService
builder.Services.AddScoped<Basket.Application.GrpcService.DiscountGrpcService>();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.MapControllers();
app.Run();