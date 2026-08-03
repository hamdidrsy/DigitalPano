namespace DigitalPano.Web.Services;

public interface IScreenKeyService
{
    string Generate();

    bool IsValid(string expectedKey, string? suppliedKey);
}
