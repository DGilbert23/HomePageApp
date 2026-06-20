namespace HomePageApp.Core.Interfaces;

using HomePageApp.Core.Models;

public interface IWeatherApiService
{
    Task<WeatherData> GetWidgetWeatherAsync(string city);
}
