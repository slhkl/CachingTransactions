using Data.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Service;
using System.Runtime.CompilerServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ProductService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/InMemoryCache", ([FromServices] IMemoryCache cache) =>
{
    string index = "CachedDate";
    if (cache.TryGetValue(index, out DateTime cachedDate))
        return cachedDate;

    cachedDate = DateTime.Now;

    MemoryCacheEntryOptions memoryCacheEntryOptions = new MemoryCacheEntryOptions();
    memoryCacheEntryOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
    cache.Set(index, cachedDate, memoryCacheEntryOptions);

    return cachedDate;
});

app.MapGet("/GetAll", ([FromServices] IMemoryCache cache, [FromServices] ProductService service) =>
{
    string index = "ProductList";
    if (cache.TryGetValue(index, out List<Product> productList))
        return productList;

    productList = service.GetProductList();

    MemoryCacheEntryOptions memoryCacheEntryOptions = new MemoryCacheEntryOptions();
    memoryCacheEntryOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
    cache.Set(index, new List<Product>(productList), memoryCacheEntryOptions);

    return productList;
});

app.MapPost("/Add", ([FromServices] IMemoryCache cache, [FromServices] ProductService service, Product product) =>
{
    service.AddProduct(product);

    string index = "ProductList";
    cache.Remove(index);
});

app.Run();