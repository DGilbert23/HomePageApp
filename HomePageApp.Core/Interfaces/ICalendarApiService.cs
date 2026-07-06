using HomePageApp.Core.Models.CalendarApi;

namespace HomePageApp.Core.Interfaces
{
    public interface ICalendarApiService
    {
        Task<List<CalendarData>> GetUpcomingEventsAsync(string accessToken, CancellationToken cancellationToken = default);
    }
}
