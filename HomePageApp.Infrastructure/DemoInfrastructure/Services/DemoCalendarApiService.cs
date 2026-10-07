using HomePageApp.Core.Interfaces.Calendar;
using HomePageApp.Core.Models.CalendarApi;
using HomePageApp.Infrastructure.Services.Google.GoogleCalendar;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;

namespace HomePageApp.Infrastructure.DemoInfrastructure.Services;

public class DemoCalendarApiService : ICalendarApiService
{
    public async Task<List<CalendarData>> GetUpcomingEventsAsync(CancellationToken cancellationToken = default)
    {
        List<CalendarData> data = new List<CalendarData>();

        data.Add(new CalendarData
        {
            Id = "0",
            Title = "Example Appointment",
            Description = "A basic event example",
            StartTime = DateTime.Now.AddHours(3),
            EndTime = DateTime.Now.AddHours(4),
            IsAllDay = false,
            HtmlLink = "https://drg-webdev.com/"
        });

        data.Add(new CalendarData
        {
            Id = "0",
            Title = "All-day Event Example",
            Description = "A basic event example that runs for a full day",
            StartTime = DateTime.Now.AddDays(1),
            EndTime = DateTime.Now.AddDays(2),
            IsAllDay = true,
            HtmlLink = "https://drg-webdev.com/"
        });

        return data;
    }
}

