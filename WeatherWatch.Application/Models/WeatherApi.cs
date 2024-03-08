using WeatherWatch.Application.Interfaces;

namespace WeatherWatch.Application.Models
{
    public class WeatherApi : IWeatherApi
    {
        public string Url { get; set; } = String.Empty;
    }
}
