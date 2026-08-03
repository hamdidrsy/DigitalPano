using DigitalPano.Web.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DigitalPano.Web.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<AppUser>(options)
{
    public DbSet<Announcement> Announcements => Set<Announcement>();

    public DbSet<AnnouncementScreen> AnnouncementScreens => Set<AnnouncementScreen>();

    public DbSet<Screen> Screens => Set<Screen>();

    public DbSet<Media> Media => Set<Media>();

    public DbSet<InstitutionSetting> InstitutionSettings => Set<InstitutionSetting>();

    public DbSet<TickerMessage> TickerMessages => Set<TickerMessage>();

    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
