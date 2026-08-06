namespace HomePageApp.Core.Models.UserManagement
{
    public class UserInfo
    {
        public int Id { get; set; }

        public Guid IdentityUserId { get; set; }

        public string Email { get; set; } = string.Empty;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool LockoutEnabled { get; set; }

        public DateTimeOffset? LockoutEnd { get; set; }

        public List<string> Roles { get; set; } = new();
    }
}
