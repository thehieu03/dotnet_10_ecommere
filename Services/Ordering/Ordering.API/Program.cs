using Common.Logging;
using EventBus.Messages.Common;
using MassTransit;
using Ordering.API.EventBusConsumer;
using Ordering.API.Extensions;
using Ordering.Application.Extensions;
using Ordering.Infrastructure.Data;
using Ordering.Infrastructure.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog Configuration
builder.Host.UseSerilog(Logging.cfg);
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Add API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
});
// Consumer class
builder.Services.AddScoped<BasketOrderingConsumer>();
builder.Services.AddScoped<BasketCheckoutEventV2>();
// Add a versioned API explorer to support Swagger
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger generation
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo()
    {
        Title = "Ordering API",
        Version = "v1",
        Description = "Ordering Service API"
    });
});
// Mass Transit
builder.Services.AddMassTransit(config =>
{
    // Mark this a consumer
    config.AddConsumer<BasketOrderingConsumer>();
    config.AddConsumer<BasketOrderingConsumerV2>();
    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
        // provide the queue name with consumer settings
        cfg.ReceiveEndpoint(EventBusConstant.BasketCheckoutQueue, c =>
        {
            c.ConfigureConsumer<BasketOrderingConsumer>(ctx);
        });
        //V2 Version
        cfg.ReceiveEndpoint(EventBusConstant.BasketCheckoutQueueV2, c =>
        {
            c.ConfigureConsumer<BasketOrderingConsumerV2>(ctx);
        });
    });
    
});
builder.Services.AddMassTransitHostedService();
// Apply db Migrations

// Application Services
builder.Services.AddApplicationServices();
// Infra services
builder.Services.AddInfraServices(builder.Configuration);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Apply db Migrations
app.MigrateDatabase<OrderContext>((context, services) =>
{
    var logger = services.GetService<ILogger<OrderContextSeed>>();
    OrderContextSeed.SeedAsync(context, logger).Wait();
});

// CORS must be before MapControllers
app.UseCors();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();