using HomePageApp.Core.Interfaces;
using HomePageApp.Infrastructure.Identity;
using HomePageApp.Infrastructure.Services.Google.GoogleAuth;
using HomePageApp.Infrastructure.Services.WeatherApi;
using Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IWeatherApiService, WeatherApiService>(client =>
        {
            var baseUrl = configuration["WeatherApiSettings:BaseUrl"];
            client.BaseAddress = new Uri(baseUrl ?? "");
        });

        services.AddScoped<IUserAccountService, UserAccountService>();

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });

        services.AddAuthorizationBuilder();

        services.AddHttpClient<IGoogleAuthService, GoogleAuthService>(client =>
        {
            client.BaseAddress = new Uri(configuration["GoogleApi:BaseUrl"] ?? string.Empty);
        });

        services.AddHttpClient<ICalendarApiService, CalendarApiService>();

        return services;
    }
}
