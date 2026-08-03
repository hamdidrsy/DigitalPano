using System.Security.Cryptography;
using System.Text;

namespace DigitalPano.Web.Services;

public sealed class ScreenKeyService : IScreenKeyService
{
    public string Generate() => RandomNumberGenerator.GetHexString(64, lowercase: true);

    public bool IsValid(string expectedKey, string? suppliedKey)
    {
        if (string.IsNullOrWhiteSpace(expectedKey) || string.IsNullOrWhiteSpace(suppliedKey))
        {
            return false;
        }

        byte[] expectedBytes = Encoding.UTF8.GetBytes(expectedKey);
        byte[] suppliedBytes = Encoding.UTF8.GetBytes(suppliedKey);
        return expectedBytes.Length == suppliedBytes.Length &&
               CryptographicOperations.FixedTimeEquals(expectedBytes, suppliedBytes);
    }
}
