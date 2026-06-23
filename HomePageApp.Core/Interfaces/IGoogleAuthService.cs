namespace HomePageApp.Core.Interfaces
{
    public interface IGoogleAuthService
    {
        Task<ExternalDataDto?> GetRemoteDataAsync(CancellationToken cancellationToken = default);
    }

    public record ExternalDataDto(string Id, string Content, DateTime FetchedAt);
}
