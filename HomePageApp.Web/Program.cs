using HomePageApp.Core.Interfaces.Identity;
using HomePageApp.Core.Models.Google;
using HomePageApp.Infrastructure.Identity;
using HomePageApp.Infrastructure.Services.Google.GoogleAuth;
using HomePageApp.Web.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
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
    .AddInteractiveServerRenderMode()
    .AllowAnonymous();


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
        RedirectUri = "/calendarwidget/account/callback"
    };

    await httpContext.ChallengeAsync("Google", properties);
});

app.MapGet("calendarwidget/account/logout", async (
    HttpContext httpContext,
    IUserAccountService userAccountService) =>
{
    await userAccountService.RemoveGoogleConnectionAsync();

    await httpContext.SignOutAsync("GoogleAuthCookie");

    return Results.Redirect("/");
});

app.MapGet("calendarwidget/account/callback", async (
    HttpContext httpContext,
    IUserAccountService userAccountService,
    GoogleTokenProtector tokenProtector) =>
{
    var result = await httpContext.AuthenticateAsync("GoogleAuthCookie");

    if (!result.Succeeded)
    {
        return Results.Redirect("/?googleError=authenticationFailed");
    }

    var refreshToken = result.Properties?
        .GetTokenValue("refresh_token");

    if (string.IsNullOrEmpty(refreshToken))
    {
        return Results.Redirect("/?googleError=noRefreshToken");
    }


    var encryptedRefreshToken =
        tokenProtector.Protect(refreshToken);


    var googleUserId =
        result.Principal?.FindFirst("sub")?.Value ?? "";

    var googleEmail =
        result.Principal?.FindFirst(
            System.Security.Claims.ClaimTypes.Email)?.Value ?? "";

    var googleFirstName =
        result.Principal?.FindFirst(
            System.Security.Claims.ClaimTypes.GivenName)?.Value;

    var googleLastName =
        result.Principal?.FindFirst(
            System.Security.Claims.ClaimTypes.Surname)?.Value;


    await userAccountService.SaveGoogleConnectionAsync(
        new GoogleConnectionInfo
        {
            EncryptedRefreshToken = encryptedRefreshToken,
            GoogleUserId = googleUserId,
            GoogleEmail = googleEmail,
            GoogleFirstName = googleFirstName,
            GoogleLastName = googleLastName
        });


    await httpContext.SignOutAsync("GoogleAuthCookie");


    return Results.Redirect("/");
});
#endregion

app.Run();