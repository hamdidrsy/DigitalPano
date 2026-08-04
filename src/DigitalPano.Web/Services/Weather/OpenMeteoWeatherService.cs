using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Memory;

namespace DigitalPano.Web.Services.Weather;

public sealed class OpenMeteoWeatherService(
    IHttpClientFactory httpClientFactory,
    IMemoryCache memoryCache,
    ILogger<OpenMeteoWeatherService> logger) : IWeatherService
{
    private static readonly Action<ILogger, string, Exception?> WeatherRequestFailed =
        LoggerMessage.Define<string>(LogLevel.Warning, new EventId(2001, "WeatherRequestFailed"),
            "{City} için hava durumu alınamadı.");

    public async Task<WeatherSnapshot?> GetCurrentAsync(string city, CancellationToken cancellationToken = default)
    {
        string normalizedCity = string.IsNullOrWhiteSpace(city) ? "İstanbul" : city.Trim();
        string cacheKey = $"weather:{normalizedCity.ToUpperInvariant()}";
        if (memoryCache.TryGetValue(cacheKey, out WeatherSnapshot? cached)) return cached;

        try
        {
            HttpClient client = httpClientFactory.CreateClient(nameof(OpenMeteoWeatherService));
            string geoUrl = $"https://geocoding-api.open-meteo.com/v1/search?name={Uri.EscapeDataString(normalizedCity)}&count=1&language=tr&format=json&countryCode=TR";
            GeoResponse? geo = await client.GetFromJsonAsync<GeoResponse>(geoUrl, cancellationToken);
            GeoResult? location = geo?.Results?.FirstOrDefault();
            if (location is null)
            {
                memoryCache.Set<WeatherSnapshot?>(cacheKey, null, TimeSpan.FromMinutes(2));
                return null;
            }

            string latitude = location.Latitude.ToString(CultureInfo.InvariantCulture);
            string longitude = location.Longitude.ToString(CultureInfo.InvariantCulture);
            string forecastUrl = $"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&current=temperature_2m,weather_code&timezone=Europe%2FIstanbul";
            ForecastResponse? forecast = await client.GetFromJsonAsync<ForecastResponse>(forecastUrl, cancellationToken);
            if (forecast?.Current is null)
            {
                memoryCache.Set<WeatherSnapshot?>(cacheKey, null, TimeSpan.FromMinutes(2));
                return null;
            }

            (string description, string symbol) = Describe(forecast.Current.WeatherCode);
            var snapshot = new WeatherSnapshot(location.Name, forecast.Current.Temperature, description, symbol);
            memoryCache.Set(cacheKey, snapshot, TimeSpan.FromMinutes(15));
            return snapshot;
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException)
        {
            WeatherRequestFailed(logger, normalizedCity, exception);
            memoryCache.Set<WeatherSnapshot?>(cacheKey, null, TimeSpan.FromMinutes(2));
            return null;
        }
    }

    public static (string Description, string Symbol) Describe(int code) => code switch
    {
        0 => ("Açık", "☀"),
        1 or 2 => ("Parçalı bulutlu", "⛅"),
        3 => ("Kapalı", "☁"),
        45 or 48 => ("Sisli", "🌫"),
        >= 51 and <= 67 => ("Yağmurlu", "🌧"),
        >= 71 and <= 77 => ("Karlı", "❄"),
        >= 80 and <= 82 => ("Sağanak", "🌦"),
        >= 85 and <= 86 => ("Kar yağışlı", "🌨"),
        >= 95 => ("Fırtınalı", "⛈"),
        _ => ("Değişken", "🌤")
    };

    private sealed record GeoResponse([property: JsonPropertyName("results")] GeoResult[]? Results);
    private sealed record GeoResult(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("latitude")] double Latitude,
        [property: JsonPropertyName("longitude")] double Longitude);
    private sealed record ForecastResponse([property: JsonPropertyName("current")] CurrentWeather? Current);
    private sealed record CurrentWeather(
        [property: JsonPropertyName("temperature_2m")] double Temperature,
        [property: JsonPropertyName("weather_code")] int WeatherCode);
}
