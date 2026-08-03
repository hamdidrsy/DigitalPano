using DigitalPano.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalPano.Web.Data.Configurations;

public sealed class AnnouncementConfiguration : IEntityTypeConfiguration<Announcement>
{
    public void Configure(EntityTypeBuilder<Announcement> builder)
    {
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(4000).IsRequired();
        builder.Property(x => x.ContentType).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.DisplayDurationSeconds).HasDefaultValue(10);

        builder.HasIndex(x => new { x.IsActive, x.StartDateUtc, x.EndDateUtc });
        builder.HasIndex(x => new { x.IsEmergency, x.IsActive });

        builder.ToTable(table =>
        {
            table.HasCheckConstraint(
                "CK_Announcements_DateRange",
                "[EndDateUtc] > [StartDateUtc]");
            table.HasCheckConstraint(
                "CK_Announcements_DisplayDuration",
                "[DisplayDurationSeconds] BETWEEN 1 AND 3600");
        });

        builder.HasOne(x => x.CreatedByUser)
            .WithMany(x => x.CreatedAnnouncements)
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.Media)
            .WithMany(x => x.Announcements)
            .HasForeignKey(x => x.MediaId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
