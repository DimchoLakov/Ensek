using Ensek.Meters.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ensek.Meters.Data.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.FirstName)
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(256);

        builder
            .Property(x => x.LastName)
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(256);

        builder
            .HasMany(x => x.MeterReadings)
            .WithOne(x => x.Account)
            .HasForeignKey(x => x.AccountId);
    }
}
