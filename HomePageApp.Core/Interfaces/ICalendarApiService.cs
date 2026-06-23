using HomePageApp.Core.Models;

namespace HomePageApp.Core.Interfaces
{
    public interface ICalendarApiService
    {
        Task<List<CalendarData>> GetUpcomingEventsAsync(CancellationToken cancellationToken = default);
    }
}
