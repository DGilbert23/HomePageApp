using HomePageApp.Core.Interfaces;
using HomePageApp.Infrastructure.Services.GoogleAuth;
using HomePageApp.Infrastructure.Services.WeatherApi;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        // Configure Local App Authentication using Google
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
            options.SaveTokens = true; // Required to capture the access_token for the outbound API
        });

        services.AddAuthorizationBuilder();

        // 2. Register the Outbound Client pointing to the external server
        services.AddHttpClient<IExternalApiService, GoogleAuthService>(client =>
        {
            client.BaseAddress = new Uri(configuration["GoogleApi:BaseUrl"] ?? "");
        });


        return services;
    }
}
