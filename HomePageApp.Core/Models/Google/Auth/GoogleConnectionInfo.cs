namespace HomePageApp.Core.Models;

public class GoogleConnectionInfo
{
    public string GoogleUserId { get; set; } = string.Empty;

    public string GoogleEmail { get; set; } = string.Empty;

    public string? GoogleFirstName { get; set; }

    public string? GoogleLastName { get; set; }

    public string EncryptedRefreshToken { get; set; } = string.Empty;
}