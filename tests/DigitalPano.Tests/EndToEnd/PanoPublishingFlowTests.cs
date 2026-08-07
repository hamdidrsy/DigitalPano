using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using DigitalPano.Tests.Integration;
using DigitalPano.Tests.Services;
using DigitalPano.Web.Data;
using DigitalPano.Web.Data.Entities;
using DigitalPano.Web.Services.Weather;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace DigitalPano.Tests.EndToEnd;

public sealed class PanoPublishingFlowTests
{
    [Fact]
    public async Task AdminCreatesAnnouncementAndTargetPanoDisplaysIt()
    {
        await using var factory = new PanoEndToEndFactory();
        await factory.SeedAsync(includeEmergency: false);
        using HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost"),
            HandleCookies = true
        });
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthHandler.SchemeName);

        using HttpResponseMessage formResponse = await client.GetAsync("/Admin/Announcements/Create");
        string formHtml = await formResponse.Content.ReadAsStringAsync();
        Assert.True(formResponse.IsSuccessStatusCode, formHtml);
        Match tokenMatch = Regex.Match(formHtml, "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"");
        Assert.True(tokenMatch.Success, "Duyuru formunda antiforgery belirteci bulunamadı.");
        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = WebUtility.HtmlDecode(tokenMatch.Groups[1].Value),
            ["Title"] = "HTTP ile oluşturulan duyuru",
            ["Description"] = "Yönetimden panoya uçtan uca doğrulama",
            ["ContentType"] = "Text",
            ["StartDate"] = "2026-08-04T11:00",
            ["EndDate"] = "2026-08-04T13:00",
            ["DisplayDurationSeconds"] = "10",
            ["SortOrder"] = "0",
            ["IsActive"] = "true",
            ["SelectedScreenIds"] = "10"
        };

        using HttpResponseMessage created = await client.PostAsync(
            "/Admin/Announcements/Create", new FormUrlEncodedContent(form));
        Assert.Equal(HttpStatusCode.Redirect, created.StatusCode);

        string panoHtml = WebUtility.HtmlDecode(
            await client.GetStringAsync("/pano/ana-ekran?key=e2e-device-key"));
        Assert.Contains("HTTP ile oluşturulan duyuru", panoHtml, StringComparison.Ordinal);
        Assert.Contains("Yönetimden panoya uçtan uca doğrulama", panoHtml, StringComparison.Ordinal);
    }

    [Fact]
    public async Task HomeRedirectsToAuthorizedPanoAndRendersOnlyCurrentContent()
    {
        await using var factory = new PanoEndToEndFactory();
        await factory.SeedAsync(includeEmergency: false);
        using HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });

        using HttpResponseMessage home = await client.GetAsync("/");
        Assert.Equal(HttpStatusCode.Redirect, home.StatusCode);
        Assert.NotNull(home.Headers.Location);
        Assert.Contains("/pano/ana-ekran", home.Headers.Location.OriginalString, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("key=e2e-device-key", home.Headers.Location.OriginalString, StringComparison.Ordinal);

        using HttpResponseMessage pano = await client.GetAsync(home.Headers.Location);
        string html = WebUtility.HtmlDecode(await pano.Content.ReadAsStringAsync());
        Assert.Equal(HttpStatusCode.OK, pano.StatusCode);
        Assert.Contains("Güncel robotik etkinliği", html, StringComparison.Ordinal);
        Assert.Contains("Bugünkü etkinlik 14.00'te", html, StringComparison.Ordinal);
        Assert.DoesNotContain("Süresi dolmuş duyuru", html, StringComparison.Ordinal);
        Assert.Contains("İstanbul", html, StringComparison.Ordinal);
        Assert.Contains("21°", html, StringComparison.Ordinal);
        Assert.Equal(2, CountOccurrences(html, "class=\"ticker-group\""));
        Assert.Contains("class=\"ticker-message\"", html, StringComparison.Ordinal);
    }

    private static int CountOccurrences(string value, string search) =>
        value.Split(search, StringSplitOptions.None).Length - 1;

    [Fact]
    public async Task InvalidDeviceKeyIsRejectedAcrossHttpPipeline()
    {
        await using var factory = new PanoEndToEndFactory();
        await factory.SeedAsync(includeEmergency: false);
        using HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        using HttpResponseMessage response = await client.GetAsync("/pano/ana-ekran?key=wrong-key");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task EmergencyReplacesNormalPanoThroughRenderedHttpResponse()
    {
        await using var factory = new PanoEndToEndFactory();
        await factory.SeedAsync(includeEmergency: true);
        using HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        string html = WebUtility.HtmlDecode(await client.GetStringAsync("/pano/ana-ekran?key=e2e-device-key"));

        Assert.Contains("ACİL DUYURU", html, StringComparison.Ordinal);
        Assert.Contains("Binayı sakin biçimde boşaltın", html, StringComparison.Ordinal);
        Assert.DoesNotContain("Güncel robotik etkinliği", html, StringComparison.Ordinal);
    }

    [Fact]
    public async Task OfflineShellAndServiceWorkerArePublished()
    {
        await using var factory = new PanoEndToEndFactory();
        using HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        string worker = await client.GetStringAsync("/service-worker.js");
        string offline = await client.GetStringAsync("/offline.html");

        Assert.Contains("digitalpano-v1", worker, StringComparison.Ordinal);
        Assert.Contains("X-DigitalPano-Offline", worker, StringComparison.Ordinal);
        Assert.Contains("Bağlantı bekleniyor", offline, StringComparison.Ordinal);
    }
}

internal sealed class PanoEndToEndFactory : WebApplicationFactory<Program>
{
    private static readonly DateTime UtcNow = new(2026, 8, 4, 9, 0, 0, DateTimeKind.Utc);
    private readonly string _databaseName = $"e2e-{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("SeedAdmin:Enabled", "false");
        builder.ConfigureLogging(logging => logging.ClearProviders());
        builder.ConfigureTestServices(services =>
        {
            services.AddDataProtection().UseEphemeralDataProtectionProvider();
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            ServiceDescriptor[] databaseConfigurations = services
                .Where(x => x.ServiceType.Name.StartsWith("IDbContextOptionsConfiguration", StringComparison.Ordinal))
                .ToArray();
            foreach (ServiceDescriptor descriptor in databaseConfigurations) services.Remove(descriptor);
            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(_databaseName));
            services.RemoveAll<TimeProvider>();
            services.AddSingleton<TimeProvider>(new FixedTimeProvider(new DateTimeOffset(UtcNow)));
            services.RemoveAll<IWeatherService>();
            services.AddSingleton<IWeatherService>(new EndToEndWeatherService());
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                    options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });
        });
    }

    public async Task SeedAsync(bool includeEmergency)
    {
        _ = Services;
        await using AsyncServiceScope scope = Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (await dbContext.Screens.AnyAsync()) return;

        var screen = new Screen
        {
            Id = 10, Name = "Ana Ekran", Slug = "ana-ekran",
            DeviceKey = "e2e-device-key", IsActive = true
        };
        dbContext.Screens.Add(screen);
        dbContext.InstitutionSettings.Add(new InstitutionSetting
        {
            InstitutionName = "Test Kurumu", City = "İstanbul"
        });
        dbContext.TickerMessages.Add(new TickerMessage
        {
            Text = "Bugünkü etkinlik 14.00'te",
            StartDateUtc = new DateTime(2026, 1, 1), EndDateUtc = new DateTime(2026, 12, 31), IsActive = true
        });
        dbContext.Announcements.AddRange(
            Announcement(20, "Güncel robotik etkinliği", new DateTime(2026, 1, 1), new DateTime(2026, 12, 31), screen),
            Announcement(21, "Süresi dolmuş duyuru", new DateTime(2025, 1, 1), new DateTime(2025, 12, 31), screen));
        if (includeEmergency)
        {
            Announcement emergency = Announcement(22, "Tahliye", new DateTime(2026, 1, 1), new DateTime(2026, 12, 31), screen);
            emergency.Description = "Binayı sakin biçimde boşaltın";
            emergency.IsEmergency = true;
            dbContext.Announcements.Add(emergency);
        }
        await dbContext.SaveChangesAsync();
    }

    private static Announcement Announcement(int id, string title, DateTime start, DateTime end, Screen screen) => new()
    {
        Id = id, Title = title, Description = title,
        ContentType = AnnouncementContentType.Text,
        StartDateUtc = start, EndDateUtc = end, IsActive = true,
        AnnouncementScreens = [new AnnouncementScreen { ScreenId = screen.Id }]
    };
}

internal sealed class EndToEndWeatherService : IWeatherService
{
    public Task<WeatherSnapshot?> GetCurrentAsync(string city, CancellationToken cancellationToken = default) =>
        Task.FromResult<WeatherSnapshot?>(new WeatherSnapshot(city, 21, "Açık", "☀"));
}
