using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Basket.Application.GrpcService;
using Common.Logging;
using Discount.Grpc.Protos;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetValue<string>("CacheSettings:ConnectionString");
});
builder.Services.AddMassTransit(configure =>
{
    configure.UsingRabbitMq((ct, cfg) => { cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]); });
});
builder.Services.AddMassTransitHostedService();
builder.Services.AddScoped<IBasketRepository, BaskRepository>();
builder.Services.AddScoped<DiscountGrpcService>();
builder.Services.AddGrpcClient<DiscountService.DiscountServiceClient>(cfg =>
{
    var discountUrl = builder.Configuration["GrpcSettings:DiscountUrl"]
        ?? throw new InvalidOperationException("GrpcSettings:DiscountUrl is not configured");
    cfg.Address = new Uri(discountUrl);
});

// Register DiscountGrpcService
builder.Services.AddScoped<DiscountGrpcService>();

// Add Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["IdentityServer:Authority"] ?? "http://localhost:9009";
        options.RequireHttpsMetadata = false; // Only for development
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudiences = new[] { "basket", "gateway" },
            ValidateIssuer = true,
            ValidateLifetime = true
        };
    });

// Add Authorization
builder.Services.AddAuthorization(options =>
{
    // Public: View basket, add items
    options.AddPolicy("Public", policy => policy.RequireAssertion(_ => true));
    
    // RequireAuth: Checkout (cần đăng nhập)
    options.AddPolicy("RequireAuth", policy => 
        policy.RequireAuthenticatedUser());
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

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
    c.RoutePrefix = "swagger";
});

// Middleware order is important!
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();