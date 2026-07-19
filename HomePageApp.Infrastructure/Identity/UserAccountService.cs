using HomePageApp.Core.Interfaces;
using HomePageApp.Core.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HomePageApp.Infrastructure.Identity;

public class UserAccountService : IUserAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IDbContextFactory<AppDbContext> _dbFactory;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public UserAccountService(UserManager<ApplicationUser> userManager, IDbContextFactory<AppDbContext> dbFactory, AuthenticationStateProvider authenticationStateProvider)
    {
        _userManager = userManager;
        _dbFactory = dbFactory;
        _authenticationStateProvider = authenticationStateProvider;
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

    public async Task<int> GetCurrentUserProfileId()
    {
        var identityId = await GetCurrentUserId();
        var profile = await GetUserProfile(identityId);

        if (profile == null)
            throw new InvalidOperationException("Authenticated user has no UserProfile.");

        return profile.Id;
    }

    public async Task<Guid> GetCurrentUserId()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        var id = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (id == null)
            throw new InvalidOperationException("No authenticated user found");

        return Guid.Parse(id);
    }

    public async Task<UserProfile?> GetUserProfile(Guid identityId)
    {
        using var context = _dbFactory.CreateDbContext();
        return await context.UserProfiles.Where(p => p.IdentityUserId == identityId).FirstOrDefaultAsync();
    }

    
}