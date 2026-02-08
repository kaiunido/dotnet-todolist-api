using System.Text;
using System.Text.Json;
using DotNetEnv;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using TodoList.API.Configurations;
using TodoList.API.Data;
using TodoList.API.DTOs;
using TodoList.API.Handlers;
using TodoList.API.Middlewares;
using TodoList.API.Services;
using TodoList.API.Validators;

Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetSection(JwtSettings.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();

builder.Services.AddControllers();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var problem = new ValidationProblemDetails(context.ModelState);

        return new BadRequestObjectResult(new ValidationErrorResponseDto
        {
            Title = problem.Title ?? "One or more validation errors occurred.",
            Errors = problem.Errors
        });
    };
});

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new OpenApiComponents();

        document.Components.SecuritySchemes ??=
            new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes["BearerAuth"] =
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Description = "Add your JWT (Bearer <token>)"
            };

        document.Security ??= new List<OpenApiSecurityRequirement>();

        var schemeRef = new OpenApiSecuritySchemeReference("BearerAuth");
        document.Security.Add(new OpenApiSecurityRequirement
        {
            [schemeRef] = new List<string>()
        });

        return Task.CompletedTask;
    });
});

if (!builder.Environment.IsEnvironment("Testing"))
{
    var connectionString =
        $"Server={Environment.GetEnvironmentVariable("DB_HOST")};" +
        $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
        $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
        $"Uid={Environment.GetEnvironmentVariable("DB_USER")};" +
        $"Pwd={Environment.GetEnvironmentVariable("DB_PASSWORD")};" +
        "AllowPublicKeyRetrieval=True;";

    builder.Services.AddDbContext<AppDbContext>(options => options.UseMySQL(
        connectionString
    ));
}

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var settings = builder.Configuration.GetSection(JwtSettings.SectionName)
            .Get<JwtSettings>();

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

        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var payload = JsonSerializer.Serialize(new
                {
                    title = "Unauthorized",
                    message = "Missing or invalid access token."
                });

                return context.Response.WriteAsync(payload);
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITaskListService, TaskListService>();

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
    app.MapScalarApiReference(options => options
        .AddPreferredSecuritySchemes("BearerAuth")
        .AddHttpAuthentication("BearerAuth", auth => { })
    );
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseMiddleware<TokenSessionMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();