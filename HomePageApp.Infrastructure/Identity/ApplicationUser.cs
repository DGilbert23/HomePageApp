using Microsoft.AspNetCore.Identity;

namespace HomePageApp.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public string? GoogleRefreshToken { get; set; }
    public string? GoogleUserId { get; set; }
    public string? GoogleEmail { get; set; }
    public string? GoogleFirstName { get; set; }
    public string? GoogleLastName { get; set; }
    public DateTime? GoogleConnectedUtc { get; set; }
}