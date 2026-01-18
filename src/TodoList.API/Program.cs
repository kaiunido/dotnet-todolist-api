using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using TodoList.API.Configurations;
using TodoList.API.Data;
using TodoList.API.Handlers;
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
builder.Services.AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetSection(JwtSettings.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options => options.UseMySQL(
    connectionString
));

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var settings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>();

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = settings?.Issuer,
            ValidAudience = settings?.Audience,
            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(settings?.Key ?? ""))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services
    .AddValidatorsFromAssemblyContaining<IAssemblyMarker>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseMiddleware<TokenSessionMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
