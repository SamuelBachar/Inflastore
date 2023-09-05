global using Microsoft.EntityFrameworkCore;
global using InflaStoreWebAPI.Data;
global using InflaStoreWebAPI.Services.EmailService;
global using InflaStoreWebAPI.Services.UserService;

global using SharedTypesLibrary.Models.API;
global using SharedTypesLibrary.Models.API.DatabaseModels;
global using SharedTypesLibrary.Models.API.ServiceResponseModel;
global using SharedTypesLibrary.DTOs.API;
using Microsoft.Extensions.FileProviders;
using InflaStoreWebAPI.Services.ItemPriceService;
using InflaStoreWebAPI.Services.ItemsService;
using InflaStoreWebAPI.Services.NavigationShopDatasService;
using InflaStoreWebAPI.Services.UnitsService;
using InflaStoreWebAPI.Services.CompanyService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddDbContext<DataContext>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IItemPriceService, ItemPriceService>();
builder.Services.AddScoped<IItemsService, ItemsService>();
builder.Services.AddScoped<INavigationShopDatas, NavigationShopDatas>();
builder.Services.AddScoped<IUnitsService, UnitService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseStaticFiles(); // ked chcem aj cez url ziskat obsah vo wwwroot priecinku

app.UseFileServer(new FileServerOptions
{
    FileProvider = new PhysicalFileProvider
    (
        Path.Combine(Directory.GetCurrentDirectory(), "StaticFile")
    ),

    RequestPath = "/StaticFile"
});

#if !DEBUG
app.UseHttpsRedirection();
#endif

app.UseAuthorization();

app.MapControllers();

app.Run();

// JWT Authentication a hovori ze Azure Key Vault existuje a ma video s nejakym inym riesenim
//https://youtu.be/mgeuh8k3I4g?t=249