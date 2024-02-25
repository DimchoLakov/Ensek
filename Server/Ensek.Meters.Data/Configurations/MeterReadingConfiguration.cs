using Ensek.Meters.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ensek.Meters.Data.Configurations;

public class MeterReadingConfiguration : IEntityTypeConfiguration<MeterReading>
{
    public void Configure(EntityTypeBuilder<MeterReading> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .HasIndex(x => new { x.MeterRead, x.MeterReadingDateTime })
            .IsUnique();
    }
}
