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

// Email verification / confirmation, Password reset, user login, user register .net 6 Patrick God
// https://www.youtube.com/watch?v=2Q9Uh-5O8Sk&ab_channel=PatrickGod

// .net 7 Web Api course: Web API Structure (Clien -> Controller -> Service -> Database), DTO, GET, POST, PUT, DELETE
// https://www.youtube.com/watch?v=9zJn3a7L1uE&ab_channel=PatrickGod

// ak nebude stacit horne video tak tu je: vytvorenie repository/service inject a registracia repository/service
//https://www.youtube.com/watch?v=Wiy54682d1w&ab_channel=PatrickGod

// 1:n 1:1 n:n ... na prikaz pre build a vyhodnotenie ci je to co to chcem alebo idem starou cestou
// https://www.youtube.com/watch?v=V0UF4vEMlhQ&ab_channel=PatrickGod

// 1. dotnet ef migrations add NewDatabaseLayout_v3
// 2. dotnet ef database update
