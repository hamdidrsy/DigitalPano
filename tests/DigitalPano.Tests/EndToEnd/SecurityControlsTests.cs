using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using DigitalPano.Tests.Integration;
using DigitalPano.Web.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace DigitalPano.Tests.EndToEnd;

public sealed class SecurityControlsTests
{
    [Fact]
    public async Task RazorEncodesStoredXssPayloadOnPano()
    {
        await using var factory = new PanoEndToEndFactory();
        await factory.SeedAsync(includeEmergency: false);
        using HttpClient client = CreateAuthenticatedClient(factory);
        (string token, _) = await GetAntiforgeryFormAsync(client, "/Admin/Announcements/Create");
        const string titlePayload = "<script>window.xss=true</script>";
        const string descriptionPayload = "<img src=x onerror=alert(1)>";

        using HttpResponseMessage created = await client.PostAsync("/Admin/Announcements/Create",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["__RequestVerificationToken"] = token,
                ["Title"] = titlePayload,
                ["Description"] = descriptionPayload,
                ["ContentType"] = "Text",
                ["StartDate"] = "2026-08-04T11:00",
                ["EndDate"] = "2026-08-04T13:00",
                ["DisplayDurationSeconds"] = "10",
                ["SortOrder"] = "0",
                ["IsActive"] = "true",
                ["SelectedScreenIds"] = "10"
            }));
        Assert.Equal(HttpStatusCode.Redirect, created.StatusCode);

        string rawHtml = await client.GetStringAsync("/pano/ana-ekran?key=e2e-device-key");
        Assert.DoesNotContain(titlePayload, rawHtml, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(descriptionPayload, rawHtml, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("&lt;script&gt;", rawHtml, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("&lt;img", rawHtml, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AuthenticatedPostWithoutAntiforgeryTokenIsRejectedAndDoesNotMutateData()
    {
        await using var factory = new PanoEndToEndFactory();
        await factory.SeedAsync(includeEmergency: false);
        using HttpClient client = CreateAuthenticatedClient(factory);

        using HttpResponseMessage response = await client.PostAsync("/Admin/TickerMessages/Create",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["Text"] = "CSRF ile eklenmemeli",
                ["StartDate"] = "2026-08-04T11:00",
                ["EndDate"] = "2026-08-04T13:00",
                ["IsActive"] = "true"
            }));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await using AsyncServiceScope scope = factory.Services.CreateAsyncScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.False(await dbContext.TickerMessages.AnyAsync(x => x.Text == "CSRF ile eklenmemeli"));
    }

    [Fact]
    public async Task SpoofedImageUploadIsRejectedAcrossHttpPipeline()
    {
        await using var factory = new PanoEndToEndFactory();
        await factory.SeedAsync(includeEmergency: false);
        using HttpClient client = CreateAuthenticatedClient(factory);
        (string token, _) = await GetAntiforgeryFormAsync(client, "/Admin/Media/Upload");
        using var content = new MultipartFormDataContent();
        content.Add(new StringContent(token), "__RequestVerificationToken");
        var fakeImage = new ByteArrayContent("<script>alert(1)</script>"u8.ToArray());
        fakeImage.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        content.Add(fakeImage, "File", "zararli.png");

        using HttpResponseMessage response = await client.PostAsync("/Admin/Media/Upload", content);
        string html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Yalnızca geçerli JPEG, PNG, WebP veya MP4", WebUtility.HtmlDecode(html), StringComparison.Ordinal);
        await using AsyncServiceScope scope = factory.Services.CreateAsyncScope();
        Assert.Empty(await scope.ServiceProvider.GetRequiredService<AppDbContext>().Media.ToListAsync());
    }

    [Theory]
    [InlineData("/Admin")]
    [InlineData("/Admin/Announcements/Create")]
    [InlineData("/Admin/Media/Upload")]
    [InlineData("/Admin/Emergencies")]
    [InlineData("/Admin/Settings")]
    public async Task AnonymousUserCannotOpenAdminEndpoints(string path)
    {
        await using var factory = new PanoEndToEndFactory();
        using HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });

        using HttpResponseMessage response = await client.GetAsync(path);

        Assert.True(
            response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Redirect,
            $"Beklenen 401 veya giriş yönlendirmesi, alınan: {response.StatusCode}");
        if (response.StatusCode == HttpStatusCode.Redirect)
        {
            Assert.Equal("/hesap/giris", response.Headers.Location?.AbsolutePath);
        }
    }

    private static HttpClient CreateAuthenticatedClient(PanoEndToEndFactory factory)
    {
        HttpClient client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            HandleCookies = true,
            BaseAddress = new Uri("https://localhost")
        });
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthHandler.SchemeName);
        return client;
    }

    private static async Task<(string Token, string Html)> GetAntiforgeryFormAsync(HttpClient client, string path)
    {
        using HttpResponseMessage response = await client.GetAsync(path);
        string html = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Match match = Regex.Match(html, "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"");
        Assert.True(match.Success, $"{path} formunda antiforgery belirteci bulunamadı.");
        return (WebUtility.HtmlDecode(match.Groups[1].Value), html);
    }
}
