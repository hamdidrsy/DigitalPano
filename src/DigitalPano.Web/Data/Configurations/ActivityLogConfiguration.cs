using DigitalPano.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalPano.Web.Data.Configurations;

public sealed class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
{
    public void Configure(EntityTypeBuilder<ActivityLog> builder)
    {
        builder.Property(x => x.ActionType).HasMaxLength(100).IsRequired();
        builder.Property(x => x.EntityType).HasMaxLength(100).IsRequired();
        builder.Property(x => x.EntityId).HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
        builder.Property(x => x.IpAddress).HasMaxLength(45);
        builder.HasIndex(x => x.CreatedAtUtc);

        builder.HasOne(x => x.User)
            .WithMany(x => x.ActivityLogs)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
