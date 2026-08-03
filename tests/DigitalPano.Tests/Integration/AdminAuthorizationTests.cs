using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using DigitalPano.Web.Models.Admin;
using DigitalPano.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DigitalPano.Tests.Integration;

public sealed class AdminAuthorizationTests
{
    [Fact]
    public async Task AnonymousAdminRequestRedirectsToLogin()
    {
        await using var factory = new DigitalPanoWebApplicationFactory();
        using HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });

        using HttpResponseMessage response = await client.GetAsync("/Admin");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.Equal("/hesap/giris", response.Headers.Location.AbsolutePath);
        Assert.Contains("ReturnUrl=%2FAdmin", response.Headers.Location.OriginalString, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task LoginPageIsAvailableInTurkish()
    {
        await using var factory = new DigitalPanoWebApplicationFactory();
        using HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        using HttpResponseMessage response = await client.GetAsync("/hesap/giris");
        string html = await response.Content.ReadAsStringAsync();

        Assert.True(response.StatusCode == HttpStatusCode.OK, html);
        Assert.Contains("Yönetici girişi", html, StringComparison.Ordinal);
        Assert.Contains("Giriş yap", html, StringComparison.Ordinal);
    }

    [Fact]
    public async Task AuthenticatedAdminRequestDisplaysDashboard()
    {
        await using var factory = new AuthenticatedDigitalPanoWebApplicationFactory();
        using HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthHandler.SchemeName);

        using HttpResponseMessage response = await client.GetAsync("/Admin");
        string html = await response.Content.ReadAsStringAsync();

        Assert.True(response.StatusCode == HttpStatusCode.OK, html);
        Assert.Contains("Gösterge paneli", html, StringComparison.Ordinal);
        Assert.Contains("Yayındaki duyuru", html, StringComparison.Ordinal);
    }

    [Fact]
    public async Task LogoutDoesNotAcceptGetRequests()
    {
        await using var factory = new AuthenticatedDigitalPanoWebApplicationFactory();
        using HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthHandler.SchemeName);

        using HttpResponseMessage response = await client.GetAsync("/hesap/cikis");

        Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
    }

    [Fact]
    public async Task AnonymousLogoutRequestRedirectsToLogin()
    {
        await using var factory = new DigitalPanoWebApplicationFactory();
        using HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });

        using HttpResponseMessage response = await client.PostAsync("/hesap/cikis", content: null);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.Equal("/hesap/giris", response.Headers.Location.AbsolutePath);
    }
}

internal class DigitalPanoWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureLogging(logging => logging.ClearProviders());
        builder.ConfigureTestServices(services =>
            services.AddDataProtection().UseEphemeralDataProtectionProvider());
    }
}

internal sealed class AuthenticatedDigitalPanoWebApplicationFactory : DigitalPanoWebApplicationFactory
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IDashboardService>();
            services.AddSingleton<IDashboardService>(new FakeDashboardService());
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                    options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });
        });
    }
}

internal sealed class FakeDashboardService : IDashboardService
{
    public Task<DashboardViewModel> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var summary = new DashboardViewModel(4, 2, 1, 3, 2, 0);
        return Task.FromResult(summary);
    }
}

internal sealed class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "Test";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        Claim[] claims =
        [
            new(ClaimTypes.NameIdentifier, "test-admin-id"),
            new(ClaimTypes.Name, "test-admin@example.local")
        ];
        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
