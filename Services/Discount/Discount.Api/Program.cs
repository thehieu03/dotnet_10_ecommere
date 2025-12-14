using Common.Logging;
using Discount.Api.Services;
using Discount.Application.Handlers;
using Discount.Core.Repositories;
using Discount.Infrastructure.Extensions;
using Discount.Infrastructure.Repositories;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
// Add Serilog Configuration
builder.Host.UseSerilog(Logging.cfg);
// Register AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);
// Register MediatR
var assemblies = new Assembly[]
{
    Assembly.GetExecutingAssembly(),
    typeof(CreateDiscountCommandHandler).Assembly
};
builder.Services.AddMediatR(cfg=> cfg.RegisterServicesFromAssemblies(assemblies));
// Add services to the container.
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
builder.Services.AddGrpc();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
// Migrate Database
app.MigrateDatabase<Program>();
app.UseRouting();
#pragma warning disable ASP0014 // Suggest using top level route registrations
app.UseEndpoints(endpoints =>
{
    endpoints.MapGrpcService<DiscountServices>();
    endpoints.MapGet("/", async context =>
    {
        await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. " +
                                          "To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
    });
});
#pragma warning restore ASP0014 // Suggest using top level route registrations


app.Run();
