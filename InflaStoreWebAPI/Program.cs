global using Microsoft.EntityFrameworkCore;
global using InflaStoreWebAPI.Models;
global using InflaStoreWebAPI.Data;
global using InflaStoreWebAPI.Services.EmailService;
global using InflaStoreWebAPI.Models.ServiceResponseModel;
global using InflaStoreWebAPI.DTOs;
global using InflaStoreWebAPI.Services.UserService;

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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
