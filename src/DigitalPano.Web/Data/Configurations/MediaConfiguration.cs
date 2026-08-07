using DigitalPano.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalPano.Web.Data.Configurations;

public sealed class MediaConfiguration : IEntityTypeConfiguration<Media>
{
    public void Configure(EntityTypeBuilder<Media> builder)
    {
        builder.Property(x => x.OriginalFileName).HasMaxLength(255).IsRequired();
        builder.Property(x => x.StoredFileName).HasMaxLength(255).IsRequired();
        builder.Property(x => x.RelativePath).HasMaxLength(500).IsRequired();
        builder.Property(x => x.MimeType).HasMaxLength(100).IsRequired();
        builder.Property(x => x.MediaType).HasConversion<string>().HasMaxLength(20);

        builder.HasIndex(x => x.StoredFileName).IsUnique();

        builder.ToTable(table => table.HasCheckConstraint(
            "CK_Media_FileSize",
            "\"FileSize\" > 0"));
    }
}
