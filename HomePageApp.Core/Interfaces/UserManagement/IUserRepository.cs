using HomePageApp.Core.Models.UserManagement;

namespace HomePageApp.Core.Interfaces.UserManagement
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserInfo>> GetAllAsync();

        Task<UserInfo?> GetByIdAsync(int id);

        Task<UserInfo?> GetByIdentityUserIdAsync(Guid id);

        Task UpdateAsync(UserInfo user);

        Task DeleteAsync(Guid id);

        Task UpdateRolesAsync(Guid identityUserId, IEnumerable<string> roles);
    }
}
