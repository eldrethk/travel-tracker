using Azure.Storage.Blobs;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;
using TravelExpenseTracker.Data;
using TravelExpenseTracker.Domain;
using TravelExpenseTracker.Models;
using TravelExpenseTracker.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        options.Authority = $"https://login.microsoftonline.com/{builder.Configuration["AzureAd:TenantId"]}/v2.0";
        options.ClientId = builder.Configuration["AzureAd:ClientId"];
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.CallbackPath = "/signin-oidc";
        options.SignedOutCallbackPath = "/signout-callback-oidc";
        options.UsePkce = true;

        // Explicitly remove any client authentication
        options.ClientSecret = null;

        // Configure scopes
        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");

        // Save tokens for later use if needed
        options.SaveTokens = true;

        options.Events = new OpenIdConnectEvents
        {
            OnSignedOutCallbackRedirect = context =>
            {
                context.Response.Redirect("/");
                context.HandleResponse();
                return Task.CompletedTask;
            }
        };
    })
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["GoogleAuth:ClientId"]!;
        options.ClientSecret = builder.Configuration["GoogleAuth:ClientSecret"]!;
        options.CallbackPath = "/signin-google";
    });


//global json setting for API responses
builder.Services.Configure<JsonSerializerOptions>(options =>
{
    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.Configure<AzureBlobContainers>(
    builder.Configuration.GetSection("AzureBlobContainers"));

builder.Services.AddSingleton(sp =>
{
    return new BlobServiceClient(builder.Configuration.GetConnectionString("AzureStorage"));
});

// Always register CosmosClient (this was the fix)
builder.Services.AddSingleton(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("Cosmos");
    return new CosmosClient(connectionString);
});

// Add Entity Framework for any EF Core needs
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("TravelExpenseDb"));

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITripService, TripService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddSingleton<IContainerNameResolver, ContainerNameResolver>();
builder.Services.AddScoped<IBlobService, BlobService>();
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// If you’re behind Azure App Service’s proxy, forward headers is nice to have:
app.Use((ctx, next) =>
{
    ctx.Request.Scheme = "https"; // ensures callbacks build as https on Azure
    return next();
});

app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();