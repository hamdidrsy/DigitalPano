using DigitalPano.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalPano.Web.Data.Configurations;

public sealed class InstitutionSettingConfiguration : IEntityTypeConfiguration<InstitutionSetting>
{
    public void Configure(EntityTypeBuilder<InstitutionSetting> builder)
    {
        builder.Property(x => x.InstitutionName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.LogoPath).HasMaxLength(500);
        builder.Property(x => x.PrimaryColor).HasMaxLength(20).IsRequired();
        builder.Property(x => x.SecondaryColor).HasMaxLength(20).IsRequired();
        builder.Property(x => x.City).HasMaxLength(100).IsRequired();
        builder.Property(x => x.TimeZoneId).HasMaxLength(100).IsRequired();

        builder.HasData(new InstitutionSetting
        {
            Id = 1,
            InstitutionName = "Özel Eğitim Kursu",
            PrimaryColor = "#0D6EFD",
            SecondaryColor = "#6C757D",
            City = "İstanbul",
            TimeZoneId = "Europe/Istanbul",
            UpdatedAtUtc = new DateTime(2026, 8, 3, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}
