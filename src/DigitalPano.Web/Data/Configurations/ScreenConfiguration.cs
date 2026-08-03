using DigitalPano.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalPano.Web.Data.Configurations;

public sealed class ScreenConfiguration : IEntityTypeConfiguration<Screen>
{
    public void Configure(EntityTypeBuilder<Screen> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Slug).HasMaxLength(120).IsRequired();
        builder.Property(x => x.DeviceKey).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Location).HasMaxLength(200);

        builder.HasIndex(x => x.Slug).IsUnique();
        builder.HasIndex(x => x.DeviceKey).IsUnique();

        builder.HasData(new Screen
        {
            Id = 1,
            Name = "Giriş Katı",
            Slug = "giris-kati",
            DeviceKey = "development-screen-key-change-before-production",
            Location = "Giriş Katı",
            IsActive = true,
            CreatedAtUtc = new DateTime(2026, 8, 3, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}
