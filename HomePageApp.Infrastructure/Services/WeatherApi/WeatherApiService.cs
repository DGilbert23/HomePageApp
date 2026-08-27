using HomePageApp.Core.Interfaces.WeatherApi;
using HomePageApp.Core.Models.WeatherApi;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace HomePageApp.Infrastructure.Services.WeatherApi
{
    public class WeatherApiService : IWeatherApiService
    {

        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public WeatherApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["WeatherApiSettings:ApiKey"] ?? string.Empty;
        }

        private async Task<WeatherData> GetWeatherDataObjectAsync(string queryVar)
        {
            var requestUrl = $"v1/forecast.json?key={_apiKey}&q={Uri.EscapeDataString(queryVar)}&days=4";
            var rawData = await _httpClient.GetFromJsonAsync<WeatherResponse>(requestUrl);
            if (rawData == null) return new WeatherData();

            return new WeatherData
            {
                City = rawData.Location.Name,
                State = rawData.Location.Region,
                CurrentTempF = rawData.Current.TempF,
                TodayMinTempF = (rawData.Forecast.Forecastday.Where(fd => Convert.ToDateTime(fd.Date) == DateTime.Today).ToList())[0].Day.MintempF,
                TodayMaxTempF = (rawData.Forecast.Forecastday.Where(fd => Convert.ToDateTime(fd.Date) == DateTime.Today).ToList())[0].Day.MaxtempF,
                ConditionText = rawData.Current.Condition.Text,
                ConditionIconUrl = rawData.Current.Condition.Icon,
                TodayHours = rawData.Forecast.Forecastday.Where(fd => Convert.ToDateTime(fd.Date) == DateTime.Today || Convert.ToDateTime(fd.Date) == DateTime.Today.AddDays(1))
                                                            .SelectMany(day => day.Hour)
                                                            .Where(h => Convert.ToDateTime(h.Time) >= DateTime.Now)
                                                            .Select(h => new Core.Models.WeatherApi.Hour
                                                            {
                                                                Time = TimeOnly.Parse(Convert.ToDateTime(h.Time).TimeOfDay.ToString()),
                                                                ConditionIconUrl = h.Condition.Icon,
                                                                TempF = h.TempF,
                                                                ChanceOfRain = h.ChanceOfRain
                                                            }).ToList(),

                ForecastFuture = rawData.Forecast.Forecastday.Where(fd => Convert.ToDateTime(fd.Date) != DateTime.Today)
                                                             .Select(fd => new ForecastDay
                                                             {
                                                                 Date = DateOnly.Parse(fd.Date),
                                                                 ConditionIconUrl = fd.Day.Condition.Icon,
                                                                 ConditionText = fd.Day.Condition.Text,
                                                                 AverageTempF = fd.Day.AvgtempF,
                                                                 MinTempF = fd.Day.MintempF,
                                                                 MaxTempF = fd.Day.MaxtempF,
                                                                 ChanceOfRain = fd.Day.DailyChanceOfRain
                                                             }).ToList()
            };
        }

        public async Task<WeatherData> GetWidgetWeatherAsync(string zip)
        {
            try
            {
                return await GetWeatherDataObjectAsync(zip);
            }
            catch (HttpRequestException)
            {
                return new WeatherData { City = "Unavailable" };
            }
        }

        public async Task<WeatherData> GetWidgetWeatherAsync(double latitude, double longitude)
        {           
            try
            {
                return await GetWeatherDataObjectAsync(latitude.ToString() + "," + longitude.ToString());
            }
            catch (HttpRequestException)
            {
                return new WeatherData { City = "Unavailable" };
            }
        }
    }
}
