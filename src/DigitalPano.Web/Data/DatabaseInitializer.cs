using DigitalPano.Web.Data.Entities;
using DigitalPano.Web.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace DigitalPano.Web.Data;

public sealed class DatabaseInitializer(
    UserManager<AppUser> userManager,
    IOptions<SeedAdminOptions> options,
    ILogger<DatabaseInitializer> logger)
{
    private static readonly Action<ILogger, string, Exception?> AdminCreated =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(1001, nameof(AdminCreated)),
            "Başlangıç yönetici hesabı {Email} için oluşturuldu.");

    public async Task SeedAdminAsync(CancellationToken cancellationToken = default)
    {
        SeedAdminOptions settings = options.Value;
        if (!settings.Enabled)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(settings.Email) || string.IsNullOrWhiteSpace(settings.Password))
        {
            throw new InvalidOperationException(
                "SeedAdmin etkin olduğunda e-posta ve parola güvenli yapılandırmadan sağlanmalıdır.");
        }

        AppUser? existingUser = await userManager.FindByEmailAsync(settings.Email);
        if (existingUser is not null)
        {
            return;
        }

        var user = new AppUser
        {
            UserName = settings.Email,
            Email = settings.Email,
            EmailConfirmed = true,
            DisplayName = settings.DisplayName,
            CreatedAtUtc = DateTime.UtcNow
        };

        IdentityResult result = await userManager.CreateAsync(user, settings.Password);
        if (!result.Succeeded)
        {
            string errors = string.Join(", ", result.Errors.Select(x => x.Description));
            throw new InvalidOperationException($"Başlangıç yöneticisi oluşturulamadı: {errors}");
        }

        AdminCreated(logger, settings.Email, null);
        cancellationToken.ThrowIfCancellationRequested();
    }
}
