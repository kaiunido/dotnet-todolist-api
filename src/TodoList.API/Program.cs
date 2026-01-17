using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using TodoList.API.Data;
using TodoList.API.Middlewares;
using TodoList.API.Services;
using TodoList.API.Validators;

DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = $"Server={Environment.GetEnvironmentVariable("DB_HOST")};" +
                       $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
                       $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                       $"Uid={Environment.GetEnvironmentVariable("DB_USER")};" +
                       $"Pwd={Environment.GetEnvironmentVariable("DB_PASSWORD")};" +
                       "AllowPublicKeyRetrieval=True;";

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options => options.UseMySQL(
    connectionString
));

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

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

app.UseAuthentication();
app.UseMiddleware<TokenSessionMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
