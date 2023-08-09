using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ViggosScraper.Database;
using ViggosScraper.Middleware;
using ViggosScraper.Service;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var services = builder.Services;
var environment = builder.Environment;


var dbConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ??
                         configuration.GetConnectionString("Postgres") ??
                         throw new Exception("No connection string found");

services
    .AddScoped<ExceptionMiddleware>()
    .AddScoped<HttpClient>()
    .AddScoped<HighscoreScraper>()
    .AddScoped<LoginService>()
    .AddScoped<SearchScraper>()
    .AddScoped<SymbolService>()
    .AddScoped<UserScraper>()
    .AddDbContext<ViggosDb>(options => options.UseNpgsql(dbConnectionString));

services
    .AddOptions()
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

        var converters = options.JsonSerializerOptions.Converters;
        converters.Add(new JsonStringEnumConverter());
    });

services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo() {Title = "Viggos Scraper", Version = "v1"}); });

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