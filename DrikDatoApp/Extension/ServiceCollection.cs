using Microsoft.Extensions.DependencyInjection;
using DrikDatoApp.Service;

namespace DrikDatoApp.Extension;

public static class ServiceCollection
{
    public static IServiceCollection AddDrikDatoService(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddScoped<IDrikDatoService, DrikDatoService>();
        
        return services;
    }
}