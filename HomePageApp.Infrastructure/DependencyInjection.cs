using HomePageApp.Core.Interfaces.BillTracker;
using HomePageApp.Core.Interfaces.Calendar;
using HomePageApp.Core.Interfaces.Google;
using HomePageApp.Core.Interfaces.Identity;
using HomePageApp.Core.Interfaces.ScratchPad;
using HomePageApp.Core.Interfaces.ToDoList;
using HomePageApp.Core.Interfaces.UserManagement;
using HomePageApp.Core.Interfaces.WeatherApi;
using HomePageApp.Infrastructure;
using HomePageApp.Infrastructure.DemoInfrastructure.Repositories;
using HomePageApp.Infrastructure.DemoInfrastructure.Services;
using HomePageApp.Infrastructure.FileSystem;
using HomePageApp.Infrastructure.Identity;
using HomePageApp.Infrastructure.Repositories;
using HomePageApp.Infrastructure.Services.Google.GoogleAuth;
using HomePageApp.Infrastructure.Services.WeatherApi;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        //DbContext
        services.AddDbContextFactory<AppDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"));
        });

        //Identity
        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.User.RequireUniqueEmail = true;

            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;

            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
            options.Lockout.AllowedForNewUsers = true;
        })
                .AddRoles<IdentityRole<Guid>>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

        //Authentication
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
            options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
        })
                .AddIdentityCookies(options =>
                {
                    options.ApplicationCookie!.Configure(cookie =>
                    {
                        cookie.LoginPath = "/login";
                    });
                });

        services.AddAuthentication()
                .AddCookie("GoogleAuthCookie", options =>
                {
                    options.Cookie.Name = "HomePageApp.Google";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                    options.SlidingExpiration = false;
                })
                .AddGoogle("Google", options =>
                {
                    options.ClientId = configuration["Authentication:Google:ClientId"]!;
                    options.ClientSecret = configuration["Authentication:Google:ClientSecret"]!;
                    options.SaveTokens = true;
                    options.Scope.Add("https://www.googleapis.com/auth/calendar");
                    options.SignInScheme = "GoogleAuthCookie";
                    options.Events =
                        new Microsoft.AspNetCore.Authentication.OAuth.OAuthEvents
                        {
                            OnRedirectToAuthorizationEndpoint = context =>
                            {
                                context.Response.Redirect(
                                    context.RedirectUri + "&access_type=offline&prompt=consent");

                                return Task.CompletedTask;
                            }
                        };
                });

        //Authorization
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
        }
                );

        
        //Repositories
        services.AddScoped<IToDoRepository, EfToDoRepository>();
        services.AddKeyedScoped<IBillTrackerRepository, EfBillTrackerRepository>("production");
        services.AddKeyedScoped<IBillTrackerRepository, DemoBillTrackerRepository>("demo");
        services.AddScoped<IUserAccountService, UserAccountService>();
        services.AddScoped<IUserRepository, EfUserRepository>();
        

        //ScratchPad
        services.AddTransient<IScratchPadStorage, ScratchPadStorage>();

        //Google
        services.AddSingleton<GoogleTokenProtector>();

        //HttpContextAccessor
        services.AddHttpContextAccessor();

        //HttpClients
        services.AddHttpClient<IWeatherApiService, WeatherApiService>(client =>
        {
            var baseUrl = configuration["WeatherApiSettings:BaseUrl"];
            client.BaseAddress = new Uri(baseUrl ?? "");
        });

        services.AddHttpClient<IGoogleAuthService, GoogleAuthService>(client =>
        {
            client.BaseAddress = new Uri(configuration["GoogleApi:BaseUrl"] ?? string.Empty);
        });

        services.AddHttpClient<ICalendarApiService, CalendarApiService>();
        services.AddKeyedScoped<ICalendarApiService, DemoCalendarApiService>("demo");

        //ForwardedHeaders
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });

        //DataProtection      
        var keysDirectory = new DirectoryInfo(@"C:\ProgramData\HomePageApp\DataProtectionKeys");
        services.AddDataProtection()
                .PersistKeysToFileSystem(keysDirectory)
                .SetApplicationName("HomePageApp");

        return services;
    }
}
