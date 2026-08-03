using DigitalPano.Web.Data;
using DigitalPano.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DigitalPano.Tests.Data;

public sealed class AppDbContextModelTests
{
    [Fact]
    public void ModelContainsAllDomainEntities()
    {
        using AppDbContext context = CreateContext();

        Type[] expectedTypes =
        [
            typeof(AppUser),
            typeof(Announcement),
            typeof(AnnouncementScreen),
            typeof(Screen),
            typeof(Media),
            typeof(InstitutionSetting),
            typeof(TickerMessage),
            typeof(ActivityLog)
        ];

        foreach (Type entityType in expectedTypes)
        {
            Assert.NotNull(context.Model.FindEntityType(entityType));
        }
    }

    [Fact]
    public void AnnouncementScreenUsesCompositePrimaryKey()
    {
        using AppDbContext context = CreateContext();
        IEntityType entityType = context.Model.FindEntityType(typeof(AnnouncementScreen))!;

        IReadOnlyList<string> keyProperties = entityType.FindPrimaryKey()!
            .Properties
            .Select(x => x.Name)
            .ToArray();

        Assert.Equal(
            [nameof(AnnouncementScreen.AnnouncementId), nameof(AnnouncementScreen.ScreenId)],
            keyProperties);
    }

    [Fact]
    public void ScreenSlugAndDeviceKeyAreUnique()
    {
        using AppDbContext context = CreateContext();
        IEntityType entityType = context.Model.FindEntityType(typeof(Screen))!;

        IReadOnlyList<IReadOnlyList<string>> uniqueIndexes = entityType.GetIndexes()
            .Where(x => x.IsUnique)
            .Select(x => (IReadOnlyList<string>)x.Properties.Select(p => p.Name).ToArray())
            .ToArray();

        Assert.Contains(uniqueIndexes, x => x.SequenceEqual([nameof(Screen.Slug)]));
        Assert.Contains(uniqueIndexes, x => x.SequenceEqual([nameof(Screen.DeviceKey)]));
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=DigitalPanoModelTests;Trusted_Connection=True")
            .Options;

        return new AppDbContext(options);
    }
}
