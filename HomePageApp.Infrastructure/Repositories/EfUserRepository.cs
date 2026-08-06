using HomePageApp.Core.Interfaces.UserManagement;
using HomePageApp.Core.Models.UserManagement;
using HomePageApp.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HomePageApp.Infrastructure.Repositories
{
    public class EfUserRepository : IUserRepository
    {
        private readonly IDbContextFactory<AppDbContext> _dbFactory;
        private readonly UserManager<ApplicationUser> _userManager;

        public EfUserRepository(IDbContextFactory<AppDbContext> contextFactory, UserManager<ApplicationUser> userManager)
        {
            _dbFactory = contextFactory;
            _userManager = userManager;
        }

        public async Task<IEnumerable<UserInfo>> GetAllAsync()
        {
            using var context = await _dbFactory.CreateDbContextAsync();

            var results = new List<UserInfo>();
            var users = await context.Users.ToListAsync();

            foreach (var user in users)
            {
                var profile = await context.UserProfiles.FirstOrDefaultAsync(u => u.IdentityUserId == user.Id);
                var roles = await _userManager.GetRolesAsync(user);

                results.Add(new UserInfo
                {
                    Id = profile?.Id ?? 0,
                    IdentityUserId = user.Id,
                    Email = user.Email ?? string.Empty,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    EmailConfirmed = user.EmailConfirmed,
                    LockoutEnabled = user.LockoutEnabled,
                    LockoutEnd = user.LockoutEnd,
                    Roles = roles.ToList()
                });
            }

            return results;
        }

        public async Task<UserInfo?> GetByIdAsync(int id)
        {
            using var context = await _dbFactory.CreateDbContextAsync();

            var profile = await context.UserProfiles.FirstOrDefaultAsync(u => u.Id == id);
            if (profile == null)
                throw new InvalidOperationException("Unable to find UserProfile for ID " + id);

            var user = await _userManager.FindByIdAsync(profile.IdentityUserId.ToString());
            if (user == null)
                throw new InvalidOperationException("Unable to find User for IdentityUserId " + profile.IdentityUserId);

            var roles = await _userManager.GetRolesAsync(user);

            return new UserInfo
            {
                Id = profile.Id,
                IdentityUserId = user.Id,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEnd = user.LockoutEnd,
                Roles = roles.ToList()
            };
        }

        public async Task<UserInfo?> GetByIdentityUserIdAsync(Guid id)
        {
            using var context = await _dbFactory.CreateDbContextAsync();

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                throw new InvalidOperationException("Unable to find User for IdentityUserId " + id.ToString());

            var profile = await context.UserProfiles.FirstOrDefaultAsync(u => u.IdentityUserId == user.Id);
            if (profile == null)
                throw new InvalidOperationException("Unable to find UserProfile for IdentityUserId " + user.Id);

            var roles = await _userManager.GetRolesAsync(user);

            return new UserInfo
            {
                Id = profile.Id,
                IdentityUserId = user.Id,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEnd = user.LockoutEnd,
                Roles = roles.ToList()
            };
        }

        public async Task UpdateAsync(UserInfo user)
        {
            using var context = await _dbFactory.CreateDbContextAsync();

            var identityUser = await _userManager.FindByIdAsync(user.IdentityUserId.ToString());
            if (identityUser == null)
                throw new InvalidOperationException("Unable to find User for IdentityUserId " + user.IdentityUserId.ToString());

            identityUser.Email = user.Email;
            identityUser.FirstName = user.FirstName;
            identityUser.LastName = user.LastName;

            var result = await _userManager.UpdateAsync(identityUser);

            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task DeleteAsync(Guid id)
        {
            using var context = await _dbFactory.CreateDbContextAsync();

            var identityUser = await _userManager.FindByIdAsync(id.ToString());
            if (identityUser == null)
                throw new InvalidOperationException("Unable to find User for IdentityUserId " + id.ToString());

            var result = await _userManager.DeleteAsync(identityUser);

            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task UpdateRolesAsync(Guid identityUserId, IEnumerable<string> roles)
        {
            using var context = await _dbFactory.CreateDbContextAsync();

            var identityUser = await _userManager.FindByIdAsync(identityUserId.ToString());
            if (identityUser == null)
                throw new InvalidOperationException("Unable to find User for IdentityUserId " + identityUserId.ToString());

            var currentRoles = await _userManager.GetRolesAsync(identityUser);
            var rolesToRemove = currentRoles.Except(roles).ToList();
            var rolesToAdd = roles.Except(currentRoles).ToList();

            if (rolesToRemove.Count != 0)
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(identityUser, rolesToRemove);

                if (!removeResult.Succeeded)
                {
                    throw new InvalidOperationException(string.Join(", ", removeResult.Errors.Select(e => e.Description)));
                }
            }

            if (rolesToAdd.Count != 0)
            {
                var addResult = await _userManager.AddToRolesAsync(identityUser, rolesToAdd);

                if (!addResult.Succeeded)
                {
                    throw new InvalidOperationException(string.Join(", ", addResult.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
