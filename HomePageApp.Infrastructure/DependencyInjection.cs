using HomePageApp.Core.Interfaces;
using HomePageApp.Infrastructure.Services.Google.GoogleAuth;
using HomePageApp.Infrastructure.Services.WeatherApi;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
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

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = "Google";
        })
        .AddCookie()
        .AddGoogle("Google", options =>
        {
            options.ClientId = configuration["Authentication:Google:ClientId"]!;
            options.ClientSecret = configuration["Authentication:Google:ClientSecret"]!;
            options.SaveTokens = true;
            options.Scope.Add("https://www.googleapis.com/auth/calendar");
            options.AccessType = "offline";
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
