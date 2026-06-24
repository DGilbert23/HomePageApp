namespace HomePageApp.Core.Interfaces
{
    public interface IGoogleAuthService
    {
        Task<GoogleAuthDto?> GetRemoteDataAsync(CancellationToken cancellationToken = default);
        Task<string?> GetValidAccessTokenAsync();
    }

    public record GoogleAuthDto(string Id, string Content, DateTime FetchedAt);
}
