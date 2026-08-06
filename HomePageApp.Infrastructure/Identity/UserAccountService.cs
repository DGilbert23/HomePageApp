using HomePageApp.Core.Contracts.Accounts;
using HomePageApp.Core.Interfaces.Identity;
using HomePageApp.Core.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HomePageApp.Infrastructure.Identity;

public class UserAccountService : IUserAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IDbContextFactory<AppDbContext> _dbFactory;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserAccountService(
        UserManager<ApplicationUser> userManager,
        IDbContextFactory<AppDbContext> dbFactory,
        AuthenticationStateProvider authenticationStateProvider,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _dbFactory = dbFactory;
        _authenticationStateProvider = authenticationStateProvider;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<UserProfile> CreateUserAsync(string email, string password, string firstName, string lastName)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        var profile = new UserProfile
        {
            IdentityUserId = user.Id
        };

        var context = _dbFactory.CreateDbContext();
        context.UserProfiles.Add(profile);

        await context.SaveChangesAsync();

        return profile;
    }

    public async Task<bool> CheckPasswordAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            return false;
        }

        return await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task<int> GetCurrentUserProfileIdAsync()
    {
        var identityId = await GetCurrentUserIdAsync();
        var profile = await GetUserProfileAsync(identityId);

        if (profile == null)
            throw new InvalidOperationException("Authenticated user has no UserProfile.");

        return profile.Id;
    }

    public async Task<Guid> GetCurrentUserIdAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        var id = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (id == null)
            throw new InvalidOperationException("No authenticated user found");

        return Guid.Parse(id);
    }

    public Guid GetCurrentUserIdFromHttpContext()
    {
        var id = _httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        if (id == null)
            throw new InvalidOperationException("No authenticated user found");

        return Guid.Parse(id);
    }

    public async Task<UserProfile?> GetUserProfileAsync(Guid identityId)
    {
        using var context = _dbFactory.CreateDbContext();
        return await context.UserProfiles.Where(p => p.IdentityUserId == identityId).FirstOrDefaultAsync();
    }

    public async Task<AccountOperationResult> ChangeUserPasswordAsync(string currentPassword,  string newPassword)
    {
        var user = await _userManager.FindByIdAsync((await GetCurrentUserIdAsync()).ToString());

        if (user == null)
            throw new InvalidOperationException("No current user found. Unable to change password.");

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

        if (result.Succeeded)
            return AccountOperationResult.Success();
        else
            return AccountOperationResult.Failure(result.Errors.Select(e => e.Description ?? "Unknown Error").ToArray());
    }

    public async Task SaveGoogleConnectionAsync(
    GoogleConnectionInfo googleConnection)
    {
        var userId = GetCurrentUserIdFromHttpContext();

        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            throw new InvalidOperationException(
                "Authenticated user not found.");
        }

        user.GoogleRefreshToken = googleConnection.EncryptedRefreshToken;
        user.GoogleUserId = googleConnection.GoogleUserId;
        user.GoogleEmail = googleConnection.GoogleEmail;
        user.GoogleFirstName = googleConnection.GoogleFirstName;
        user.GoogleLastName = googleConnection.GoogleLastName;
        user.GoogleConnectedUtc = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(", ",
                    result.Errors.Select(e => e.Description)));
        }
    }

    public async Task<GoogleConnectionInfo?> GetGoogleConnectionAsync()
    {
        var userId = await GetCurrentUserIdAsync();

        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            return null;
        }

        if (string.IsNullOrEmpty(user.GoogleRefreshToken))
        {
            return null;
        }

        return new GoogleConnectionInfo
        {
            EncryptedRefreshToken = user.GoogleRefreshToken,
            GoogleUserId = user.GoogleUserId ?? "",
            GoogleEmail = user.GoogleEmail ?? "",
            GoogleFirstName = user.GoogleFirstName,
            GoogleLastName = user.GoogleLastName
        };
    }

    public async Task RemoveGoogleConnectionAsync()
    {
        var userId = await GetCurrentUserIdAsync();

        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            throw new InvalidOperationException(
                "Authenticated user not found.");
        }

        user.GoogleRefreshToken = null;
        user.GoogleUserId = null;
        user.GoogleEmail = null;
        user.GoogleFirstName = null;
        user.GoogleLastName = null;
        user.GoogleConnectedUtc = null;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(", ",
                    result.Errors.Select(e => e.Description)));
        }
    }
}