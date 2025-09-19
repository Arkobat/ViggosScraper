using System.Text.Json;
using System.Text.Json.Serialization;
using DrikDatoApp.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.OpenApi.Models;
using ViggosScraper.Database;
using ViggosScraper.Middleware;
using ViggosScraper.Model;
using ViggosScraper.Service;
using ViggosScraper.Service.Background;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var services = builder.Services;
var environment = builder.Environment;


var dbConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ??
                         configuration.GetConnectionString("Postgres") ??
                         throw new Exception("No connection string found");

services
    .AddLogging(l => l.AddConsole())
    .AddHostedService<BackgroundUserScraper>()
    .AddDrikDatoService()
    .AddSingleton(new MemoryCache(new MemoryCacheOptions()))
    .AddScoped(typeof(ICache<,>), typeof(Cache<,>))
    .AddScoped<ExceptionMiddleware>()
    .AddScoped<HttpClient>()
    .AddScoped<LoginService>()
    .AddScoped<SymbolService>()
    .AddScoped<UserScraper>()
    .AddScoped<NewUserService>()
    .AddDbContext<ViggosDb>(options => options.UseNpgsql(dbConnectionString))
    ;

services
    .AddOptions()
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

        var converters = options.JsonSerializerOptions.Converters;
        converters.Add(new JsonStringEnumConverter());
    });

services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo {Title = "Viggos Scraper", Version = "v2"});
    
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,

        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                },
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

using var app = builder.Build();

app.UseCors(policy =>
{
    policy
        .AllowCredentials()
        .SetIsOriginAllowed(_ => true)
        .AllowAnyMethod()
        .AllowAnyHeader();
});

app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    //app.UseDeveloperExceptionPage();
}

//app.UseHttpsRedirection();
app.MapControllers();

app.Run();