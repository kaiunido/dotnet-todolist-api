using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using TodoList.API.Data;
using TodoList.API.Services;
using TodoList.API.Validators;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);
var rawConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var connectionString = Environment.ExpandEnvironmentVariables(rawConnectionString ?? "");

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(
    connectionString, ServerVersion.AutoDetect(connectionString)
));

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services
    .AddValidatorsFromAssemblyContaining<IAssemblyMarker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
