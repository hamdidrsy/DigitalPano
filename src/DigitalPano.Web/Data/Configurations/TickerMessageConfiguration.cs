using DigitalPano.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalPano.Web.Data.Configurations;

public sealed class TickerMessageConfiguration : IEntityTypeConfiguration<TickerMessage>
{
    public void Configure(EntityTypeBuilder<TickerMessage> builder)
    {
        builder.Property(x => x.Text).HasMaxLength(1000).IsRequired();
        builder.HasIndex(x => new { x.IsActive, x.StartDateUtc, x.EndDateUtc });

        builder.ToTable(table => table.HasCheckConstraint(
            "CK_TickerMessages_DateRange",
            "\"EndDateUtc\" > \"StartDateUtc\""));
    }
}
