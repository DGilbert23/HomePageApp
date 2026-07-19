using HomePageApp.Core.Interfaces;
using HomePageApp.Infrastructure;
using HomePageApp.Infrastructure.FileSystem;
using HomePageApp.Infrastructure.Identity;
using HomePageApp.Infrastructure.Repositories;
using HomePageApp.Web.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddDbContextFactory<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.User.RequireUniqueEmail = true;

        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services
    .AddAuthentication(options =>
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

builder.Services
    .AddAuthentication()
    .AddCookie("GoogleAuthCookie", options =>
    {
        options.Cookie.Name = "HomePageApp.Google";
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        options.SlidingExpiration = true;
    })
    .AddGoogle("Google", options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
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

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();
}
);

builder.Services.AddScoped<IToDoRepository, EfToDoRepository>();
builder.Services.AddScoped<IBillTrackerRepository, EfBillTrackerRepository>();
builder.Services.AddScoped<IUserAccountService, UserAccountService>();

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddHttpContextAccessor();

var path = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "uploads", builder.Configuration["StorageSettings:ScratchPadPath"] ?? "");
builder.Services.AddTransient<IScratchPadStorage>(provider => new ScratchPadStorage(path));

var keysDirectory = new DirectoryInfo(@"C:\ProgramData\HomePageApp\DataProtectionKeys");
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(keysDirectory)
    .SetApplicationName("HomePageApp");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseForwardedHeaders();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapStaticAssets()
    .AllowAnonymous();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

#region Mapping endpoints for ASP.NET Identity Auth

app.MapPost("/account/login", async (
    HttpContext context,
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager) =>
{
    var form = await context.Request.ReadFormAsync();

    var email = form["Email"].ToString();
    var password = form["Password"].ToString();

    //Treat bad username (no user found) the same failed authentication.
    var user = await userManager.FindByEmailAsync(email);
    if (user == null)
        return Results.Redirect("/login?error=invalid");

    var result = await signInManager.PasswordSignInAsync(
        user,
        password,
        isPersistent: true,
        lockoutOnFailure: false);

    if (result.Succeeded)
        return Results.Redirect("/");
    else
        return Results.Redirect("/login?error=invalid");
})
    .AllowAnonymous();

app.MapPost("/account/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(IdentityConstants.ApplicationScheme);

    return Results.Redirect("/login");
})
    .AllowAnonymous();

#endregion

#region Mapping endpoints for GoogleAuth for CalendarWidget
app.MapGet("calendarwidget/account/login", async (HttpContext httpContext) =>
{
    var properties = new AuthenticationProperties
    {
        RedirectUri = "/",

        IsPersistent = true,
        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(14),
        AllowRefresh = true
    };

    await httpContext.ChallengeAsync("Google", properties);
});

app.MapGet("calendarwidget/account/logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync("GoogleAuthCookie");
    httpContext.Response.Redirect("/");
});
#endregion

#region Endpoint for auth token debugging
app.MapGet("api/debug-tokens", async (HttpContext httpContext) =>
{
    // Force AuthenticateAsync against the widget scheme to extract Google tokens safely
    var authResult = await httpContext.AuthenticateAsync("GoogleAuthCookie");

    var accessToken = authResult.Properties?.GetTokenValue("access_token");
    var refreshToken = authResult.Properties?.GetTokenValue("refresh_token");
    var expiresAt = authResult.Properties?.GetTokenValue("expires_at");

    return Results.Ok(new
    {
        IsWidgetUserAuthenticated = authResult.Succeeded,
        HasAccessToken = !string.IsNullOrEmpty(accessToken),
        AccessTokenPreview = accessToken != null && accessToken.Length > 10
            ? accessToken.Substring(0, 10) + "..."
            : accessToken,

        HasRefreshToken = !string.IsNullOrEmpty(refreshToken),
        RefreshTokenPreview = refreshToken != null && refreshToken.Length > 10
            ? refreshToken.Substring(0, 10) + "..."
            : "MISSING",

        ExpiresAtRawString = expiresAt,
        ParsedUtcTime = DateTimeOffset.TryParse(expiresAt, out var dt) ? dt.ToString("u") : "Failed to parse",
        CurrentUtcTime = DateTimeOffset.UtcNow.ToString("u")
    });
});
#endregion

app.Run();