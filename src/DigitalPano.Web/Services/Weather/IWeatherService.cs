namespace DigitalPano.Web.Services.Weather;

public interface IWeatherService
{
    Task<WeatherSnapshot?> GetCurrentAsync(string city, CancellationToken cancellationToken = default);
}

public sealed record WeatherSnapshot(string City, double TemperatureCelsius, string Description, string Symbol);
