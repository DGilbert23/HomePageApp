using HomePageApp.Core.Interfaces.Calendar;
using HomePageApp.Core.Interfaces.Google;
using HomePageApp.Core.Models.CalendarApi;
using HomePageApp.Infrastructure.Services.Google.GoogleCalendar;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Infrastructure.Services;

public class CalendarApiService : ICalendarApiService
{
    private readonly HttpClient _httpClient;
    private readonly IGoogleAuthService _googleAuthService;

    public CalendarApiService(
        HttpClient httpClient,
        IGoogleAuthService googleAuthService,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _googleAuthService = googleAuthService;
        _httpClient.BaseAddress = new Uri(configuration["GoogleCalendarApi:BaseUrl"] ?? string.Empty);
    }

    public async Task<List<CalendarData>> GetUpcomingEventsAsync(CancellationToken cancellationToken = default)
    {
        var accessToken = await _googleAuthService.GetValidAccessTokenAsync();

        if (string.IsNullOrEmpty(accessToken))
            return new List<CalendarData>();

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

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
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException($"Google Calendar API failure (Status: {ex.StatusCode}). Detail: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Unexpected error compiling calendar data: {ex.Message}", ex);
        }
    }
}
