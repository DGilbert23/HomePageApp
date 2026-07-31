using HomePageApp.Core.Contracts.Accounts;
using HomePageApp.Core.Models;

namespace HomePageApp.Core.Interfaces;

public interface IUserAccountService
{
    Task<UserProfile> CreateUserAsync(string email, string password, string firstName, string lastName);

    Task<bool> CheckPasswordAsync(string email, string password);

    Task<Guid> GetCurrentUserIdAsync();

    Task<int> GetCurrentUserProfileIdAsync();

    Task<UserProfile?> GetUserProfileAsync(Guid identityId);

    Task<AccountOperationResult> ChangeUserPasswordAsync(string currentPassword, string newPassword);

    Task SaveGoogleConnectionAsync(GoogleConnectionInfo googleConnection);

    Task<GoogleConnectionInfo?> GetGoogleConnectionAsync();

    Task RemoveGoogleConnectionAsync();
}