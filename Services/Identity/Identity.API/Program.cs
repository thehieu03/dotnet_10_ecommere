using Duende.IdentityServer;
using Identity.API;
using Serilog;
using Common.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog Configuration
builder.Host.UseSerilog(Logging.cfg);

// Add MVC for UI
builder.Services.AddControllersWithViews();

// Add Authentication for UI
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
    });

// Add Identity Server
builder.Services.AddIdentityServer(options =>
{
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;
    
    // See https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/
    options.EmitStaticAudienceClaim = true;
    
    // UI Configuration
    options.UserInteraction.LoginUrl = "/Account/Login";
    options.UserInteraction.LogoutUrl = "/Account/Logout";
    options.UserInteraction.ErrorUrl = "/Account/Error";
})
.AddInMemoryIdentityResources(Config.IdentityResources)
.AddInMemoryApiScopes(Config.ApiScopes)
.AddInMemoryClients(Config.Clients)
.AddDeveloperSigningCredential(); // Only for development!

// Add CORS for Next.js
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

// Middleware order is important!
app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseIdentityServer();

// Map UI controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.MapGet("/", () => "Identity Server is running!");

app.Run();

