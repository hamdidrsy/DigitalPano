using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace DigitalPano.Web.Services;

public sealed partial class SlugService : ISlugService
{
    public string CreateSlug(string value)
    {
        string turkishNormalized = value
            .Replace('ı', 'i')
            .Replace('İ', 'I')
            .Replace('ş', 's')
            .Replace('Ş', 'S')
            .Replace('ğ', 'g')
            .Replace('Ğ', 'G')
            .Replace('ü', 'u')
            .Replace('Ü', 'U')
            .Replace('ö', 'o')
            .Replace('Ö', 'O')
            .Replace('ç', 'c')
            .Replace('Ç', 'C');

        string decomposed = turkishNormalized.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(decomposed.Length);
        foreach (char character in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(character);
            }
        }

        string lowercase = builder.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
        string slug = InvalidSlugCharacterRegex().Replace(lowercase, "-");
        return RepeatedHyphenRegex().Replace(slug, "-").Trim('-');
    }

    [GeneratedRegex("[^a-z0-9]+", RegexOptions.CultureInvariant)]
    private static partial Regex InvalidSlugCharacterRegex();

    [GeneratedRegex("-{2,}", RegexOptions.CultureInvariant)]
    private static partial Regex RepeatedHyphenRegex();
}
