using HomePageApp.Core.Interfaces;
using HomePageApp.Core.Models;
using HomePageApp.Infrastructure.Services.Google.GoogleCalendar;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Xml;

namespace Infrastructure.Services;

public class CalendarApiService : ICalendarApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CalendarApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _httpClient.BaseAddress = new Uri(configuration["GoogleCalendarApi:BaseUrl"] ?? string.Empty);
    }

    public async Task<List<CalendarData>> GetUpcomingEventsAsync(CancellationToken cancellationToken = default)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return new List<CalendarData>();

        var accessToken = await httpContext.GetTokenAsync("access_token");
        if (string.IsNullOrEmpty(accessToken)) return new List<CalendarData>();

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var timeMin = XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Utc);
        var timeMax = XmlConvert.ToString(DateTime.Now.AddDays(3), XmlDateTimeSerializationMode.Utc);
        var requestUrl = _httpClient.BaseAddress + "?timeMin=" + timeMin + "&timeMax=" + timeMax;

        try
        {
            var response = await _httpClient.GetFromJsonAsync<CalendarResponse>(requestUrl, cancellationToken);

            if (response?.Items == null) return new List<CalendarData>();

            List<CalendarData> calendarDatas = new List<CalendarData>();
            foreach (Item item in response.Items)
            {
                calendarDatas.Add(new CalendarData
                {
                    Id = item.Id,
                    Title = item.Summary,
                    Description = item.Description,
                    StartTime = Convert.ToDateTime(item.Start.DateTime == null ? item.Start.Date : item.Start.DateTime),
                    EndTime = Convert.ToDateTime(item.End.DateTime == null ? item.End.Date : item.End.DateTime),
                    HtmlLink = item.HtmlLink
                });
            }
            return calendarDatas;
        }
        catch (Exception)
        {
            return new List<CalendarData>();
        }
    }
}
