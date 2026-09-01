using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TravelPlanner.Domain.Interfaces;
using TravelPlanner.Infrastructure.Persistence;
using TravelPlanner.Infrastructure.Repositories;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using TravelPlanner.Infrastructure.ExternalServices.GoogleMaps;
using TravelPlanner.Infrastructure.ExternalServices.AI;
using TravelPlanner.Infrastructure.ExternalServices.Weather;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddControllers();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ����� �������� ������ �-Dependency Injection
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITripRepository, TripRepository>();
builder.Services.AddScoped<ISavedPlaceRepository, SavedPlaceRepository>();
// ����� ������:

//1.
// ����� ��� ����� ������: "������� ���� IMapsService, ��� �� �� GoogleMapsService ������� �� ������ �-HttpClient"
// ����� ����� ����
builder.Services.AddHttpClient<IMapsService, GoogleMapsService>();

//2.
// ���� ��� ����� ������
builder.Services.AddScoped<IMapsService, GoogleMapsService>();

//����� ������
//1.
builder.Services.AddHttpClient<IAIService, GeminiService>();
//2.
builder.Services.AddScoped<IAIService, GeminiService>();

// ����� ����� ��� ������
builder.Services.AddHttpClient<IWeatherService, WeatherService>();
builder.Services.AddScoped<IWeatherService, WeatherService>();

// ����� ��� ����� ������ �� �����
builder.Services.AddScoped<TravelPlanner.Application.Common.Interfaces.IJwtTokenGenerator, TravelPlanner.Infrastructure.Authentication.JwtTokenGenerator>();

// ����� MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(TravelPlanner.Application.Features.Users.Commands.RegisterUser.RegisterUserCommand).Assembly));

builder.Services.AddEndpointsApiExplorer();

// ����� Swagger �� ������ ������ ���� (������)
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "TravelPlanner API", Version = "v1" });

    // ����� ����� ������ (Security Definition)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "�� �����: Bearer {your_token}"
    });

    // ���� ����� ������ �� �� �-Endpoints
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// ����� ����� (Authentication)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),


    //// ����� ������� ������ ����� �-NameId:
    //    NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
    //    RoleClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role"
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("Authentication Failed: " + context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token Validated Successfully!");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngular");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();








