using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HomePageApp.Core.Interfaces;
using HomePageApp.Infrastructure.Services.WeatherApi;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register the Weather Service HttpClient with Base URL
        services.AddHttpClient<IWeatherApiService, WeatherApiService>(client =>
        {
            var baseUrl = configuration["WeatherApiSettings:BaseUrl"];
            client.BaseAddress = new Uri(baseUrl ?? "");
        });

        return services;
    }
}
