var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",new OpenApiInfo()
    {
        Title =  "Catalog API",
        Version = "v1"
    });
});
builder.Services.AddAutoMapper(typeof(Catalog.Application.Mapper.ProductMappingProfile).Assembly);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
// Register Application Services
builder.Services.AddScoped<ICatalogContext, CatalogContext>();
builder.Services.AddScoped<IProductRepository,ProductRepository>();
builder.Services.AddScoped<IBrandRepository,ProductRepository>();
builder.Services.AddScoped<ITypeRepository,ProductRepository>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthorization();

app.MapControllers();

app.Run();
