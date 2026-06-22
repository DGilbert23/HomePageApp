namespace HomePageApp.Core.Interfaces
{
    public interface IExternalApiService
    {
        Task<ExternalDataDto?> GetRemoteDataAsync(CancellationToken cancellationToken = default);
    }

    public record ExternalDataDto(string Id, string Content, DateTime FetchedAt);
}
