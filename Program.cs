using Azure.Storage.Blobs;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;
using TravelExpenseTracker.Data;
using TravelExpenseTracker.Domain;
using TravelExpenseTracker.Models;
using TravelExpenseTracker.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

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
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();