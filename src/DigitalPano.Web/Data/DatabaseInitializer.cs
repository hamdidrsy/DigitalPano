using DigitalPano.Web.Data.Entities;
using DigitalPano.Web.Options;
using DigitalPano.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DigitalPano.Web.Data;

public sealed class DatabaseInitializer(
    AppDbContext dbContext,
    IScreenKeyService screenKeyService,
    IHostEnvironment hostEnvironment,
    UserManager<AppUser> userManager,
    IOptions<SeedAdminOptions> options,
    ILogger<DatabaseInitializer> logger)
{
    private const string DevelopmentScreenKey = "development-screen-key-change-before-production";
    private static readonly Action<ILogger, string, Exception?> AdminCreated =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(1001, nameof(AdminCreated)),
            "Başlangıç yönetici hesabı {Email} için oluşturuldu.");

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (!hostEnvironment.IsEnvironment("Testing") && dbContext.Database.IsRelational())
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
        }

        List<Screen> screensWithDevelopmentKey = hostEnvironment.IsEnvironment("Testing")
            ? []
            : await dbContext.Screens
                .Where(x => x.DeviceKey == DevelopmentScreenKey)
                .ToListAsync(cancellationToken);
        foreach (Screen screen in screensWithDevelopmentKey)
        {
            screen.DeviceKey = screenKeyService.Generate();
        }

        if (screensWithDevelopmentKey.Count > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        await SeedAdminAsync(cancellationToken);
    }

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
