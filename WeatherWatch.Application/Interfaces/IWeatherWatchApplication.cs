using WeatherWatch.Application.Models;

namespace WeatherWatch.Application.Interfaces
{
    public interface IWeatherWatchApplication
    {
        HomeViewModel GetForecast(string zipCode, Guid traceId, Guid spanId);
        HomeViewModel GetRandom(Guid traceId, Guid spanId);
        HomeViewModel SaveFavorite(string zipCode, Guid traceId, Guid spanId);
    }
}
