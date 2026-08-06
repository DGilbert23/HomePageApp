namespace HomePageApp.Core.Interfaces.WeatherApi;

using HomePageApp.Core.Models.WeatherApi;

public interface IWeatherApiService
{
    Task<WeatherData> GetWidgetWeatherAsync(string city);
}
