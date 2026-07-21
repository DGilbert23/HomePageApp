using HomePageApp.Core.Models;

namespace HomePageApp.Core.Interfaces;

public interface IUserAccountService
{
    Task<UserProfile> CreateUserAsync(string email, string password, string firstName, string lastName);

    Task<bool> CheckPasswordAsync(string email, string password);

    Task<Guid> GetCurrentUserId();

    Task<int> GetCurrentUserProfileId();

    Task<UserProfile?> GetUserProfile(Guid identityId);

    Task SaveGoogleConnectionAsync(GoogleConnectionInfo googleConnection);

    Task<GoogleConnectionInfo?> GetGoogleConnectionAsync();

    Task RemoveGoogleConnectionAsync();
}