using DigitalPano.Web.Services.Weather;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;

namespace DigitalPano.Tests.Services;

public sealed class OpenMeteoWeatherServiceTests
{
    [Theory]
    [InlineData(0, "Açık", "☀")]
    [InlineData(3, "Kapalı", "☁")]
    [InlineData(61, "Yağmurlu", "🌧")]
    [InlineData(95, "Fırtınalı", "⛈")]
    public void DescribeMapsWmoCodes(int code, string description, string symbol)
    {
        Assert.Equal((description, symbol), OpenMeteoWeatherService.Describe(code));
    }

    [Fact]
    public async Task ProviderFailureReturnsNullInsteadOfBreakingPano()
    {
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new OpenMeteoWeatherService(
            new ThrowingHttpClientFactory(), cache, NullLogger<OpenMeteoWeatherService>.Instance);

        WeatherSnapshot? result = await service.GetCurrentAsync("İstanbul");

        Assert.Null(result);
    }
}

internal sealed class ThrowingHttpClientFactory : IHttpClientFactory
{
    public HttpClient CreateClient(string name) => new(new ThrowingHandler());

    private sealed class ThrowingHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            throw new HttpRequestException("Test service outage");
    }
}
