using DigitalPano.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalPano.Web.Data.Configurations;

public sealed class AnnouncementScreenConfiguration : IEntityTypeConfiguration<AnnouncementScreen>
{
    public void Configure(EntityTypeBuilder<AnnouncementScreen> builder)
    {
        builder.HasKey(x => new { x.AnnouncementId, x.ScreenId });

        builder.HasOne(x => x.Announcement)
            .WithMany(x => x.AnnouncementScreens)
            .HasForeignKey(x => x.AnnouncementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Screen)
            .WithMany(x => x.AnnouncementScreens)
            .HasForeignKey(x => x.ScreenId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
