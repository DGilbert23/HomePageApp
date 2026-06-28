using HomePageApp.Core.Interfaces;
using HomePageApp.Infrastructure;
using HomePageApp.Infrastructure.Repositories;
using HomePageApp.Web.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddDbContextFactory<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IToDoRepository, EfToDoRepository>();

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddHttpContextAccessor();

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
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.MapGet("account/login", async (HttpContext httpContext) =>
{
    await httpContext.ChallengeAsync("Google", new AuthenticationProperties { RedirectUri = "/" });
});

// Add Logout Route Endpoint mapping
app.MapGet("account/logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    httpContext.Response.Redirect("/");
});

app.MapGet("api/debug-tokens", async (HttpContext httpContext) =>
{
    var accessToken = await httpContext.GetTokenAsync("access_token");
    var refreshToken = await httpContext.GetTokenAsync("refresh_token");
    var expiresAt = await httpContext.GetTokenAsync("expires_at");

    return Results.Ok(new
    {
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

app.Run();
