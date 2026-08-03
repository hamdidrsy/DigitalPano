using DigitalPano.Web.Services;

namespace DigitalPano.Tests.Services;

public sealed class ScreenIdentityServicesTests
{
    [Theory]
    [InlineData("Giriş Katı", "giris-kati")]
    [InlineData("Öğretmenler Odası", "ogretmenler-odasi")]
    [InlineData("  2. Kat / Doğu  ", "2-kat-dogu")]
    public void CreateSlugNormalizesTurkishNames(string input, string expected)
    {
        var service = new SlugService();

        Assert.Equal(expected, service.CreateSlug(input));
    }

    [Fact]
    public void ScreenKeysAreRandomAndValidated()
    {
        var service = new ScreenKeyService();

        string first = service.Generate();
        string second = service.Generate();

        Assert.Equal(64, first.Length);
        Assert.NotEqual(first, second);
        Assert.True(service.IsValid(first, first));
        Assert.False(service.IsValid(first, second));
        Assert.False(service.IsValid(first, null));
    }
}
