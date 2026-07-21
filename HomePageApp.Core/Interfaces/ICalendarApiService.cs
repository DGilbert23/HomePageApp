using HomePageApp.Core.Models.CalendarApi;

namespace HomePageApp.Core.Interfaces
{
    public interface ICalendarApiService
    {
        Task<List<CalendarData>> GetUpcomingEventsAsync(CancellationToken cancellationToken = default);
    }
}
