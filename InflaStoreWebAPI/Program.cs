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
using InflaStoreWebAPI.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using InflaStoreWebAPI.Services.ClubCardService;
using InflaStoreWebAPI.Services.CategoryService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddDbContext<DataContext>();

builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwt =>
{
    //var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JwtConfig:Secret").Value); // orig vo videu https://youtu.be/Y-MjCw6thao?t=2126
    var key = Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JwtConfig:Secret").Value!);

    jwt.SaveToken = true;
    jwt.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false, // for development common isue and problem with https https://youtu.be/Y-MjCw6thao?t=2307 vysvetluje preco ale v produkci to MUSI BYT TRUE !!! 
        ValidateAudience = false, // to iste ako vyssie
        RequireExpirationTime = false, // vysvetlenie preco false https://youtu.be/Y-MjCw6thao?t=2368 ---> tu refresh token https://www.youtube.com/watch?v=2_H0Zj-C8EM&ab_channel=MohamadLawand
        ValidateLifetime = true // validating lifetime of token we are sending, if we send token with existence of 1 minute he can calculate if it is already expired or not
    };
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IItemPriceService, ItemPriceService>();
builder.Services.AddScoped<IItemsService, ItemsService>();
builder.Services.AddScoped<INavigationShopDatas, NavigationShopDatas>();
builder.Services.AddScoped<IUnitsService, UnitService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IClubCardService, ClubCardService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(); // ked chcem aj cez url ziskat obsah vo wwwroot priecinku

// problem s 
//app.UseFileServer(new FileServerOptions
//{
//    FileProvider = new PhysicalFileProvider
//    (
//        Path.Combine(Directory.GetCurrentDirectory(), "StaticFile")
//    ),

//    RequestPath = "/StaticFile"
//});

#if !DEBUG
app.UseHttpsRedirection();
#endif

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// JWT Authentication a hovori ze Azure Key Vault existuje a ma video s nejakym inym riesenim
//https://youtu.be/mgeuh8k3I4g?t=249

// dotnet ef migrations add "Sprava"
// dotnet ef database update

// Patrick God asi najlepsie s tymto pokracovat https://www.youtube.com/watch?v=TDY_DtTEkes&ab_channel=PatrickGod Authentication - Authorization with JWT + Role
// Patrick God Refresh tokens https://www.youtube.com/watch?v=HGIdAn2h8BA&ab_channel=PatrickGod
// Patrick God read JWT Authorization Claims of a User (from a JSON Web Token) https://www.youtube.com/watch?v=fhWIkbF18lM&ab_channel=PatrickGod

// Use HttpClient correct way in API https://www.youtube.com/watch?v=Z6Y2adsMnAA&ab_channel=NickChapsas

// Mohamad refresh token https://www.youtube.com/watch?v=2_H0Zj-C8EM&ab_channel=MohamadLawand